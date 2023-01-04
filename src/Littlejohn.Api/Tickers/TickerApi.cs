using Littlejohn.Api.Extensions;
using Littlejohn.Services.Portfolios;
using Littlejohn.Services.Tickers;

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
        group.MapGet("/{symbol}/history", GetTickerHistory);

        return group;
    }

    private static IResult GetTickerHistory(string symbol, DateOnly? date, ITickerRepository tickerRepository)
    {
        if (!TickerConstants.AllowedTickers.Contains(symbol))
        {
            return Results.NotFound();
        }

        var today = DateTime.Today.ToDateOnly();
        var selectedDate = date ?? today;

        if (selectedDate > today)
        {
            selectedDate = today;
        }

        var history = tickerRepository.GetTickerValue(symbol, selectedDate, 90);
        return Results.Ok(history);
    }

    public static IResult GetUserTickers(
        HttpContext context,
        IPortfolioRepository portfolioRepository,
        ITickerRepository tickerRepository
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

        var tickers = symbols.Select(s =>
        {
            var pricePoint = tickerRepository.GetTickerValue(s, DateOnly.FromDateTime(DateTime.UtcNow));
            return new Ticker(s, pricePoint.Price);
        });

        return Results.Ok(tickers);
    }
}
