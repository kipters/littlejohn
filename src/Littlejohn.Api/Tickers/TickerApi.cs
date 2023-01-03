using Littlejohn.Api.Extensions;
using Littlejohn.Api.Portfolios;

namespace Littlejohn.Api.Tickers;

internal static class TickerApi
{
    public static RouteGroupBuilder MapTickers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/tickers");

        group.WithTags("Tickers");

        group.RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetUserTickers);

        return group;
    }

    public static IResult GetUserTickers(
        HttpContext context,
        IPortfolioRepository portfolioRepository
        )
    {
        var username = context.GetUsername();

        if (username is null)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["username"] = new[] { "Invalid username" }
            });
        }

        var symbols = portfolioRepository.GetSymbolsInUserPortfolio(username);

        var tickers = symbols.Select(s => new Ticker(s, 123.4m));

        return Results.Ok(tickers);
    }
}
