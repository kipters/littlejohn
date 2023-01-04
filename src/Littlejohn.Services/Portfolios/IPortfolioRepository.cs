namespace Littlejohn.Services.Portfolios;

public interface IPortfolioRepository
{
    IEnumerable<string> GetSymbolsInUserPortfolio(string username);
}
