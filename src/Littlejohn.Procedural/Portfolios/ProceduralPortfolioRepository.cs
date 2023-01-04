using System.Security.Cryptography;
using System.Text;
using Littlejohn.Services.Portfolios;
using static Littlejohn.Services.Tickers.TickerConstants;

namespace Littlejohn.Procedural.Portfolios;

#pragma warning disable CA1812 // This class is intended to be instantiated by DI
public sealed class ProceduralPortfolioRepository : IPortfolioRepository
{
    public IEnumerable<string> GetSymbolsInUserPortfolio(string username)
    {
        ArgumentNullException.ThrowIfNull(username);
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("The username must not be empty", nameof(username));
        }

        // We need to generate portfolios in a stable pseudo-random way for any possible user, so the username
        // (or its hash) are the only candidate as a seed.

        var usernameData = Encoding.UTF8.GetBytes(username);

        // We don't need a cryptographically secure hash, but SHA256 is likely be accelerated on most platforms
        var hash = SHA256.HashData(usernameData);

        // We only need a 20-bit map
        var bitmap = BitConverter.ToUInt32(hash);
        return BitmaskToPortfolio(bitmap);
    }

    internal static IEnumerable<string> BitmaskToPortfolio(uint bitmap)
    {
        if ((bitmap & 0x0F_FF_FF) == 0)
        {
#pragma warning disable CA5394 // We don't need a cryptographically secure RNG, Random is enough
            var randomIndex = Random.Shared.Next(0, AllowedTickers.Length);
#pragma warning restore CA5394
            yield return AllowedTickers[randomIndex];
            yield break;
        }

        var mask = 1;
        var left = 10;

        for (var i = 0; i < AllowedTickers.Length; i++)
        {
            if ((bitmap & mask) == mask)
            {
                yield return AllowedTickers[i];
                left -= 1;
            }

            if (left == 0)
            {
                yield break;
            }

            mask <<= 1;
        }
    }
}
