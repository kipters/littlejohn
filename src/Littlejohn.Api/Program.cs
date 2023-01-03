#pragma warning disable CA1852
// this warning is a false positive, it should not be emitted on auto-generated entrypoint classes
// and it will be fixed in a future version of .NET 7

using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Littlejohn.Api.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization()
    .AddAuthentication(BasicAuthenticationOptions.SchemeName)
    .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(BasicAuthenticationOptions.SchemeName, _ => { });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapGet("/env", (IHostEnvironment env, HttpContext context) => new
{
    Version = Assembly
        .GetEntryAssembly()?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? "unknown",
    Runtime = $"{Environment.Version}+{RuntimeInformation.RuntimeIdentifier}",
    OS = RuntimeInformation.OSDescription,
    Env = env.EnvironmentName,
    Username = context.User?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
});

app.Run();
