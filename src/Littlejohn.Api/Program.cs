#pragma warning disable CA1852
// this warning is a false positive, it should not be emitted on auto-generated entrypoint classes
// and it will be fixed in a future version of .NET 7

using System.Reflection;
using System.Runtime.InteropServices;
using Littlejohn.Api.Authentication;
using Littlejohn.Api.Extensions;
using Littlejohn.Api.Tickers;
using Littlejohn.Procedural.Portfolios;
using Littlejohn.Procedural.Tickers;
using Littlejohn.Services.Portfolios;
using Littlejohn.Services.Tickers;
using Serilog;
using Serilog.Formatting.Compact;

#pragma warning disable CA1305 // False positive
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
#pragma warning restore CA1305

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        var config = configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();

        if (context.HostingEnvironment.IsDevelopment() ||
            Environment.GetEnvironmentVariable("HUMAN_READABLE_LOGS") is not null)
        {
#pragma warning disable CA1305 // false positive
            config = config.WriteTo.Console();
#pragma warning restore CA1305
        }
        else
        {
            config.WriteTo.Console(new CompactJsonFormatter());
        }
    });

    // Add services to the container.

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.Configure<BasicAuthenticationOptions>(builder.Configuration.GetSection("BasicAuthentication"));
    builder.Services.AddAuthorization()
        .AddAuthentication(BasicAuthenticationOptions.SchemeName)
        .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(BasicAuthenticationOptions.SchemeName, _ => { });
    builder.Services.AddSingleton<IPortfolioRepository, ProceduralPortfolioRepository>();
    builder.Services.AddSingleton<ITickerRepository, ProceduralTickerRepository>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
    }

    app.UseAuthorization();

    app.MapGet("/env", (IHostEnvironment env, HttpContext context) => new EnvInfo
    (
        Version: Assembly
            .GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "unknown",
        Runtime: $"{Environment.Version}+{RuntimeInformation.RuntimeIdentifier}",
        OS: RuntimeInformation.OSDescription,
        Env: env.EnvironmentName,
        Username: context.GetUsername()
    ))
    .Produces<EnvInfo>()
    .WithOpenApi()
    .WithName("EnvInfo")
    .WithDescription("Return info about the execution environment");

    app.MapTickers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error during application bootstrap");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

internal record EnvInfo(string Version, string Runtime, string OS, string Env, string? Username);
