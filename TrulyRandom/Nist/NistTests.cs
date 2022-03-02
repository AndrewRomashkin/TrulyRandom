using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TrulyRandom
{
    public partial class NistTests
    {
        public static int MaximumThreads { get; set; } = Environment.ProcessorCount;
        public TestParameters Parameters { get; protected set; } = new TestParameters();

        public NistTests()
        {
        }

        public NistTests(TestParameters parameters)
        {
            Parameters = parameters;
        }

        public NistTests(int sequenceSize)
        {
            Parameters = new TestParameters(sequenceSize);
            if (Parameters.TestsToPerform == 0)
            {
                throw new ArgumentException("Sequence size isn't sufficient for any of the tests");
            }
        }

        [Flags]
        public enum TestsEnum
        {
            Frequency = 0x0001, BlockFrequency = 0x0002, Runs = 0x0004, LongestRunOfOnes = 0x0008, BinaryMatrixRank = 0x0010, DiscreteFourierTransform = 0x0020,
            NonOverlappingTemplateMatchings = 0x0040, OverlappingTemplateMatchings = 0x0080, MaurersUniversal = 0x0100, LinearComplexity = 0x0200, Serial = 0x0400,
            ApproximateEntropy = 0x0800, CumulativeSums = 0x1000, RandomExcursions = 0x2000, RandomExcursionsVariant = 0x4000, All = 0x7FFF, None = 0
        }

        List<Dictionary<TestsEnum, double[]>> testHistory = new List<Dictionary<TestsEnum, double[]>>();

        public TestResult Perform(byte[] data)
        {
            return Perform(new BitArray(data), Parameters.TestsToPerform);
        }

        public TestResult Perform(BitArray data)
        {
            return Perform(data, Parameters.TestsToPerform);
        }

        public TestResult Perform(byte[] data, TestsEnum testsToPerform)
        {
            return Perform(new BitArray(data), testsToPerform);
        }

        public void ClearHistory()
        {
            lock (testHistory)
            {
                testHistory.Clear();
            }
        }

        public TestResult Perform(BitArray data, TestsEnum testsToPerform)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Dictionary<TestsEnum, SingleTestResult> results = new Dictionary<TestsEnum, SingleTestResult>();

            List<Action> actions = new List<Action>();

            if (testsToPerform.HasFlag(TestsEnum.DiscreteFourierTransform))
            {
                actions.Add(() =>
                {
                    RawTestResult rawTestResult = DiscreteFourierTransform(data);
                    lock (testHistory)
                    {
                        lock (results)
                        {
                            results.Add(TestsEnum.DiscreteFourierTransform, new SingleTestResult(rawTestResult, Parameters, TestsEnum.DiscreteFourierTransform, testHistory.Where(x => x.ContainsKey(TestsEnum.DiscreteFourierTransform)).Select(x => x[TestsEnum.DiscreteFourierTransform]).ToArray()));
                        }
                    }
                });
            }

            if (testsToPerform.HasFlag(TestsEnum.NonOverlappingTemplateMatchings))
            {
                actions.Add(() =>
                {
                    RawTestResult rawTestResult;
                    if (Parameters.NonOverlappingTemplateMatchings.Templates != null && Parameters.NonOverlappingTemplateMatchings.Templates.Length > 0)
                    {
                        rawTestResult = NonOverlappingTemplateMatchings(data, Parameters.NonOverlappingTemplateMatchings.Templates, Parameters.NonOverlappingTemplateMatchings.BlockLength);
                    }
                    else if (Parameters.NonOverlappingTemplateMatchings.Template != null)
                    {
                        rawTestResult = NonOverlappingTemplateMatchings(data, Parameters.NonOverlappingTemplateMatchings.Template, Parameters.NonOverlappingTemplateMatchings.BlockLength);
                    }
                    else
                    {
                        rawTestResult = NonOverlappingTemplateMatchings(data, Parameters.NonOverlappingTemplateMatchings.TemplateLength, Parameters.NonOverlappingTemplateMatchings.BlockLength);
                    }

                    lock (testHistory)
                    {
                        lock (results)
                        {
                            results.Add(TestsEnum.NonOverlappingTemplateMatchings, new SingleTestResult(rawTestResult, Parameters, TestsEnum.NonOverlappingTemplateMatchings, testHistory.Where(x => x.ContainsKey(TestsEnum.NonOverlappingTemplateMatchings)).Select(x => x[TestsEnum.NonOverlappingTemplateMatchings]).ToArray()));
                        }
                    }
                });
            }

            if (testsToPerform.HasFlag(TestsEnum.LinearComplexity))
            {
                actions.Add(() =>
                {
                    RawTestResult rawTestResult = LinearComplexity(data, Parameters.LinearComplexity.BlockLength, Parameters.LinearComplexity.DegreesOfFreedom);

                    lock (testHistory)
                    {
                        lock (results)
                        {
                            results.Add(TestsEnum.LinearComplexity, new SingleTestResult(rawTestResult, Parameters, TestsEnum.LinearComplexity, testHistory.Where(x => x.ContainsKey(TestsEnum.LinearComplexity)).Select(x => x[TestsEnum.LinearComplexity]).ToArray()));
                        }
                    }
                });
            }

            if ((testsToPerform & ~TestsEnum.DiscreteFourierTransform & ~TestsEnum.NonOverlappingTemplateMatchings & ~TestsEnum.LinearComplexity) != TestsEnum.None)
            {
                actions.Add(() =>
                {
                    RawTestResult rawTestResult;
                    foreach (TestsEnum test in (testsToPerform & ~TestsEnum.DiscreteFourierTransform & ~TestsEnum.NonOverlappingTemplateMatchings
                        & ~TestsEnum.LinearComplexity).EnumerateFlags())
                    {
                        switch (test)
                        {
                            case TestsEnum.Frequency:
                                rawTestResult = Frequency(data);
                                break;
                            case TestsEnum.BlockFrequency:
                                rawTestResult = BlockFrequency(data, Parameters.BlockFrequency.BlockLength);
                                break;
                            case TestsEnum.Runs:
                                rawTestResult = Runs(data);
                                break;
                            case TestsEnum.LongestRunOfOnes:
                                rawTestResult = LongestRunOfOnes(data);
                                break;
                            case TestsEnum.BinaryMatrixRank:
                                rawTestResult = BinaryMatrixRank(data, Parameters.BinaryMatrixRank.MatrixSize);
                                break;
                            case TestsEnum.OverlappingTemplateMatchings:
                                if (Parameters.OverlappingTemplateMatchings.Templates != null && Parameters.OverlappingTemplateMatchings.Templates.Length > 0)
                                {
                                    rawTestResult = OverlappingTemplateMatchings(data, Parameters.OverlappingTemplateMatchings.Templates, Parameters.OverlappingTemplateMatchings.DegreesOfFreedom, Parameters.OverlappingTemplateMatchings.BlockLength);
                                }
                                else if (Parameters.OverlappingTemplateMatchings.Template != null)
                                {
                                    rawTestResult = OverlappingTemplateMatchings(data, Parameters.OverlappingTemplateMatchings.Template, Parameters.OverlappingTemplateMatchings.DegreesOfFreedom, Parameters.OverlappingTemplateMatchings.BlockLength);
                                }
                                else
                                {
                                    rawTestResult = OverlappingTemplateMatchings(data, Parameters.OverlappingTemplateMatchings.TemplateLength, Parameters.OverlappingTemplateMatchings.DegreesOfFreedom, Parameters.OverlappingTemplateMatchings.BlockLength);
                                }
                                break;
                            case TestsEnum.MaurersUniversal:
                                rawTestResult = MaurersUniversal(data, Parameters.MaurersUniversal.BlockLength, Parameters.MaurersUniversal.InitializationBlocks);
                                break;
                            case TestsEnum.Serial:
                                rawTestResult = Serial(data, Parameters.Serial.BlockLength);
                                break;
                            case TestsEnum.ApproximateEntropy:
                                rawTestResult = ApproximateEntropy(data, Parameters.ApproximateEntropy.BlockLength);
                                break;
                            case TestsEnum.CumulativeSums:
                                rawTestResult = CumulativeSums(data);
                                break;
                            case TestsEnum.RandomExcursions:
                                rawTestResult = RandomExcursions(data);
                                break;
                            case TestsEnum.RandomExcursionsVariant:
                                rawTestResult = RandomExcursionsVariant(data);
                                break;
                            default:
                                throw new NotImplementedException("Unknow test");
                        }

                        lock (testHistory)
                        {
                            lock (results)
                            {
                                results.Add(test, new SingleTestResult(rawTestResult, Parameters, test, testHistory.Where(x => x.ContainsKey(test)).Select(x => x[test]).ToArray()));
                            }
                        }
                    }
                });
            }

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = MaximumThreads }, actions.ToArray());

            Dictionary<TestsEnum, double[]> pvalues = new Dictionary<TestsEnum, double[]>();

            foreach (TestsEnum test in testsToPerform.EnumerateFlags())
            {
                pvalues.Add(test, results[test].PValues);
            }

            lock (testHistory)
            {
                testHistory.Add(pvalues);
            }

            sw.Stop();

            return new TestResult(results, Parameters.AllowedFailedTestProportion, (int)sw.ElapsedMilliseconds);
        }

        public static class MinSequenceSizes
        {
            public const int Frequency = 100;
            public const int BlockFrequency = 100;
            public const int Runs = 100;
            public const int LongestRunOfOnes = 128;
            public const int BinaryMatrixRank = 152;
            public const int DiscreteFourierTransform = 1000;
            public const int NonOverlappingTemplateMatchings = 1000000; //Not specified in the paper. Assuming the same as in overlapping template matchings
            public const int OverlappingTemplateMatchings = 1000000;
            public const int MaurersUniversal = 387840;
            public const int LinearComplexity = 1000000;
            public const int Serial = 1000000;                          //Not specified, taken from the example
            public const int ApproximateEntropy = 100;                  //Not specified, taken from the example
            public const int CumulativeSums = 100;
            public const int RandomExcursions = 1000000;
            public const int RandomExcursionsVariant = 1000000;
        }
    }
}
