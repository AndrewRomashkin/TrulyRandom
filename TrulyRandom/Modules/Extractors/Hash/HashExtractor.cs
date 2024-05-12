using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TrulyRandom.Models;

namespace TrulyRandom.Modules.Extractors;

/// <summary>
/// Base class for extractors which use hash function to compress data, eliminating dependencies within it and increasing its entropy.
/// </summary>
public abstract class HashExtractor : Extractor, ISeedable
{
    /// <summary>
    /// Size of the block processed by the hash function.
    /// </summary>
    public int InputBlockSize { get; set; } = 256;

    /// <summary>
    /// Size of the output block after being processed by the hash function.
    /// </summary>
    public int OutputBlockSize => hash.HashSize / 8;

    /// <summary>
    /// Hash function used to process the data.
    /// </summary>
    private HashAlgorithm hash;

    /// <summary>
    /// Determines whether every output block should be XOR-ed with the previous one.
    /// </summary>
    public bool Chaining { get; set; } = true;

    /// <inheritdoc/>
    protected override bool UseDefaultCompressionCalculator => false;

    //ISeedable members

    /// <inheritdoc/>
    protected override bool Seedable => true;
    /// <inheritdoc/>
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
    /// Initializes a new instance of the <see cref="HashExtractor" /> class.
    /// </summary>
    protected HashExtractor()
    {
        hash = CreateHash();
        BatchSize = 10_000;
        ActualCompression = (double)InputBlockSize / hash.HashSize * 8;
    }

    /// <summary>
    /// Initializes a new instance of the hash algorithm.
    /// </summary>
    protected abstract HashAlgorithm CreateHash();

    private int currentMultiplier;
    ///<inheritdoc/>
    protected override int GetActualBatchSize()
    {
        currentMultiplier = 1;
        if (DynamicCoefficient > 0.2 && DynamicCoefficient < 0.5)
        {
            currentMultiplier = 2;
        }
        else if (DynamicCoefficient > 0.5 && DynamicCoefficient <= 0.8)
        {
            currentMultiplier = 3;
        }
        else if (DynamicCoefficient > 0.8)
        {
            currentMultiplier = 5;
        }

        return BatchSize / (InputBlockSize * currentMultiplier) * InputBlockSize * currentMultiplier;
    }

    /// <inheritdoc/>
    protected override byte[] ProcessData(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        int inputBlockSize = InputBlockSize * currentMultiplier;
        List<byte> result = new();
        for (int i = 0; i < data.Length / inputBlockSize; i++)
        {
            byte[] hashResult;
            byte[] seed = this.seed;
            if (!Chaining || seed == null)
            {
                hashResult = hash.ComputeHash(data.Subarray(i * inputBlockSize, inputBlockSize).Concat(Utils.GetSystemRandom(SeedLength)));
            }
            else
            {
                hashResult = hash.ComputeHash(data.Subarray(i * inputBlockSize, inputBlockSize).Concat(seed));
            }
            result.AddRange(hashResult);
            this.seed = hashResult;
        }
        ActualCompression = (double)inputBlockSize / hash.HashSize * 8;
        return result.ToArray();
    }
}
