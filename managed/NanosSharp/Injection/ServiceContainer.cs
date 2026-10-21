using System.Reflection;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Logging;
using NanosSharp.Packages;

namespace NanosSharp.Injection;

/// <summary>Holds the singletons of a package, resolves their dependencies and runs their lifecycle.</summary>
public sealed class ServiceContainer
{
    private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private readonly Dictionary<Type, object> _services = [];
    private readonly List<object> _autoRegisters = [];
    private readonly List<ScheduledService> _scheduled = [];
    private readonly List<CancellationTokenSource> _backgroundTokens = [];
    private readonly PackageScope _scope;
    private Action<float>? _tick;

    internal ServiceContainer(PackageScope scope)
    {
        _scope = scope;
        _services[typeof(ServiceContainer)] = this;
    }

    /// <summary>Registers a type as a singleton, created through its parameterless constructor.</summary>
    public void Add<T>() where T : class => Add(Construct(typeof(T)));

    /// <summary>Registers an existing instance as a singleton.</summary>
    public void Add(object instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        if (!_services.TryAdd(instance.GetType(), instance))
        {
            throw new InvalidOperationException($"{instance.GetType().FullName} is already registered.");
        }
    }

    /// <summary>Whether a service assignable to the type is registered.</summary>
    public bool Any<T>() where T : class => Resolve(typeof(T)) is not null;

    /// <summary>Returns the service assignable to the type, or null when it is missing.</summary>
    public T? Get<T>() where T : class => Resolve(typeof(T)) as T;

    /// <summary>Returns the service assignable to the type, or throws when it is missing.</summary>
    public T GetRequired<T>() where T : class =>
        Get<T>() ?? throw new InvalidOperationException($"No service of type {typeof(T).FullName} is registered.");

    /// <summary>Returns every service assignable to the type.</summary>
    public IReadOnlyList<T> GetAll<T>() => _services.Values.OfType<T>().ToArray();

    internal IReadOnlyList<object> AutoRegisters => _autoRegisters;

    internal void AddDiscovered(Type type)
    {
        var instance = Construct(type);
        _services[type] = instance;
        if (instance is IAutoRegister)
        {
            _autoRegisters.Add(instance);
        }

        if (instance is ScheduledService scheduled)
        {
            _scheduled.Add(scheduled);
        }
    }

    internal void ResolveInjections()
    {
        foreach (var service in _services.Values.ToArray())
        {
            Inject(service);
        }
    }

    internal async Task StartAsync(IReadOnlyList<Package> entries)
    {
        using var _ = PackageContext.Enter(_scope);
        try
        {
            foreach (var service in _services.Values.ToArray())
            {
                await CallInitializers(service);
            }

            foreach (var service in GetAll<IService>())
            {
                await StartService(service);
            }

            SubscribeTick();

            foreach (var entry in entries)
            {
                entry.OnStart();
            }
        }
        catch (Exception exception)
        {
            Log.Exception(exception, "The package failed to start its services");
        }
    }

    internal void Stop()
    {
        using var _ = PackageContext.Enter(_scope);
        if (_tick is not null)
        {
            NativeCallbacks.Tick -= _tick;
            _tick = null;
        }

        foreach (var token in _backgroundTokens)
        {
            token.Cancel();
        }

        foreach (var service in GetAll<IService>())
        {
            StopService(service);
        }
    }

    private void SubscribeTick()
    {
        if (_scheduled.Count == 0)
        {
            return;
        }

        _tick = OnTick;
        NativeCallbacks.Tick += _tick;
    }

    private void OnTick(float deltaTime)
    {
        using var _ = PackageContext.Enter(_scope);
        foreach (var service in _scheduled)
        {
            try
            {
                service.Advance(deltaTime);
            }
            catch (Exception exception)
            {
                Log.Exception(exception, $"{service.GetType().Name}.OnTick failed");
            }
        }
    }

    private async Task StartService(IService service)
    {
        if (service is ISkippedService or ScheduledService || service is Service { IsDisabled: true })
        {
            return;
        }

        if (service is BackgroundService background)
        {
            var token = new CancellationTokenSource();
            _backgroundTokens.Add(token);
            _ = Task.Run(() => RunBackground(background, token.Token), token.Token);
            return;
        }

        try
        {
            await service.StartAsync(CancellationToken.None);
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"{service.GetType().Name}.StartAsync failed");
        }
    }

    private void Inject(object instance)
    {
        foreach (var field in instance.GetType().GetFields(MemberFlags))
        {
            if (field.GetCustomAttribute<InjectAttribute>() is not { } attribute)
            {
                continue;
            }

            if (Require(field.FieldType, attribute, instance) is { } service)
            {
                field.SetValue(instance, service);
            }
        }

        foreach (var property in instance.GetType().GetProperties(MemberFlags))
        {
            if (property.GetCustomAttribute<InjectAttribute>() is not { } attribute || !property.CanWrite)
            {
                continue;
            }

            if (Require(property.PropertyType, attribute, instance) is { } service)
            {
                property.SetValue(instance, service);
            }
        }
    }

    private object? Require(Type type, InjectAttribute attribute, object instance)
    {
        var service = Resolve(type);
        if (service is null && attribute.IsRequired)
        {
            throw new InvalidOperationException($"{instance.GetType().FullName} requires a service of type {type.FullName}.");
        }

        return service;
    }

    private object? Resolve(Type type)
    {
        if (_services.TryGetValue(type, out var service))
        {
            return service;
        }

        return _services.Values.FirstOrDefault(type.IsInstanceOfType);
    }

    private static async Task RunBackground(BackgroundService service, CancellationToken token)
    {
        try
        {
            await service.StartAsync(token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"{service.GetType().Name} background work failed");
        }
    }

    private static void StopService(IService service)
    {
        try
        {
            _ = service.StopAsync(CancellationToken.None);
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"{service.GetType().Name}.StopAsync failed");
        }
    }

    private static async Task CallInitializers(object instance)
    {
        foreach (var method in instance.GetType().GetMethods(MemberFlags))
        {
            if (method.GetCustomAttribute<OnInitAttribute>() is null)
            {
                continue;
            }

            var arguments = method.GetParameters().Any(parameter => parameter.ParameterType == typeof(CancellationToken))
                ? [CancellationToken.None]
                : Array.Empty<object>();
            try
            {
                if (method.Invoke(instance, arguments) is Task task)
                {
                    await task;
                }
            }
            catch (Exception exception)
            {
                Log.Exception(exception, $"{instance.GetType().Name} initializer failed");
            }
        }
    }

    private static object Construct(Type type)
    {
        var constructor = type.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException($"{type.FullName} needs a parameterless constructor to be a service.");
        return constructor.Invoke(null);
    }
}
