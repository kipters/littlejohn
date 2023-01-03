namespace Littlejohn.Services.Tickers;

public interface ITickerRepository
{
    TickerPricePoint GetTickerValue(string symbol, DateOnly tickerDate);
    IEnumerable<TickerPricePoint> GetTickerValue(string symbol, DateOnly firstDate, uint days);
}
