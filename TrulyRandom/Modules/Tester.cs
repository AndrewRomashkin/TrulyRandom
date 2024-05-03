using System;
using System.Collections.Generic;
using System.Linq;
using TrulyRandom.Models;

namespace TrulyRandom.Modules;

/// <summary>
/// Applies NIST statistical tests to the data.
/// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
public class Tester : Extractor
{
    private readonly List<NistTests.FullTestResult> testResultHistory = new();
    private readonly NistTests tests = new();

    /// <summary>
    /// Parameters of the tests.
    /// </summary>
    public NistTests.TestParameters TestParameters => tests.Parameters;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tester" /> class.
    /// </summary>
    public Tester()
    {
        CalculateEntropy = false;
        BatchSize = 1_000_000;
    }

    ///<inheritdoc/>
    protected override bool UseDefaultCompressionCalculator => true;
    /// <summary>
    /// Determines whether tests should be automatically selected based on the batch size.
    /// </summary>
    public bool AutoSelectTests { get; set; } = true;
    /// <summary>
    /// Determines whether data should be cropped to the size which was actually tested with all enabled tests.
    /// </summary>
    public bool OutputOnlyActuallyTestedBits { get; set; } = true;
    /// <summary>
    /// Determines whether failed batches should be discarded.
    /// </summary>
    public bool OutputOnlySuccessfullyTestedBits { get; set; } = true;
    /// <summary>
    /// Determines whether batches which failed <see cref="NistTests.TestType.RandomExcursions"/> and <see cref="NistTests.TestType.RandomExcursionsVariant"/> tests due to insufficient cycles should be merged and retested.
    /// </summary>
    public bool MergeAndRetestBlocksWithInsufficientCycles { get; set; }
    /// <summary>
    /// Results of last tests performed.
    /// </summary>
    public NistTests.FullTestResult[] TestResultHistory
    {
        get
        {
            lock (testResultHistory)
            {
                return testResultHistory.ToArray();
            }
        }
    }

    /// <summary>
    /// Results of the last test performed.
    /// </summary>
    public NistTests.FullTestResult LastTestResult
    {
        get
        {
            lock (testResultHistory)
            {
                return testResultHistory.LastOrDefault();
            }
        }
    }

    /// <summary>
    /// Proportion of successful tests.
    /// </summary>
    public double SuccessRate
    {
        get
        {
            if (!TestResultHistory.Any())
            {
                return 0;
            }
            lock (testResultHistory)
            {
                if (!TestResultHistory.Any())
                {
                    return 0;
                }
                return TestResultHistory.Count(x => x.Success) / (double)TestResultHistory.Length;
            }
        }
    }

    ///<inheritdoc/>
    protected override int GetActualBatchSize()
    {
        return BatchSize;
    }

    private byte[] insufficientCyclePool;

    ///<inheritdoc/>
    protected override byte[] ProcessData(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        //Autoselecting tests based on batch size
        if (AutoSelectTests)
        {
            NistTests.TestType testsForThisSize = NistTests.TestParameters.TestsForSize(data.Length * 8);
            tests.Parameters.TestsToPerform = testsForThisSize;

            if (testsForThisSize == NistTests.TestType.None)
            {
                return System.Array.Empty<byte>();
            }
        }

        NistTests.FullTestResult testResult = tests.Perform(data);

        if (MergeAndRetestBlocksWithInsufficientCycles && testResult.InsufficientCycles)
        {
            if (insufficientCyclePool == null)
            {
                insufficientCyclePool = data;
                return System.Array.Empty<byte>();
            }
            else
            {
                //Merging batches with insufficient cycles into one pool
                data = insufficientCyclePool.Concat(data);
                insufficientCyclePool = null;
                NistTests.TestType testsToPerform = tests.Parameters.TestsToPerform & (NistTests.TestType.RandomExcursions | NistTests.TestType.RandomExcursionsVariant);
                if (testsToPerform == NistTests.TestType.None)
                {
                    return Array.Empty<byte>();
                }
                testResult = tests.Perform(data, testsToPerform);

                if (testResult.InsufficientCycles)
                {
                    if (data.Length > BatchSize * 10)
                    {
                        //Looks like first block is heavily skewed. Discarding it.
                        insufficientCyclePool = data.Subarray(BatchSize, data.Length - BatchSize);
                    }
                    else
                    {
                        insufficientCyclePool = data;
                    }

                    return System.Array.Empty<byte>();
                }
            }
        }

        lock (testResultHistory)
        {
            testResultHistory.Add(testResult);
            if (testResultHistory.Count > 1000)
            {
                testResultHistory.RemoveAt(0);
            }
        }

        if (!testResult.Success && OutputOnlySuccessfullyTestedBits)
        {
            return System.Array.Empty<byte>();
        }
        if (OutputOnlyActuallyTestedBits)
        {
            return data.Subarray(testResult.ActuallyTestedBits / 8);
        }
        return data;
    }
}
