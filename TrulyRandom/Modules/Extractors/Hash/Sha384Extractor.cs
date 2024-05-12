using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TrulyRandom.Modules.Extractors;

/// <summary>
/// Uses a SHA2 (Secure Hash Algorithm 2) hash function with 384 bit block size to compress data, eliminating dependencies within it and increasing its entropy.
/// </summary>
public class Sha384Extractor : HashExtractor
{
    /// <inheritdoc/>
    protected override HashAlgorithm CreateHash()
    {
        return SHA384.Create();
    }
}
