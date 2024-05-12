using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TrulyRandom.Modules.Extractors;

/// <summary>
/// Uses a MD5 (Message Digest 5) hash function with 128 bit block size to compress data, eliminating dependencies within it and increasing its entropy.
/// </summary>
public class Md5Extractor : HashExtractor
{
    /// <inheritdoc/>
    protected override HashAlgorithm CreateHash()
    {
        return MD5.Create();
    }
}
