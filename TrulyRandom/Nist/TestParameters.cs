using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TrulyRandom
{
    public partial class NistTests
    {
        /// <summary>
        /// Parameters for the NIST SP 800-22 tests
        /// </summary>
        public class TestParameters
        {
            /// <summary>
            /// Determines what tests should be performed by the <see cref="Perform(byte[])"/> method
            /// </summary>
            public TestsEnum TestsToPerform { get; set; } = TestsEnum.All;

            double significanceLevel = 0.01;
            /// <summary>
            /// Determines a threshold, which p-value should exceed to be considered successful
            /// </summary>
            public double SignificanceLevel
            {
                get => significanceLevel;
                set
                {
                    if (value <= 0 || value >= 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1");
                    }
                    significanceLevel = value;
                }
            }

            double allowedFailedTestProportion = 0;
            /// <summary>
            /// Determines the maximum proportion of failed tests for sequence to be considered random
            /// </summary>
            public double AllowedFailedTestProportion
            {
                get => allowedFailedTestProportion;
                set
                {
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1");
                    }
                    allowedFailedTestProportion = value;
                }
            }

            double allowedFailedSubtestProportion = 0.2;
            /// <summary>
            /// Determines the maximum proportion of failed subtests for the test to be considered successful
            /// </summary>
            public double AllowedFailedSubtestProportion
            {
                get => allowedFailedSubtestProportion;
                set
                {
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1");
                    }
                    allowedFailedSubtestProportion = value;
                }
            }

            /// <summary>
            /// Parameters of a long-term evaluation routine. It stores and checks a history of last test results to ensure no subtest fails too often: it can
            /// be an indication of a dependency within the data and therefore a generator flaw<br/>
            /// For more info, see section 2.14.8 of the paper
            /// </summary>
            public class LongTermEvaluationParams
            {
                /// <summary>
                /// Determines whether proportion of the succeeded tests should be considered good 
                /// if not enough data is accumulated ( &lt; <see cref="MinPreviousTestResultsToCheck"/> ).<br/>
                /// Disabling this parameter will likely cause first <see cref="MinPreviousTestResultsToCheck"/> sequences to be considered non-random,
                /// however is can increase overall security of the solution
                /// </summary>
                public bool PassByDefault { get; set; } = true;
                /// <summary>
                /// Allowed rate of failure of a single p-value within the last <see cref="PreviousTestResultsToCheck"/>
                /// </summary>
                public double AllowedSinglePValueFailureRate { get; set; } = 0.1;
                /// <summary>
                /// Maximum amount of last test results to be tested
                /// </summary>
                public int PreviousTestResultsToCheck { get; set; } = 100;
                /// <summary>
                /// Maximum amount of last test results to be tested. If the number of results is insufficient then behaviour 
                /// is determined by the <see cref="PassByDefault"/> option
                /// </summary>
                public int MinPreviousTestResultsToCheck { get; set; } = 10;
            }

            /// <summary>
            /// Parameters of the block frequency test
            /// </summary>
            public class BlockFrequencyParams
            {
                /// <summary>
                /// Length of a single block (-1 for autoselection)
                /// </summary>
                public int BlockLength { get; set; } = -1;
            }

            /// <summary>
            /// Parameters of the binary matrix rank test
            /// </summary>
            public class BinaryMatrixRankParams
            {
                /// <summary>
                /// Size of a single matrix (-1 for autoselection). Matrices are square, so they contain matrixSize^2 elements
                /// </summary>
                public int MatrixSize { get; set; } = -1;
            }

            /// <summary>
            /// Parameters of the non-overlapping template matchings test
            /// </summary>
            public class NonOverlappingTemplateMatchingsParams
            {
                /// <summary>
                /// Template to be used for testing (null to disable this parameter).<br/> 
                /// If one of the following parameters (in order of priority):<br/> 
                /// 1. <see cref="Templates"/><br/> 
                /// 2. <see cref="Template"/><br/> 
                /// 3. <see cref="TemplateLength"/><br/> 
                /// is specified, it will be used. If none - <see cref="TemplateLength"/> will be autoselected.
                /// </summary>
                public BitArray Template { get; set; } = null;
                /// <summary>
                /// An array of templates to be used for testing (null to disable this parameter).<br/>
                /// If one of the following parameters (in order of priority):<br/> 
                /// 1. <see cref="Templates"/><br/> 
                /// 2. <see cref="Template"/><br/> 
                /// 3. <see cref="TemplateLength"/><br/> 
                /// is specified, it will be used. If none - <see cref="TemplateLength"/> will be autoselected.
                /// </summary>
                public BitArray[] Templates { get; set; } = null;
                /// <summary>
                /// All possible templates of this length will be generated and used for testing (-1 for autoselection).<br/>
                /// If one of the following parameters (in order of priority):<br/> 
                /// 1. <see cref="Templates"/><br/> 
                /// 2. <see cref="Template"/><br/> 
                /// 3. <see cref="TemplateLength"/><br/> 
                /// is specified, it will be used. If none - <see cref="TemplateLength"/> will be autoselected.
                /// </summary>
                public int TemplateLength { get; set; } = -1;
                /// <summary>
                /// Length of a single block (-1 for autoselection)
                /// </summary>
                public int BlockLength { get; set; } = -1;
            }

            /// <summary>
            /// Parameters of the overlapping template matchings test
            /// </summary>
            public class OverlappingTemplateMatchingsParams
            {
                /// <summary>
                /// Template to be used for testing (null to disable this parameter).<br/> 
                /// If one of the following parameters (in order of priority):<br/> 
                /// 1. <see cref="Templates"/><br/> 
                /// 2. <see cref="Template"/><br/> 
                /// 3. <see cref="TemplateLength"/><br/> 
                /// is specified, it will be used. If none - <see cref="TemplateLength"/> will be autoselected.
                /// </summary>
                public BitArray Template { get; set; } = null;
                /// <summary>
                /// An array of templates to be used for testing (null to disable this parameter).<br/>
                /// If one of the following parameters (in order of priority):<br/> 
                /// 1. <see cref="Templates"/><br/> 
                /// 2. <see cref="Template"/><br/> 
                /// 3. <see cref="TemplateLength"/><br/> 
                /// is specified, it will be used. If none - <see cref="TemplateLength"/> will be autoselected.
                /// </summary>
                public BitArray[] Templates { get; set; } = null;
                /// <summary>
                /// A template of all ones of this length will be generated and used for testing (-1 for autoselection).<br/>
                /// If one of the following parameters (in order of priority):<br/> 
                /// 1. <see cref="Templates"/><br/> 
                /// 2. <see cref="Template"/><br/> 
                /// 3. <see cref="TemplateLength"/><br/> 
                /// is specified, it will be used. If none - <see cref="TemplateLength"/> will be autoselected.
                /// </summary>
                public int TemplateLength { get; set; } = -1;
                /// <summary>
                /// Degrees of freedom of the chi-squared distribution (-1 for autoselection)
                /// </summary>
                public int DegreesOfFreedom { get; set; } = -1;
                /// <summary>
                /// Length of a single block (-1 for autoselection)
                /// </summary>
                public int BlockLength { get; set; } = -1;
            }

            /// <summary>
            /// Parameters of the Maurer's universal test
            /// </summary>
            public class MaurersUniversalParams
            {
                /// <summary>
                /// Length of a single block (-1 for autoselection)
                /// </summary>
                public int BlockLength { get; set; } = -1;
                /// <summary>
                /// A number of blocks used to initialize the table (-1 for autoselection)
                /// </summary>
                public int InitializationBlocks { get; set; } = -1;
            }

            /// <summary>
            /// Parameters of the linear complexity test
            /// </summary>
            public class LinearComplexityParams
            {
                /// <summary>
                /// Length of a single block (-1 for autoselection)
                /// </summary>
                public int BlockLength { get; set; } = -1;
                /// <summary>
                /// Degrees of freedom of the chi-squared distribution (-1 for autoselection)
                /// </summary>
                public int DegreesOfFreedom { get; set; } = -1;
            }

            /// <summary>
            /// Parameters of the serial test
            /// </summary>
            public class SerialParams
            {
                /// <summary>
                /// Length of a single block (-1 for autoselection)
                /// </summary>
                public int BlockLength { get; set; } = -1;
            }

            /// <summary>
            /// Parameters of the approximate entropy test
            /// </summary>
            public class ApproximateEntropyParams
            {
                /// <summary>
                /// Length of a single block (-1 for autoselection)
                /// </summary>
                public int BlockLength { get; set; } = -1;
            }

            /// <summary>
            /// Parameters of the block frequency test
            /// </summary>
            public BlockFrequencyParams BlockFrequency { get; set; } = new BlockFrequencyParams();
            /// <summary>
            /// Parameters of the binary matrix rank test
            /// </summary>
            public BinaryMatrixRankParams BinaryMatrixRank { get; set; } = new BinaryMatrixRankParams();
            /// <summary>
            /// Parameters of the non-overlapping template matchings test
            /// </summary>
            public NonOverlappingTemplateMatchingsParams NonOverlappingTemplateMatchings { get; set; } = new NonOverlappingTemplateMatchingsParams();
            /// <summary>
            /// Parameters of the overlapping template matchings test
            /// </summary>
            public OverlappingTemplateMatchingsParams OverlappingTemplateMatchings { get; set; } = new OverlappingTemplateMatchingsParams();
            /// <summary>
            /// Parameters of the Maurer's universal test
            /// </summary>
            public MaurersUniversalParams MaurersUniversal { get; set; } = new MaurersUniversalParams();
            /// <summary>
            /// Parameters of the linear complexity test
            /// </summary>
            public LinearComplexityParams LinearComplexity { get; set; } = new LinearComplexityParams();
            /// <summary>
            /// Parameters of the serial test
            /// </summary>
            public SerialParams Serial { get; set; } = new SerialParams();
            /// <summary>
            /// Parameters of the approximate entropy test
            /// </summary>
            public ApproximateEntropyParams ApproximateEntropy { get; set; } = new ApproximateEntropyParams();

            /// <summary>
            /// Parameters of a long-term evaluation routine. It stores and checks a history of last test results to ensure no subtest fails too often: it can
            /// be an indication of a dependency within the data and therefore a generator flaw<br/>
            /// For more info, see section 2.14.8 of the paper
            /// </summary>
            public LongTermEvaluationParams LongTermEvaluation { get; set; } = new LongTermEvaluationParams();

            /// <summary>
            /// Initializes a new instance of the <see cref="TestParameters" /> class
            /// </summary>
            public TestParameters()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="TestParameters" /> class with test selection for the specified sequence size
            /// It is possible to test longer sequences, but test results for shorter ones will be nonrepresentative
            /// </summary>
            /// <param name="size">Minimum sequence size</param>
            public TestParameters(int size)
            {
                TestsToPerform = TestsForSize(size);
            }
            /// <summary>
            /// Minimum sequnce size required for the selected tests. It is still allowed to test shorter sequences, but results will be nonrepresentative
            /// </summary>
            public int MinSequenceSize => GetMinSequenceSize(TestsToPerform);

            /// <summary>
            /// Minimum sequnce size required for the selected tests. It is still allowed to test shorter sequences, but results will be nonrepresentative
            /// </summary>
            public static int GetMinSequenceSize(TestsEnum tests)
            {
                IEnumerable<TestsEnum> testFlags = tests.EnumerateFlags();

                if (!testFlags.Any())
                {
                    throw new ArgumentException("Tests shouldn't be empty");
                }

                return testFlags.Min(x => MinSequenceSizeForTest(x));
            }

            static int MinSequenceSizeForTest(TestsEnum test)
            {
                return test switch
                {
                    0 => 0,
                    TestsEnum.Frequency => MinSequenceSizes.Frequency,
                    TestsEnum.BlockFrequency => MinSequenceSizes.BlockFrequency,
                    TestsEnum.Runs => MinSequenceSizes.Runs,
                    TestsEnum.LongestRunOfOnes => MinSequenceSizes.LongestRunOfOnes,
                    TestsEnum.BinaryMatrixRank => MinSequenceSizes.BinaryMatrixRank,
                    TestsEnum.DiscreteFourierTransform => MinSequenceSizes.DiscreteFourierTransform,
                    TestsEnum.NonOverlappingTemplateMatchings => MinSequenceSizes.NonOverlappingTemplateMatchings,
                    TestsEnum.OverlappingTemplateMatchings => MinSequenceSizes.OverlappingTemplateMatchings,
                    TestsEnum.MaurersUniversal => MinSequenceSizes.MaurersUniversal,
                    TestsEnum.LinearComplexity => MinSequenceSizes.LinearComplexity,
                    TestsEnum.Serial => MinSequenceSizes.Serial,
                    TestsEnum.ApproximateEntropy => MinSequenceSizes.ApproximateEntropy,
                    TestsEnum.CumulativeSums => MinSequenceSizes.CumulativeSums,
                    TestsEnum.RandomExcursions => MinSequenceSizes.RandomExcursions,
                    TestsEnum.RandomExcursionsVariant => MinSequenceSizes.RandomExcursionsVariant,
                    TestsEnum.All => TestsEnum.All.EnumerateFlags().Max(x => MinSequenceSizeForTest(x)),
                    _ => throw new ArgumentException("Unknown test"),
                };
            }

            /// <summary>
            /// Selects tests for which sequence size is sufficient
            /// </summary>
            /// <param name="size">Minimum sequence size</param>
            /// <returns></returns>
            public static TestsEnum TestsForSize(int size)
            {
                TestsEnum result = 0;

                foreach (TestsEnum test in Enum.GetValues(typeof(TestsEnum)))
                {
                    if (MinSequenceSizeForTest(test) <= size)
                    {
                        result |= test;
                    }
                }

                return result;
            }
        }
    }
}
