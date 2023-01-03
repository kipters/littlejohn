using System.Collections.Immutable;

namespace Littlejohn.Services.Tickers;

public static class TickerConstants
{
    public static ImmutableArray<string> AllowedTickers { get; } = new[]
    {
        "AAPL",
        "MSFT",
        "GOOG",
        "AMZN",
        "FB",
        "TSLA",
        "NVDA",
        "JPM",
        "BABA",
        "JNJ",
        "WMT",
        "PG",
        "PYPL",
        "DIS",
        "ADBE",
        "PFE",
        "V",
        "MA",
        "CRM",
        "NFLX",
    }.ToImmutableArray();

}
