namespace TrulyRandom;

public partial class NistTests
{
    /// <summary>
    /// Reasult of a single test, not evaluated yet.
    /// </summary>
    public struct SingleTestResult
    {
        /// <summary>
        /// P-values for the subtests. P-value is a measure of how random the sequence is according to this test or subtest.
        /// </summary>
        public readonly double[] PValues;
        /// <summary>
        /// Number of bits which was actually tested. Some tests discard some of the data if it is not enough to form a whole block.
        /// </summary>
        public readonly int ActuallyTestedBits;
        /// <summary>
        /// Human-readable report about the test.
        /// </summary>
        public readonly string Report;
        /// <summary>
        /// Duration of the testing.
        /// </summary>
        public readonly int Time;

        internal SingleTestResult(double[] pValues, int actuallyTestedBits, string report, int time)
        {
            PValues = pValues;
            ActuallyTestedBits = actuallyTestedBits;
            Report = report;
            Time = time;
        }
    }
}
