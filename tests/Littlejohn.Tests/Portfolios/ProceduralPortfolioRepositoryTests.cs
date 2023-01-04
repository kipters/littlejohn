using Littlejohn.Procedural.Portfolios;

namespace Littlejohn.Tests.Portfolios;

public class ProceduralPortfolioRepositoryTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ThrowsForInvalidUsernames(string username)
    {
        var sut = new ProceduralPortfolioRepository();

        Assert.Throws<ArgumentException>(() => sut.GetSymbolsInUserPortfolio(username));
    }

    [Fact]
    public void ThrowsForNullUsernames()
    {
        var sut = new ProceduralPortfolioRepository();

        Assert.Throws<ArgumentNullException>(() => sut.GetSymbolsInUserPortfolio(null!));
    }

    [Fact]
    public void PortfoliosAreStableBetweenInstances()
    {
        var firstSut = new ProceduralPortfolioRepository();
        var secondSut = new ProceduralPortfolioRepository();

        var firstPortfolio = firstSut.GetSymbolsInUserPortfolio("foo");
        var secondPortfolio = secondSut.GetSymbolsInUserPortfolio("foo");

        Assert.Equal(firstPortfolio, secondPortfolio);
    }

    [Fact]
    public void PortfoliosAreStableBetweenCalls()
    {
        var sut = new ProceduralPortfolioRepository();

        var firstPortfolio = sut.GetSymbolsInUserPortfolio("foo");
        var secondPortfolio = sut.GetSymbolsInUserPortfolio("foo");

        Assert.Equal(firstPortfolio, secondPortfolio);
    }

    [Fact]
    public void DifferentUsernamesGetDifferentPortfolios()
    {
        var sut = new ProceduralPortfolioRepository();

        var firstPortfolio = sut.GetSymbolsInUserPortfolio("foo");
        var secondPortfolio = sut.GetSymbolsInUserPortfolio("bar");

        Assert.NotEqual(firstPortfolio, secondPortfolio);
    }

    // These next two tests are BAD.
    // They test the implementation rather than the intended behavior
    // Since the implementation relies on SHA256, there's no other option
    // other than finding arbitrary hash collisions, and if that was possible
    // we'd have way bigger problems than bad tests.
    [Fact]
    public void AllZeroMapsGetAtLeastOneTickerAnyway()
    {
        var portfolio = ProceduralPortfolioRepository.BitmaskToPortfolio(0x00);

        Assert.True(portfolio.Any());
    }

    [Theory]
    [InlineData(0xFFFF)]
    [InlineData(0xF0FF)]
    [InlineData(0xAAAA)]
    public void PortfoliosDontGetMoreThanTenTickers(uint bitmap)
    {
        var portfolio = ProceduralPortfolioRepository.BitmaskToPortfolio(bitmap);

        Assert.True(portfolio.Count() <= 10);
    }
}
