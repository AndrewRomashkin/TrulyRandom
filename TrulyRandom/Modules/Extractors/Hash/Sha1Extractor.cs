using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TrulyRandom.Modules.Extractors;

/// <summary>
/// Uses a SHA1 (Secure Hash Algorithm 1) hash function with 160 bit block size to compress data, eliminating dependencies within it and increasing its entropy.
/// </summary>
public class Sha1Extractor : HashExtractor
{
    /// <inheritdoc/>
    protected override HashAlgorithm CreateHash()
    {
        return SHA1.Create();
    }
}
