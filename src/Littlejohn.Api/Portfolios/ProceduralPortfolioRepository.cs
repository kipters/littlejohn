using System.Security.Cryptography;
using System.Text;

namespace Littlejohn.Api.Portfolios;

#pragma warning disable CA1812 // This class is intended to be instantiated by DI
internal sealed class ProceduralPortfolioRepository : IPortfolioRepository
{
    internal static string[] AllowedTickers =
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
    };

    public IEnumerable<string> GetSymbolsInUserPortfolio(string username)
    {
        // We need to generate portfolios in a stable pseudo-random way for any possible user, so the username
        // (or its hash) are the only candidate as a seed.

        var usernameData = Encoding.UTF8.GetBytes(username);

        // We don't need a cryptographically secure hash, but SHA256 is likely be accelerated on most platforms
        var hash = SHA256.HashData(usernameData);

        // We only need a 20-bit mask
        var bitmask = BitConverter.ToUInt32(hash);
        var currentmask = 1;

        for (var i = 0; i < AllowedTickers.Length; i++)
        {
            if ((bitmask & currentmask) == currentmask)
            {
                yield return AllowedTickers[i];
            }

            currentmask <<= 1;
        }
    }
}
