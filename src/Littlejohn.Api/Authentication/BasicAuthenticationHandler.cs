using System.Buffers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Littlejohn.Api.Authentication;

#pragma warning disable CA1812
internal sealed class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options
        , ILoggerFactory logger
        , UrlEncoder encoder
        , ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headerExists = Context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authHeader);

        if (headerExists == false)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var authValue = authHeader.SingleOrDefault();

        if (authValue is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Only one Authorization header is allowed"));
        }

        if (!authValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        return Task.FromResult(ValidateToken(authValue[6..]));
    }

    private AuthenticateResult ValidateToken(string token)
    {
        var buffer = Array.Empty<byte>();
        try
        {
            buffer = ArrayPool<byte>.Shared.Rent(token.Length);
            if (!Convert.TryFromBase64String(token, buffer, out var tokenBytesLength))
            {
                return AuthenticateResult.Fail("Invalid authorization data");
            }

            var decodedToken = Encoding.UTF8.GetString(buffer, 0, tokenBytesLength);

            var tokenParts = decodedToken.Split(':', StringSplitOptions.TrimEntries);

            if (tokenParts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid authorization data");
            }

            if (tokenParts[0].Length == 0 ||
                (Options.EnforceEmptyPassword && tokenParts[1].Length > 0))
            {
                return AuthenticateResult.Fail("Invalid credentials");
            }

            var claimsPrincipal = new ClaimsPrincipal();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, tokenParts[0]) };
            var claimsIdentity = new ClaimsIdentity(claims, BasicAuthenticationOptions.SchemeName);
            claimsPrincipal.AddIdentity(claimsIdentity);

            var ticket = new AuthenticationTicket(claimsPrincipal, BasicAuthenticationOptions.SchemeName);
            return AuthenticateResult.Success(ticket);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
