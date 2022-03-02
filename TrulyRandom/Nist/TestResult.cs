using System.Collections.Generic;
using System.Linq;

namespace TrulyRandom
{
    public partial class NistTests
    {
        public class TestResult
        {
            public int Time { get; private set; } = 0;
            public bool Success => (1 - SuccessfulTestProportion) <= proportionOfTestsAllowedToFail;

            public Dictionary<TestsEnum, SingleTestResult> FailedTests => TestResults.Where(x => !x.Value.Success).ToDictionary(dict => dict.Key, dict => dict.Value);

            public bool InsufficientCycles => FailedTests.Count > 0 && FailedTests.Values.All(x => x.Result == TestResultEnum.IncufficientCycles);

            public double SuccessfulTestProportion
            {
                get
                {
                    if (TestResults.Count == 0)
                    {
                        return 0;
                    }

                    return TestResults.Values.Where(x => x.Success).Count() / (double)TestResults.Count;
                }
            }

            public int ActuallyTestedBits => TestResults.Values.Min(x => x.ActuallyTestedBits);
            double proportionOfTestsAllowedToFail;

            public Dictionary<TestsEnum, SingleTestResult> TestResults { get; private set; } = new Dictionary<TestsEnum, SingleTestResult>();
            public string Report { get; private set; } = "";

            internal TestResult(Dictionary<TestsEnum, SingleTestResult> testResults, double proportionOfTestsAllowedToFail, int time)
            {
                TestResults = testResults;
                this.proportionOfTestsAllowedToFail = proportionOfTestsAllowedToFail;
                Time = time;

                Report = $"{TestResults.Values.Where(x => x.Success).Count()} of {TestResults.Count} tests passed\nTime: {Time}ms\nResult: {(Success ? "SUCCESS" : "FAILURE")}\n\n" + string.Join("\n\n", TestResults.Values.Select(x => x.Report));
            }
        }
    }
}
