using System.Security.Claims;

namespace Littlejohn.Api.Extensions;

public static class HttpContextExtensions
{
    public static string? GetUsername(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.User?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}
