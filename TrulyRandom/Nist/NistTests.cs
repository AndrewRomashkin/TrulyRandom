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
            /// <summary>
            /// The focus of the test is the proportion of zeroes and ones for the entire sequence.
            /// The purpose of this test is to determine whether the number of ones and zeros in a sequence are approximately
            /// the same as wouldbe expected for a truly random sequence. The test assesses the closeness of the fraction of
            /// ones to ½, that is, the number of ones and zeroes in a sequence should be about the same.<br/>
            /// For more info, see section 2.1 of the paper
            /// </summary>
            Frequency = 0x0001,
            /// <summary>
            /// The focus of the test is the proportion of ones within M-bit blocks.
            /// The purpose of this test is to determine whether the frequency of ones in an M-bit block is approximately M/2, 
            /// as would be expected under an assumption of randomness.<br/>
            /// For more info, see section 2.2 of the paper
            /// </summary>
            BlockFrequency = 0x0002,
            /// <summary>
            /// The focus of this test is the total number of runs in the sequence, where a run is an uninterrupted sequence
            /// of identical bits. A run of length k consists of exactly k identical bits and is bounded before and after with
            /// a bit of the opposite value. The purpose of the runs test is to determine whether the number of runs of
            /// ones and zeros of various lengths is as expected for a random sequence. In particular, this test determines
            /// whether the oscillation between such zeros and ones is too fast or too slow.<br/>
            /// For more info, see section 2.3 of the paper
            /// </summary>
            Runs = 0x0004,
            /// <summary>
            /// The focus of the test is the longest run of ones within M-bit blocks. The purpose of this test is to
            /// determine whether the length of the longest run of ones within the tested sequence is consistent with the
            /// length of the longest run of ones that would be expected in a random sequence.<br/>
            /// For more info, see section 2.4 of the paper
            /// </summary>
            LongestRunOfOnes = 0x0008,
            /// <summary>
            /// The focus of the test is the rank of disjoint sub-matrices of the entire sequence. The purpose of this test is
            /// to check for linear dependence among fixed length substrings of the original sequence.<br/>
            /// For more info, see section 2.5 of the paper
            /// </summary>
            BinaryMatrixRank = 0x0010,
            /// <summary>
            /// The focus of this test is the peak heights in the Discrete Fourier Transform of the sequence. The purpose
            /// of this test is to detect periodic features (i.e., repetitive patterns that are near each other) in the tested
            /// sequence that would indicate a deviation from the assumption of randomness.<br/>
            /// For more info, see section 2.6 of the paper
            /// </summary>
            DiscreteFourierTransform = 0x0020,
            /// <summary>
            /// The focus of this test is the number of occurrences of pre-specified target strings. The purpose of this
            /// test is to detect generators that produce too many occurrences of a given non-periodic (aperiodic) pattern.
            /// For this test and for the Overlapping Template Matching test of Section 2.8, an m-bit window is used to
            /// search for a specific m-bit pattern. If the pattern is not found, the window slides one bit position. If the
            /// pattern is found, the window is reset to the bit after the found pattern, and the search resumes.<br/>
            /// For more info, see section 2.7 of the paper
            /// </summary>
            NonOverlappingTemplateMatchings = 0x0040,
            /// <summary>
            /// The focus of the Overlapping Template Matching test is the number of occurrences of pre-specified target
            /// strings. Both this test and the Non-overlapping Template Matching use an m-bit window to search 
            /// for a specific m-bit pattern. As with the Non-overlapping Template Matching test, if the pattern is not found,
            /// the window slides one bit position. The difference between this test and the test in Section 2.7 is that
            /// when the pattern is found, the window slides only one bit before resuming the search.<br/>
            /// For more info, see section 2.8 of the paper
            /// </summary>
            OverlappingTemplateMatchings = 0x0080,
            /// <summary>
            /// The focus of this test is the number of bits between matching patterns (a measure that is related to the
            /// length of a compressed sequence). The purpose of the test is to detect whether or not the sequence can be
            /// significantly compressed without loss of information. A significantly compressible sequence is
            /// considered to be non-random.<br/>
            /// For more info, see section 2.9 of the paper
            /// </summary>
            MaurersUniversal = 0x0100,
            /// <summary>
            /// The focus of this test is the length of a linear feedback shift register (LFSR). The purpose of this test is to
            /// determine whether or not the sequence is complex enough to be considered random. Random sequences
            /// are characterized by longer LFSRs. An LFSR that is too short implies non-randomness.<br/>
            /// For more info, see section 2.10 of the paper
            /// </summary>
            LinearComplexity = 0x0200,
            /// <summary>
            /// The focus of this test is the frequency of all possible overlapping m-bit patterns across the entire
            /// sequence. The purpose of this test is to determine whether the number of occurrences of the 2m m-bit
            /// overlapping patterns is approximately the same as would be expected for a random sequence. Random
            /// sequences have uniformity; that is, every m-bit pattern has the same chance of appearing as every other
            /// m-bit pattern.<br/>
            /// For more info, see section 2.11 of the paper
            /// </summary>
            Serial = 0x0400,
            /// <summary>
            /// As with the Serial test, the focus of this test is the frequency of all possible overlapping
            /// m-bit patterns across the entire sequence. The purpose of the test is to compare the frequency of
            /// overlapping blocks of two consecutive/adjacent lengths (m and m+1) against the expected result for a
            /// random sequence.<br/>
            /// For more info, see section 2.12 of the paper
            /// </summary>
            ApproximateEntropy = 0x0800,
            /// <summary>
            /// The focus of this test is the maximal excursion (from zero) of the random walk defined by the cumulative
            /// sum of adjusted (-1, +1) digits in the sequence.The purpose of the test is to determine whether the
            /// cumulative sum of the partial sequences occurring in the tested sequence is too large or too small relative
            /// to the expected behavior of that cumulative sum for random sequences.For a random sequence, the excursions 
            /// of the random walk should be near zero.<br/>
            /// For more info, see section 2.13 of the paper
            /// </summary>
            CumulativeSums = 0x1000,
            /// <summary>
            /// The focus of this test is the number of cycles having exactly K visits in a cumulative sum random walk.
            /// The cumulative sum random walk is derived from partial sums after the(0,1) sequence is transferred to
            /// the appropriate(-1, +1) sequence.A cycle of a random walk consists of a sequence of steps of unit length
            /// taken at random that begin at and return to the origin.The purpose of this test is to determine if the
            /// number of visits to a particular state within a cycle deviates from what one would expect for a random
            /// sequence.This test is actually a series of eight tests (and conclusions), one test and conclusion for each of
            /// the states: -4, -3, -2, -1 and +1, +2, +3, +4. <br/>
            /// Note that for this test it is recommended to keep block size as big as possible to avoid getting an <see cref="TestResultEnum.IncufficientCycles"/> resut.<br/>
            /// For more info, see section 2.14 of the paper
            /// </summary>
            RandomExcursions = 0x2000,
            /// <summary>
            /// The focus of this test is the total number of times that a particular state is visited (i.e., occurs) in a
            /// cumulative sum random walk. The purpose of this test is to detect deviations from the expected number
            /// of visits to various states in the random walk. This test is actually a series of eighteen tests (and
            /// conclusions), one test and conclusion for each of the states: -9, -8, …, -1 and +1, +2, …, +9.<br/>
            /// Note that for this test it is recommended to keep block size as big as possible to avoid getting an <see cref="TestResultEnum.IncufficientCycles"/> resut.<br/>
            /// For more info, see section 2.15 of the paper
            /// </summary>
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
