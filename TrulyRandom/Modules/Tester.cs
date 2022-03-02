using System.Collections.Generic;
using System.Linq;
using TrulyRandom.Models;

namespace TrulyRandom.Modules
{
    public class Tester : Extractor
    {
        List<NistTests.TestResult> testResultHistory = new List<NistTests.TestResult>();
        NistTests tests { get; } = new NistTests();

        public NistTests.TestParameters TestParameters => tests.Parameters;

        public Tester()
        {
            CalculateEntropy = false;
            BatchSize = 1_000_000;
        }

        protected override bool UseDefaultCompressionCalculator => true;
        public bool AutoSelectTests { get; set; } = true;
        public bool OutputOnlyActuallyTestedBits { get; set; } = true;
        public bool OutputOnlySuccessfullyTestedBits { get; set; } = true;
        public bool MergeAndRetestBlocksWithInsufficientCycles { get; set; } = false;

        public NistTests.TestResult[] TestResultHistory
        {
            get
            {
                lock (testResultHistory)
                {
                    return testResultHistory.ToArray();
                }
            }
        }

        public NistTests.TestResult LastTestResult
        {
            get
            {
                lock (testResultHistory)
                {
                    return testResultHistory.LastOrDefault();
                }
            }
        }

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

        protected override int GetActualBatchSize()
        {
            return BatchSize;
        }

        byte[] insufficientCyclePool = null;

        protected override byte[] ProcessData(byte[] data)
        {
            if (AutoSelectTests)
            {
                NistTests.TestsEnum testsForThisSize = NistTests.TestParameters.TestsForSize(data.Length * 8);
                tests.Parameters.TestsToPerform = testsForThisSize;

                if (testsForThisSize == NistTests.TestsEnum.None)
                {
                    return new byte[0];
                }
            }

            NistTests.TestResult testResult = tests.Perform(data);

            if (MergeAndRetestBlocksWithInsufficientCycles && testResult.InsufficientCycles)
            {
                if (insufficientCyclePool == null)
                {
                    insufficientCyclePool = data;
                    return new byte[0];
                }
                else
                {
                    data = insufficientCyclePool.Concat(data);
                    insufficientCyclePool = null;
                    NistTests.TestsEnum testsToPerform = tests.Parameters.TestsToPerform & (NistTests.TestsEnum.RandomExcursions | NistTests.TestsEnum.RandomExcursionsVariant);
                    if (testsToPerform == NistTests.TestsEnum.None)
                    {
                        return new byte[0];
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

                        return new byte[0];
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
                return new byte[0];
            }
            if (OutputOnlyActuallyTestedBits)
            {
                return data.Subarray(testResult.ActuallyTestedBits / 8);
            }
            return data;
        }
    }
}
