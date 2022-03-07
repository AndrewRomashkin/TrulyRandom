using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TrulyRandom
{
    /// <summary>
    /// Provides methods for testing of random number sequences according to NIST SP 800-22, as well as a number of capabilities 
    /// for an extended evaluation, statistics, reporting, parameter selection etc.
    /// </summary>
    public partial class NistTests
    {
        /// <summary>
        /// Maximum amount of threads this instance allowed to create
        /// </summary>
        // TODO: count total threads
        public static int MaximumThreads { get; set; } = Environment.ProcessorCount;
        /// <summary>
        /// Test parameters
        /// </summary>
        public TestParameters Parameters { get; protected set; } = new TestParameters();

        /// <summary>
        /// Initializes a new instance of the <see cref="NistTests" /> class
        /// </summary>
        public NistTests()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NistTests" /> class with specified parameters
        /// </summary>
        public NistTests(TestParameters parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NistTests" /> class with test selection for the specified sequence size
        /// It is possible to test longer sequences, but test results for shorter ones will be nonrepresentative
        /// </summary>
        /// <param name="sequenceSize">Minimum sequence size</param>
        /// <exception cref="ArgumentException">Thrown if sequence size is too small to run even a single test</exception>
        public NistTests(int sequenceSize)
        {
            Parameters = new TestParameters(sequenceSize);
            if (Parameters.TestsToPerform == 0)
            {
                throw new ArgumentException("Sequence size isn't sufficient for any of the tests");
            }
        }

        /// <summary>
        /// Specifies a NIST SP 800-22 test or a set of tests
        /// </summary>
        [Flags]
        public enum TestsEnum
        {
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="Frequency"]'/>
            Frequency = 0x0001,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="BlockFrequency"]'/>
            BlockFrequency = 0x0002,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="Runs"]'/>
            Runs = 0x0004,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="LongestRunOfOnes"]'/>
            LongestRunOfOnes = 0x0008,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="BinaryMatrixRank"]'/>
            BinaryMatrixRank = 0x0010,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="DiscreteFourierTransform"]'/>
            DiscreteFourierTransform = 0x0020,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="NonOverlappingTemplateMatchings"]'/>
            NonOverlappingTemplateMatchings = 0x0040,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="OverlappingTemplateMatchings"]'/>
            OverlappingTemplateMatchings = 0x0080,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="MaurersUniversal"]'/>
            MaurersUniversal = 0x0100,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="LinearComplexity"]'/>
            LinearComplexity = 0x0200,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="Serial"]'/>
            Serial = 0x0400,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="ApproximateEntropy"]'/>
            ApproximateEntropy = 0x0800,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="CumulativeSums"]'/>
            CumulativeSums = 0x1000,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="RandomExcursions"]'/>
            RandomExcursions = 0x2000,
            /// <include file='NistDocumentation.xml' path='NistDocs/Func[@name="RandomExcursionsVariant"]'/>
            RandomExcursionsVariant = 0x4000,
            /// <summary>
            /// All available tests
            /// </summary>
            All = 0x7FFF,
            /// <summary>
            /// None of the tests
            /// </summary>
            None = 0
        }

        /// <summary>
        /// Recent test results
        /// </summary>
        readonly List<Dictionary<TestsEnum, double[]>> testHistory = new();

        /// <summary>
        /// Performs the tests on the specified data
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <returns>A <see cref="FullTestResult"/> object containing information abouth the results of aech test and an overlall conclusion</returns>
        public FullTestResult Perform(byte[] data)
        {
            return Perform(new BitArray(data), Parameters.TestsToPerform);
        }

        /// <summary>
        /// Performs the tests on the specified data
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <returns>A <see cref="FullTestResult"/> object containing information abouth the results of aech test and an overlall conclusion</returns>
        public FullTestResult Perform(BitArray data)
        {
            return Perform(data, Parameters.TestsToPerform);
        }

        /// <summary>
        /// Performs the specified tests on the specified data
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="testsToPerform">Tests to perform</param>
        /// <returns>A <see cref="FullTestResult"/> object containing information abouth the results of aech test and an overlall conclusion</returns>
        public FullTestResult Perform(byte[] data, TestsEnum testsToPerform)
        {
            return Perform(new BitArray(data), testsToPerform);
        }

        /// <summary>
        /// Performs the specified tests on the specified data
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="testsToPerform">Tests to perform</param>
        /// <returns>A <see cref="FullTestResult"/> object containing information abouth the results of aech test and an overlall conclusion</returns>
        public FullTestResult Perform(BitArray data, TestsEnum testsToPerform)
        {
            Stopwatch sw = new();
            sw.Start();
            Dictionary<TestsEnum, TestResult> results = new();

            List<Action> actions = new();

            //DFT, non overlapping template matchings and linear complexity tests are the longest to compute, so these are executed in sepatate threads
            if (testsToPerform.HasFlag(TestsEnum.DiscreteFourierTransform))
            {
                actions.Add(() =>
                {
                    SingleTestResult rawTestResult = DiscreteFourierTransform(data);
                    lock (testHistory)
                    {
                        lock (results)
                        {
                            results.Add(TestsEnum.DiscreteFourierTransform, new TestResult(rawTestResult, Parameters, TestsEnum.DiscreteFourierTransform, testHistory.Where(x => x.ContainsKey(TestsEnum.DiscreteFourierTransform)).Select(x => x[TestsEnum.DiscreteFourierTransform]).ToArray()));
                        }
                    }
                });
            }

            if (testsToPerform.HasFlag(TestsEnum.NonOverlappingTemplateMatchings))
            {
                actions.Add(() =>
                {
                    SingleTestResult rawTestResult;
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
                            results.Add(TestsEnum.NonOverlappingTemplateMatchings, new TestResult(rawTestResult, Parameters, TestsEnum.NonOverlappingTemplateMatchings, testHistory.Where(x => x.ContainsKey(TestsEnum.NonOverlappingTemplateMatchings)).Select(x => x[TestsEnum.NonOverlappingTemplateMatchings]).ToArray()));
                        }
                    }
                });
            }

            if (testsToPerform.HasFlag(TestsEnum.LinearComplexity))
            {
                actions.Add(() =>
                {
                    SingleTestResult rawTestResult = LinearComplexity(data, Parameters.LinearComplexity.BlockLength, Parameters.LinearComplexity.DegreesOfFreedom);

                    lock (testHistory)
                    {
                        lock (results)
                        {
                            results.Add(TestsEnum.LinearComplexity, new TestResult(rawTestResult, Parameters, TestsEnum.LinearComplexity, testHistory.Where(x => x.ContainsKey(TestsEnum.LinearComplexity)).Select(x => x[TestsEnum.LinearComplexity]).ToArray()));
                        }
                    }
                });
            }

            if ((testsToPerform & ~TestsEnum.DiscreteFourierTransform & ~TestsEnum.NonOverlappingTemplateMatchings & ~TestsEnum.LinearComplexity) != TestsEnum.None)
            {
                actions.Add(() =>
                {
                    SingleTestResult rawTestResult;
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
                                results.Add(test, new TestResult(rawTestResult, Parameters, test, testHistory.Where(x => x.ContainsKey(test)).Select(x => x[test]).ToArray()));
                            }
                        }
                    }
                });
            }

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = MaximumThreads }, actions.ToArray());

            Dictionary<TestsEnum, double[]> pvalues = new();

            foreach (TestsEnum test in testsToPerform.EnumerateFlags())
            {
                pvalues.Add(test, results[test].PValues);
            }

            lock (testHistory)
            {
                testHistory.Add(pvalues);
            }

            sw.Stop();

            return new FullTestResult(results, Parameters.AllowedFailedTestProportion, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Clears the test result history
        /// </summary>
        public void ClearHistory()
        {
            lock (testHistory)
            {
                testHistory.Clear();
            }
        }

        /// <summary>
        /// Minimum sequence sizes for the corresponding tests. It is still allowed to test shorter sequences, but results will be nonrepresentative
        /// </summary>
        public static class MinSequenceSizes
        {
            /// <summary>
            /// Minimum sequence size for the frequency test
            /// </summary>
            public const int Frequency = 100;
            /// <summary>
            /// Minimum sequence size for the block frequency test
            /// </summary>
            public const int BlockFrequency = 100;
            /// <summary>
            /// Minimum sequence size for the runs test
            /// </summary>
            public const int Runs = 100;
            /// <summary>
            /// Minimum sequence size for the longest run of ones test
            /// </summary>
            public const int LongestRunOfOnes = 128;
            /// <summary>
            /// Minimum sequence size for the binary matrix rank test
            /// </summary>
            public const int BinaryMatrixRank = 152;
            /// <summary>
            /// Minimum sequence size for the discrete Fourier transform test
            /// </summary>
            public const int DiscreteFourierTransform = 1_000;
            /// <summary>
            /// Minimum sequence size for the non-overlapping template matchings test
            /// </summary>
            //Not specified in the paper. Assuming the same as in overlapping template matchings
            public const int NonOverlappingTemplateMatchings = 1_000_000;
            /// <summary>
            /// Minimum sequence size for the overlapping template matchings test
            /// </summary>
            public const int OverlappingTemplateMatchings = 1_000_000;
            /// <summary>
            /// Minimum sequence size for the Maurer's universal test
            /// </summary>
            public const int MaurersUniversal = 387_840;
            /// <summary>
            /// Minimum sequence size for the linear complexity test
            /// </summary>
            public const int LinearComplexity = 1_000_000;
            /// <summary>
            /// Minimum sequence size for the serial test
            /// </summary>
            //Not specified, taken from the example
            public const int Serial = 1_000_000;
            /// <summary>
            /// Minimum sequence size for the approximate entropy test
            /// </summary>
            //Not specified, taken from the example
            public const int ApproximateEntropy = 100;
            /// <summary>
            /// Minimum sequence size for the cumulative sums test
            /// </summary>
            public const int CumulativeSums = 100;
            /// <summary>
            /// Minimum sequence size for the random excursions test<br/>
            /// Note that for this test it is recommended to keep block size as big as possible to avoid getting an <see cref="TestResultEnum.IncufficientCycles"/> resut
            /// </summary>
            public const int RandomExcursions = 1_000_000;
            /// <summary>
            /// Minimum sequence size for the random excursions variant test<br/>
            /// Note that for this test it is recommended to keep block size as big as possible to avoid getting an <see cref="TestResultEnum.IncufficientCycles"/> resut
            /// </summary>
            public const int RandomExcursionsVariant = 1_000_000;
        }
    }
}
