#define _GNU_SOURCE
#include <stdatomic.h>
#include <semaphore.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/mman.h>
#include <sys/wait.h>
#include <time.h>
#include <unistd.h>

typedef struct {
    _Atomic uint64_t request;
    _Atomic uint64_t response;
    _Atomic int payload;
    _Atomic int result;
    sem_t request_ready;
    sem_t response_ready;
} shared_t;

static shared_t *make_shared(void)
{
    shared_t *s = mmap(NULL, sizeof(shared_t), PROT_READ | PROT_WRITE,
                       MAP_SHARED | MAP_ANONYMOUS, -1, 0);
    if (s == MAP_FAILED) { perror("mmap"); exit(1); }
    memset((void *)s, 0, sizeof(shared_t));
    return s;
}

static double ns_per_call(struct timespec a, struct timespec b, uint64_t n)
{
    double ns = (b.tv_sec - a.tv_sec) * 1e9 + (b.tv_nsec - a.tv_nsec);
    return ns / (double)n;
}

static void run_spin(shared_t *s, uint64_t n)
{
    pid_t pid = fork();
    if (pid == 0) {
        for (uint64_t expected = 1; expected <= n; expected++) {
            while (atomic_load_explicit(&s->request, memory_order_acquire) < expected) { }
            atomic_store_explicit(&s->result,
                atomic_load_explicit(&s->payload, memory_order_relaxed) + 1,
                memory_order_relaxed);
            atomic_store_explicit(&s->response, expected, memory_order_release);
        }
        _exit(0);
    }

    struct timespec a, b;
    clock_gettime(CLOCK_MONOTONIC, &a);
    for (uint64_t i = 1; i <= n; i++) {
        atomic_store_explicit(&s->payload, (int)i, memory_order_relaxed);
        atomic_store_explicit(&s->request, i, memory_order_release);
        while (atomic_load_explicit(&s->response, memory_order_acquire) != i) { }
    }
    clock_gettime(CLOCK_MONOTONIC, &b);
    waitpid(pid, NULL, 0);
    printf("spin (busy-poll)       %10.1f ns/roundtrip\n", ns_per_call(a, b, n));
}

static void run_blocking(shared_t *s, uint64_t n)
{
    sem_init(&s->request_ready, 1, 0);
    sem_init(&s->response_ready, 1, 0);
    pid_t pid = fork();
    if (pid == 0) {
        for (uint64_t i = 1; i <= n; i++) {
            sem_wait(&s->request_ready);
            atomic_store_explicit(&s->result,
                atomic_load_explicit(&s->payload, memory_order_relaxed) + 1,
                memory_order_relaxed);
            sem_post(&s->response_ready);
        }
        _exit(0);
    }

    struct timespec a, b;
    clock_gettime(CLOCK_MONOTONIC, &a);
    for (uint64_t i = 1; i <= n; i++) {
        atomic_store_explicit(&s->payload, (int)i, memory_order_relaxed);
        sem_post(&s->request_ready);
        sem_wait(&s->response_ready);
    }
    clock_gettime(CLOCK_MONOTONIC, &b);
    waitpid(pid, NULL, 0);
    sem_destroy(&s->request_ready);
    sem_destroy(&s->response_ready);
    printf("blocking (semaphore)   %10.1f ns/roundtrip\n", ns_per_call(a, b, n));
}

int main(int argc, char **argv)
{
    uint64_t n = argc > 1 ? strtoull(argv[1], NULL, 10) : 1000000;
    printf("N=%lu roundtrips per variant\n", (unsigned long)n);
    shared_t *s = make_shared();
    run_spin(s, n);
    atomic_store_explicit(&s->request, 0, memory_order_relaxed);
    atomic_store_explicit(&s->response, 0, memory_order_relaxed);
    run_blocking(s, n);
    return 0;
}
