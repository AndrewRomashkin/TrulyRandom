using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TrulyRandom
{
    public partial class NistTests
    {
        public class TestParameters
        {
            public TestsEnum TestsToPerform { get; set; } = TestsEnum.All;

            double significanceLevel = 0.01;
            public double SignificanceLevel
            {
                get => significanceLevel;
                set
                {
                    if (value <= 0 || value >= 1)
                    {
                        throw new ArgumentOutOfRangeException("Value must be between 0 and 1");
                    }
                    significanceLevel = value;
                }
            }

            double allowedFailedTestProportion = 0;
            public double AllowedFailedTestProportion
            {
                get => allowedFailedTestProportion;
                set
                {
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentOutOfRangeException("Value must be between 0 and 1");
                    }
                    allowedFailedTestProportion = value;
                }
            }

            double allowedFailedSubtestProportion = 0.2;
            public double AllowedFailedSubtestProportion
            {
                get => allowedFailedSubtestProportion;
                set
                {
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentOutOfRangeException("Value must be between 0 and 1");
                    }
                    allowedFailedSubtestProportion = value;
                }
            }

            public class LongTermEvaluationParams
            {
                public bool PassByDefault { get; set; } = true;
                public double AllowedSinglePValueFailureRate { get; set; } = 0.1;
                public int PreviousTestResultsToCheck { get; set; } = 100;
                public int MinPreviousTestResultsToCheck { get; set; } = 10;
            }

            public class BlockFrequencyParams
            {
                public int BlockLength { get; set; } = -1;
            }

            public class BinaryMatrixRankParams
            {
                public int MatrixSize { get; set; } = -1;
            }

            public class NonOverlappingTemplateMatchingsParams
            {
                public BitArray Template { get; set; } = null;
                public BitArray[] Templates { get; set; } = null;
                public int TemplateLength { get; set; } = -1;
                public int BlockLength { get; set; } = -1;
            }

            public class OverlappingTemplateMatchingsParams
            {
                public BitArray Template { get; set; } = null;
                public BitArray[] Templates { get; set; } = null;
                public int TemplateLength { get; set; } = -1;
                public int DegreesOfFreedom { get; set; } = -1;
                public int BlockLength { get; set; } = -1;
            }

            public class MaurersUniversalParams
            {
                public int BlockLength { get; set; } = -1;
                public int InitializationBlocks { get; set; } = -1;
            }

            public class LinearComplexityParams
            {
                public int BlockLength { get; set; } = -1;
                public int DegreesOfFreedom { get; set; } = -1;
            }

            public class SerialParams
            {
                public int BlockLength { get; set; } = -1;
            }

            public class ApproximateEntropyParams
            {
                public int BlockLength { get; set; } = -1;
            }

            public BlockFrequencyParams BlockFrequency { get; set; } = new BlockFrequencyParams();
            public BinaryMatrixRankParams BinaryMatrixRank { get; set; } = new BinaryMatrixRankParams();
            public NonOverlappingTemplateMatchingsParams NonOverlappingTemplateMatchings { get; set; } = new NonOverlappingTemplateMatchingsParams();
            public OverlappingTemplateMatchingsParams OverlappingTemplateMatchings { get; set; } = new OverlappingTemplateMatchingsParams();
            public MaurersUniversalParams MaurersUniversal { get; set; } = new MaurersUniversalParams();
            public LinearComplexityParams LinearComplexity { get; set; } = new LinearComplexityParams();
            public SerialParams Serial { get; set; } = new SerialParams();
            public ApproximateEntropyParams ApproximateEntropy { get; set; } = new ApproximateEntropyParams();

            public LongTermEvaluationParams LongTermEvaluation { get; set; } = new LongTermEvaluationParams();

            public TestParameters()
            {
            }

            public TestParameters(int size)
            {
                TestsToPerform = TestsForSize(size);
            }

            public int MinSequenceSize => GetMinSequenceSize(TestsToPerform);

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
                switch (test)
                {
                    case 0:
                        return 0;
                    case TestsEnum.Frequency:
                        return MinSequenceSizes.Frequency;
                    case TestsEnum.BlockFrequency:
                        return MinSequenceSizes.BlockFrequency;
                    case TestsEnum.Runs:
                        return MinSequenceSizes.Runs;
                    case TestsEnum.LongestRunOfOnes:
                        return MinSequenceSizes.LongestRunOfOnes;
                    case TestsEnum.BinaryMatrixRank:
                        return MinSequenceSizes.BinaryMatrixRank;
                    case TestsEnum.DiscreteFourierTransform:
                        return MinSequenceSizes.DiscreteFourierTransform;
                    case TestsEnum.NonOverlappingTemplateMatchings:
                        return MinSequenceSizes.NonOverlappingTemplateMatchings;
                    case TestsEnum.OverlappingTemplateMatchings:
                        return MinSequenceSizes.OverlappingTemplateMatchings;
                    case TestsEnum.MaurersUniversal:
                        return MinSequenceSizes.MaurersUniversal;
                    case TestsEnum.LinearComplexity:
                        return MinSequenceSizes.LinearComplexity;
                    case TestsEnum.Serial:
                        return MinSequenceSizes.Serial;
                    case TestsEnum.ApproximateEntropy:
                        return MinSequenceSizes.ApproximateEntropy;
                    case TestsEnum.CumulativeSums:
                        return MinSequenceSizes.CumulativeSums;
                    case TestsEnum.RandomExcursions:
                        return MinSequenceSizes.RandomExcursions;
                    case TestsEnum.RandomExcursionsVariant:
                        return MinSequenceSizes.RandomExcursionsVariant;
                    case TestsEnum.All:
                        return TestsEnum.All.EnumerateFlags().Max(x => MinSequenceSizeForTest(x));
                    default:
                        throw new ArgumentException("Unknown test");
                }
            }

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
