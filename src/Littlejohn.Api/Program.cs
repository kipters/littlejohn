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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
));

app.MapTickers();

app.Run();

internal record EnvInfo(string Version, string Runtime, string OS, string Env, string? Username);
