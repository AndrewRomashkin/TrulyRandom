using System;
using System.IO;
using System.IO.Compression;
using TrulyRandom.Models;

namespace TrulyRandom.Modules.Extractors;

/// <summary>
/// Uses Deflate compression algorithm to maximize data entropy.
/// </summary>
public class DeflateExtractor : Extractor
{
    private CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Fastest;

    ///<inheritdoc/>
    protected override bool UseDefaultCompressionCalculator => true;

    ///<inheritdoc/>
    protected override int GetActualBatchSize()
    {
        return BatchSize;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeflateExtractor" /> class
    /// </summary>
    public DeflateExtractor()
    {
        MixDataFromDifferentSources = false;
        BatchSize = 100_000;
    }

    ///<inheritdoc/>
    protected override byte[] ProcessData(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        MemoryStream stream = new();

        CompressionLevel adjustedCompressionLevel = CompressionLevel;
        if (DynamicCoefficient > 0.5)
        {
            adjustedCompressionLevel = CompressionLevel.Optimal;
        }

        using (DeflateStream deflateStream = new(stream, adjustedCompressionLevel))
        {
            deflateStream.Write(data, 0, data.Length);
        }

        byte[] result = stream.ToArray();

        return result;
    }
}
