using System.Security.Cryptography;
using System.Text;
using Littlejohn.Services.Tickers;

namespace Littlejohn.Procedural.Tickers;

#pragma warning disable CA5394 // Random is not cryptographically secure, but we don't need it to be

public class ProceduralTickerRepository : ITickerRepository
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

    public TickerPricePoint GetTickerValue(string symbol, DateOnly tickerDate)
    {
        if (!TickerConstants.AllowedTickers.Contains(symbol))
        {
            throw new ArgumentException("Invalid ticker symbol", nameof(symbol));
        }

        return GetTickerValueImpl(GetSymbolSeed(symbol), tickerDate);
    }

    private static uint GetSymbolSeed(string symbol)
    {
        var symbolBytes = Encoding.UTF8.GetBytes(symbol);
        var symbolHash = SHA256.HashData(symbolBytes);
        return BitConverter.ToUInt32(symbolHash);
    }

    private static TickerPricePoint GetTickerValueImpl(uint symbolSeed, DateOnly tickerDate)
    {
        var deltaDays = (tickerDate.ToDateTime(TimeOnly.MinValue) - Epoch).TotalDays;
        var seed = HashCode.Combine(deltaDays, symbolSeed);
        var rnd = new Random(seed);
        var value = rnd.Next(20_000) / 100m;

        return new(tickerDate, value);
    }

    public IEnumerable<TickerPricePoint> GetTickerValue(string symbol, DateOnly firstDate, uint days)
    {
        if (!TickerConstants.AllowedTickers.Contains(symbol))
        {
            throw new ArgumentException("Invalid ticker symbol", nameof(symbol));
        }

        if (days <= 0)
        {
            throw new ArgumentException("Value should be equal or higher than 1", nameof(days));
        }

        var symbolSeed = GetSymbolSeed(symbol);

        for (var i = 0; i < days; i++)
        {
            yield return GetTickerValueImpl(symbolSeed, firstDate.AddDays(-i));
        }
    }
}
