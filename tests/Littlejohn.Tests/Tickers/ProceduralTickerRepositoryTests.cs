using System.Globalization;
using Littlejohn.Procedural.Tickers;

namespace Littlejohn.Tests.Tickers;

public class ProceduralTickerRepositoryTests
{
    private DateOnly Today { get; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public void ThrowsOnInvalidTicker()
    {
        var sut = new ProceduralTickerRepository();

        Assert.Throws<ArgumentException>(() => sut.GetTickerValue("FOO", Today));
        Assert.Throws<ArgumentException>(() => sut.GetTickerValue("FOO", Today, 90).ToList());
    }

    [Fact]
    public void ThrowsOnNonPositiveNumberOfDays()
    {
        var sut = new ProceduralTickerRepository();

        Assert.Throws<ArgumentException>(() => sut.GetTickerValue("AAPL", Today, 0).ToList());
    }

    [Fact]
    public void TickerValuesAreStableBetweenRuns()
    {
        var firstSut = new ProceduralTickerRepository();
        var secondSut = new ProceduralTickerRepository();

        var firstRun = firstSut.GetTickerValue("AAPL", Today, 30);
        var secondRun = secondSut.GetTickerValue("AAPL", Today, 30);

        Assert.Equal(firstRun, secondRun);
    }

    [Fact]
    public void TickerValuesAreDifferentBetweenSymbols()
    {
        var sut = new ProceduralTickerRepository();
        var firstSymbol = sut.GetTickerValue("AAPL", Today);
        var secondSymbol = sut.GetTickerValue("MSFT", Today);

        Assert.NotEqual(firstSymbol, secondSymbol);
    }

    [Fact]
    public void GeneratedDatesAreInDescendingOrder()
    {
        var sut = new ProceduralTickerRepository();
        var prices = sut.GetTickerValue("GOOG", Today, 90);

        var dates = prices.Select(p => p.Date.ToString("O", CultureInfo.InvariantCulture)).ToList();
        var orderedDates = prices.Select(p => p.Date.ToString("O", CultureInfo.InvariantCulture)).OrderDescending();

        Assert.Equal(orderedDates, dates);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(300)]
    public void TheCorrectNumberOfDaysIsGenerated(uint days)
    {
        var sut = new ProceduralTickerRepository();
        var prices = sut.GetTickerValue("GOOG", Today, days).ToArray();

        Assert.Equal(days, (uint)prices.Length);
    }
}
