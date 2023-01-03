using Microsoft.AspNetCore.Authentication;

namespace Littlejohn.Api.Authentication;

internal sealed class BasicAuthenticationOptions : AuthenticationSchemeOptions
{
    public static string SchemeName = "BasicScheme";
    public bool EnforceEmptyPassword { get; set; }
}
