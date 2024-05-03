using System;
using System.Collections;
using System.Collections.Generic;
using TrulyRandom.Models;

namespace TrulyRandom.Modules.Extractors;

/// <summary>
/// Applies Von Neumann's algotithm to the input data thus deskewing the sequence and compressing it.
/// </summary>
public class VonNeumannExtractor : Extractor
{
    private int nesting = 1;
    /// <summary>
    /// Number of times Von Neumann's algotithm is applied to the data.
    /// </summary>
    public int Nesting
    {
        get => nesting;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException("Nesting should be >= 1");
            }
            nesting = value;
        }
    }

    ///<inheritdoc/>
    protected override bool UseDefaultCompressionCalculator => true;

    ///<inheritdoc/>
    protected override int GetActualBatchSize()
    {
        return BatchSize;
    }

    ///<inheritdoc/>
    protected override byte[] ProcessData(byte[] data)
    {
        int additionalNesting = 0;
        if (DynamicCoefficient > 0.5 && DynamicCoefficient <= 0.8)
        {
            additionalNesting = 1;
        }
        else if (DynamicCoefficient > 0.8)
        {
            additionalNesting = 2;
        }

        return ProcessData(new BitArray(data), Nesting + additionalNesting);
    }

    //We take bit pairs and copy the first bit to the output, but only if these bits are different:
    // 00 -> -
    // 01 -> 0
    // 10 -> 1
    // 11 -> -
    private byte[] ProcessData(BitArray data, int nesting)
    {
        List<bool> result = new();

        for (int i = 0; i < data.Length / 2; i++)
        {
            if (data[2 * i] != data[2 * i + 1])
            {
                result.Add(data[2 * i]);
            }
        }

        if (nesting == 1)
        {
            return result.ToArray().ToByteArray();
        }
        return ProcessData(new BitArray(result.ToArray()), nesting - 1);
    }
}
