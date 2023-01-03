namespace Littlejohn.Api.Portfolios;

public interface IPortfolioRepository
{
    IEnumerable<string> GetSymbolsInUserPortfolio(string username);
}
