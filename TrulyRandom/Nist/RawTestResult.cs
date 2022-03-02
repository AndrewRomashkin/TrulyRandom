namespace TrulyRandom
{
    public partial class NistTests
    {
        public struct RawTestResult
        {
            public double[] PValues;
            public int ActuallyTestedBits;
            public string Report;
            public int Time;

            public RawTestResult(double[] pValues, int actuallyTestedBits, string report, int time)
            {
                PValues = pValues;
                ActuallyTestedBits = actuallyTestedBits;
                Report = report;
                Time = time;
            }
        }
    }
}
