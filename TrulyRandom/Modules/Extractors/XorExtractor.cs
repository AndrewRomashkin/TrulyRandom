using System;
using TrulyRandom.Models;

namespace TrulyRandom.Modules.Extractors;

/// <summary>
/// Splits the block into multiple parts and XORs them together, compressing the data and thus increasing its entropy.
/// </summary>
public class XorExtractor : Extractor, ISeedable
{
    /// <inheritdoc/>
    protected override bool UseDefaultCompressionCalculator => false;

    /// <inheritdoc/>
    protected override int GetActualBatchSize()
    {
        int multiplier = 1;
        if (DynamicCoefficient > 0.2 && DynamicCoefficient < 0.5)
        {
            multiplier = 2;
        }
        else if (DynamicCoefficient > 0.5 && DynamicCoefficient <= 0.8)
        {
            multiplier = 3;
        }
        else if (DynamicCoefficient > 0.8)
        {
            multiplier = 5;
        }

        return Math.Min(BatchSize * multiplier, MaxBatchSize);
    }

    private int compression = 2;
    /// <summary>
    /// Determines how much will data be compressed.
    /// </summary>
    public int Compression
    {
        get => compression;
        set
        {
            if (compression < 1)
            {
                throw new ArgumentException("Compression should be more than or equal to 1");
            }
            compression = value;
            ActualCompression = value;
        }
    }

    //ISeedable members

    /// <inheritdoc/>
    protected override bool Seedable => true;
    /// <summary>
    /// Length of the seed. It is recommended to select lengths which are coprime with 
    /// </summary>
    public int SeedLength { get => seedLength; set => seedLength = value; }
    /// <inheritdoc/>
    public int SeedRotationInterval { get => seedRotationInterval; set => seedRotationInterval = value; }
    /// <inheritdoc/>
    public Module SeedSource { get => seedSource; set => seedSource = value; }
    /// <inheritdoc/>
    public void SetSeed(byte[] data)
    {
        if (data == null || data.Length < 1)
        {
            throw new ArgumentException($"Seed should be at least 1 byte long");
        }
        seed = data;
    }
    /// <inheritdoc/>
    public void ForceRotateSeed()
    {
        RotateSeed(true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XorExtractor" /> class.
    /// </summary>
    public XorExtractor()
    {
        SeedLength = 61;
    }

    /// <inheritdoc/>
    protected override byte[] ProcessData(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        int compression = Compression;
        int resultLength = data.Length / compression;
        byte[] result = new byte[resultLength];

        for (int i = 0; i < resultLength; i++)
        {
            result[i] = seed[i % seed.Length];
            for (int j = 0; j < compression; j++)
            {
                result[i] ^= data[i + j * resultLength];
            }
        }

        return result;
    }
}
