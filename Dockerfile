ARG DOTNET_VERSION=7.0
ARG ALPINE_VERSION=3.16

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine${ALPINE_VERSION} as build-base
ENV RUNTIME=alpine

FROM build-base AS build-base-amd64
ENV RUNTIME=$RUNTIME-x64

FROM build-base AS build-base-arm64
ENV RUNTIME=$RUNTIME-arm64

FROM --platform=$BUILDPLATFORM build-base-${TARGETARCH} AS restore
WORKDIR /projects
COPY ./src/**/*.csproj ./
COPY ./Directory.Build.props ./
RUN dotnet restore --runtime $RUNTIME

FROM restore AS build
ARG COMMIT_HASH
RUN mkdir -p /app
COPY Directory.Build.props /app
COPY ./src /app/src
RUN dotnet publish \
    --configuration Release \
    --output /dist \
    -p:SourceRevisionId=${COMMIT_HASH} \
    /app/src/Littlejohn.Api

FROM --platform=${TARGETPLATFORM} mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-alpine${ALPINE_VERSION}
COPY --from=build /dist /app
WORKDIR /app
ENTRYPOINT [ "/app/Littlejohn.Api" ]
