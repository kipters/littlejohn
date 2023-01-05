# Littlejohn

Stock tracking service

## Development environment

The preferred development environment is to use the Dev Container feature of
Visual Studio Code.

It requires VSCode, Docker and the Remote Development extension pack. VS Code
will prompt the user to install the latter when opening the repo, as it's
listed in the recommended extensions for the workspace.

When opening the repo with all extensions installed, you will be prompted to
launch the devcontainer, giving you a fully setup and self-contained
environment. More info can be found
[here](https://kipters.dev/posts/vscode-dev-containers/).

GitHub Codespaces are supported too using the same configuration (requires a Pro
subscription).

Alternatively, both Visual Studio and Rider are supported, the only prerequisite
is to install the .NET 7.0 SDK.

Both arm64 and amd64 machines are supported, on either Windows, macOS or Linux.

## Configuration

The application can be run in Development, Staging or Production mode.
In the current state there's virtually no difference between environments,
except Development, which enables the following features:

- HTTPS server (using .NET's default dev certificate)
- Automatic redirection to HTTPS on HTTP endpoints
- Swagger generation and Swagger UI
- Human readable logs

The environment is selected setting the `DOTNET_ENVIRONMENT` environment
variable to either Development, Staging or Production. When the variable is not
set the default is Production.

Application configuration is the result of different sources which are overlayed
one on top of each other, in this order:

- appsettings.json file (mandatory)
- appsettings.{environment name}.json (optional)
- environment variables

where each one can override the previous with single key granularity.

Objects in json files represent configuration sections, i.e.

```json
{
    "Foo": {
        "Bar": 5,
        "Baz": 7
    }
}
```

Defines a section `Foo` with two keys `Bar` and `Baz`.
The fully qualified version of those keys would be `Foo:Bar` and `Foo:Baz` and
thus each one of them can be expressed in environment variables by setting
`FOO__BAR` or `FOO__BAZ` (note the double underscore).

The application only has one tweakable option, on key
`BasicAuthentication:EnforceEmptyPassword`. Setting it to false will _allow_
passwords to be empty when using Basic authentication, setting it to true will
_require_ passwords to be empty.

## Logging

In Development mode, human readable logs will be emitted using one single line
for each message.
In any other mode, JSON structured logs will be emitted instead. Human readable
logs can be forced in any mode by setting the `HUMAN_READABLE_LOGS` environment
variable to any value.

## Running

The main application is in `src/Littlejohn.Api`. It can be run with
`dotnet run` in the same folder or by passing its path via the `-p` parameter.

Optionally, passing `-c Release` will run the application with optimizations
turned on.

Of course the easiest way to launch the application during development is to use
the launch/debug button in your IDE of choice.

When running locally, port 5120 will be used for HTTP, and port 7077 will be
used for HTTPS.

## Building

`dotnet build` will build the application so it can be run locally using
`dotnet exec`, while `dotnet publish` will output artifacts that can be deployed
elsewhere, while still requiring the runtime to be installed.

The `--self-contained` parameter will produce a fully self contained version of
the application that includes the necessary runtime.

Optionally, passing `-c Release` will build the application with optimizations
turned on.

## Docker

A dockerfile is provided, which requires Docker BuildKit to be enabled.
This can be done by either setting the `DOCKER_BUILDKIT` environment variable
to 1 or by using the `docker buildx build` command instead of `docker build`.

This is required for cross-building images for arm64 on amd64 machines and
viceversa. Both architectures are fully supported, for ease of development on
modern arm64-based laptops or to take advantage of the best cost-to-performance
ratio of Graviton instances on AWS.

The container will expose the application via HTTP only on port 80.
