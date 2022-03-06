using System.Collections.Generic;
using System.Linq;

namespace TrulyRandom
{
    public partial class NistTests
    {
        /// <summary>
        /// Result of a series of tests, past all neccesary evaluations
        /// </summary>
        public class FullTestResult
        {
            /// <summary>
            /// Total duration of the testing
            /// </summary>
            public int Time { get; private set; } = 0;
            /// <summary>
            /// Final decision on the tested sequence.<br/>
            /// To be considered random, proportion of failed tests is sufficiently small (&lt;= <see cref="TestParameters.AllowedFailedTestProportion"/>)
            /// </summary>
            public bool Success { get; private set; }
            /// <summary>
            /// Tests considered failed
            /// </summary>
            public Dictionary<TestsEnum, TestResult> FailedTests => TestResults.Where(x => !x.Value.Success).ToDictionary(dict => dict.Key, dict => dict.Value);
            /// <summary>
            /// Test had failed only due to insufficient number of cycles (tests <see cref="TestsEnum.RandomExcursions"/> and <see cref="TestsEnum.RandomExcursions"/>).
            /// Try to increase sequence size to reduce the number of such failures
            /// </summary>
            public bool InsufficientCycles => FailedTests.Count > 0 && FailedTests.Values.All(x => x.Result == TestResultEnum.IncufficientCycles);
            /// <summary>
            /// Proportion of tests considered successful
            /// </summary>
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
            /// <summary>
            /// Number of bits which was actually tested by all conducted tests. Some tests discard some of the data if it is not enough to form a whole block
            /// </summary>
            public int ActuallyTestedBits => TestResults.Values.Min(x => x.ActuallyTestedBits);
            /// <summary>
            /// Results of each one of the conducted tests
            /// </summary>
            public Dictionary<TestsEnum, TestResult> TestResults { get; private set; } = new Dictionary<TestsEnum, TestResult>();
            /// <summary>
            /// Human-readable report about the test
            /// </summary>
            public string Report { get; private set; } = "";

            internal FullTestResult(Dictionary<TestsEnum, TestResult> testResults, double proportionOfTestsAllowedToFail, int time)
            {
                TestResults = testResults;
                Success = (1 - SuccessfulTestProportion) <= proportionOfTestsAllowedToFail;
                Time = time;

                Report = $"{TestResults.Values.Where(x => x.Success).Count()} of {TestResults.Count} tests passed\nTime: {Time}ms\nResult: {(Success ? "SUCCESS" : "FAILURE")}\n\n" + string.Join("\n\n", TestResults.Values.Select(x => x.Report));
            }
        }
    }
}
