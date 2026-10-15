# syntax=docker/dockerfile:1

ARG UBUNTU_VERSION=24.04
ARG DOTNET_VERSION=10.0

FROM steamcmd/steamcmd:latest AS server
ARG NANOS_BETA=
ARG SERVER_REFRESH=
RUN echo "server refresh ${SERVER_REFRESH}" \
    && for attempt in 1 2 3; do \
        steamcmd +force_install_dir /nanos +login anonymous \
            +app_update 1936830 ${NANOS_BETA:+-beta ${NANOS_BETA}} +quit \
        && test -x /nanos/NanosWorldServer && break; \
        sleep 5; \
    done \
    && test -x /nanos/NanosWorldServer

FROM mcr.microsoft.com/dotnet/runtime:${DOTNET_VERSION}-noble AS dotnet

FROM ubuntu:${UBUNTU_VERSION}
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates libicu74 libssl3t64 zlib1g \
    && rm -rf /var/lib/apt/lists/* \
    && useradd --create-home --home-dir /home/nanos nanos

ENV DOTNET_ROOT=/usr/share/dotnet \
    DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY --from=dotnet /usr/share/dotnet /usr/share/dotnet
COPY --from=server --chown=nanos:nanos /nanos /nanos
COPY --chown=nanos:nanos build/release/bundle/ /nanos/
RUN chmod +x /nanos/NanosSharp.sh /nanos/NanosWorldServer.sh /nanos/NanosWorldServer

USER nanos
WORKDIR /nanos
EXPOSE 7777/udp 7777/tcp 7778/udp
VOLUME ["/nanos/Packages", "/nanos/Assets"]
ENTRYPOINT ["./NanosSharp.sh"]
