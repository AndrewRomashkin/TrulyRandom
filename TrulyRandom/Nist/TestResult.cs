using System;
using System.Linq;

namespace TrulyRandom
{
    public partial class NistTests
    {
        /// <summary>
        /// Specifies a detailed result of a test
        /// </summary>
        public enum TestResultEnum
        {
            /// <summary>
            /// Test had succeeded
            /// </summary>
            Success,
            /// <summary>
            /// Test had succeeded, but some subtests have failed
            /// </summary>
            SuccessWithFailedSubtests,
            /// <summary>
            /// Test had failed due to insufficient number of good p-values. Their history was not considered
            /// </summary>
            BadPValues,
            /// <summary>
            /// Test had failed due to insufficient number of cycles.
            /// Try to increase sequence size to reduce the number of such failures
            /// </summary>
            IncufficientCycles,
            /// <summary>
            /// Test had failed due at least one of the failed subtest having a bad history, even though failed test proportion was good enough 
            /// </summary>
            BadHistory
        }
        /// <summary>
        /// Specifies a result of a subtest long-term evaluation
        /// </summary>
        public enum LongTermEvaluationResultEnum
        {
            /// <summary>
            /// Subtest had succeeded, so history check was not performed
            /// </summary>
            Success,
            /// <summary>
            /// Result history isn't long enough and PassByDefault option is enabled
            /// </summary>
            SuccessByDefault,
            /// <summary>
            /// Subtest history is good enough
            /// </summary>
            SuccessByHistory,
            /// <summary>
            /// Subtest had failed and its history is bad
            /// </summary>
            Failure
        }

        /// <summary>
        /// Evaluated result of a single test 
        /// </summary>
        public class TestResult
        {
            /// <summary>
            /// Shows if the test was successful
            /// </summary>
            public bool Success { get; private set; } = false;
            /// <summary>
            /// Proportion of subtests with p-value &gt; <see cref="TestParameters.SignificanceLevel"/>
            /// </summary>
            public double SuccessfulSubtestProportion { get; private set; } = 0;
            /// <summary>
            /// Results of a long-term evaluation of each of the subtest results
            /// </summary>
            public LongTermEvaluationResultEnum[] SubtestLongTermEvaluationResults { get; private set; } = Array.Empty<LongTermEvaluationResultEnum>();
            /// <summary>
            /// P-values for the each of the subtests
            /// </summary>
            public double[] PValues { get; private set; } = Array.Empty<double>();
            /// <summary>
            /// P-values for the each of the subtests
            /// </summary>
            public double?[] HistoricalSuccessRate { get; private set; } = Array.Empty<double?>();
            /// <summary>
            /// Number of bits which was actually tested by the test. Some tests discard some of the data if it is not enough to form a whole block
            /// </summary>
            public int ActuallyTestedBits { get; private set; } = 0;
            /// <summary>
            /// Human-readable report about the test
            /// </summary>
            public string Report { get; private set; } = "";
            /// <summary>
            /// Detailed result of a test
            /// </summary>
            public TestResultEnum Result { get; private set; } = TestResultEnum.BadHistory;
            /// <summary>
            /// Duration of the testing
            /// </summary>
            public int Time { get; private set; } = 0;

            internal TestResult(SingleTestResult rawTestResult, TestParameters parameters, TestsEnum test, double[][] testHistory)
            {
                Time = rawTestResult.Time;
                PValues = rawTestResult.PValues;
                ActuallyTestedBits = rawTestResult.ActuallyTestedBits;

                SuccessfulSubtestProportion = rawTestResult.PValues.Where(x => x >= parameters.SignificanceLevel).Count() / (double)rawTestResult.PValues.Length;
                SubtestLongTermEvaluationResults = new LongTermEvaluationResultEnum[rawTestResult.PValues.Length];

                testHistory = testHistory.Where(x => x.Length == rawTestResult.PValues.Length).TakeLast(parameters.LongTermEvaluation.PreviousTestResultsToCheck).ToArray();

                HistoricalSuccessRate = new double?[rawTestResult.PValues.Length];

                for (int i = 0; i < rawTestResult.PValues.Length; i++)
                {
                    if (testHistory.Length >= parameters.LongTermEvaluation.MinPreviousTestResultsToCheck)
                    {
                        HistoricalSuccessRate[i] = (testHistory.TakeLast(parameters.LongTermEvaluation.PreviousTestResultsToCheck).Where(x => x[i] >= parameters.SignificanceLevel).Count() + (rawTestResult.PValues[i] >= parameters.SignificanceLevel ? 1 : 0)) /
                        ((double)testHistory.Length + 1);
                    }

                    if (rawTestResult.PValues[i] >= parameters.SignificanceLevel)
                    {
                        SubtestLongTermEvaluationResults[i] = LongTermEvaluationResultEnum.Success;
                    }
                    else if (parameters.LongTermEvaluation.PassByDefault && testHistory.Length < parameters.LongTermEvaluation.MinPreviousTestResultsToCheck)
                    {
                        SubtestLongTermEvaluationResults[i] = LongTermEvaluationResultEnum.SuccessByDefault;
                    }
                    else if (HistoricalSuccessRate[i].HasValue && HistoricalSuccessRate[i].Value >= 1 - parameters.LongTermEvaluation.AllowedSinglePValueFailureRate)
                    {
                        SubtestLongTermEvaluationResults[i] = LongTermEvaluationResultEnum.SuccessByHistory;
                    }
                    else
                    {
                        SubtestLongTermEvaluationResults[i] = LongTermEvaluationResultEnum.Failure;
                    }
                }

                Success = (1 - SuccessfulSubtestProportion) <= parameters.AllowedFailedSubtestProportion && !SubtestLongTermEvaluationResults.Contains(LongTermEvaluationResultEnum.Failure);

                if (Success && SuccessfulSubtestProportion == 1)
                {
                    Result = TestResultEnum.Success;
                }
                else if (Success && SuccessfulSubtestProportion != 1 && (1 - SuccessfulSubtestProportion) <= parameters.AllowedFailedSubtestProportion)
                {
                    Result = TestResultEnum.SuccessWithFailedSubtests;
                }
                else if (!Success && (1 - SuccessfulSubtestProportion) > parameters.AllowedFailedSubtestProportion)
                {
                    Result = TestResultEnum.BadPValues;
                }
                else if (!Success && (test == TestsEnum.RandomExcursions || test == TestsEnum.RandomExcursions) && rawTestResult.PValues.All(x => x == 0))
                {
                    Result = TestResultEnum.IncufficientCycles;
                }
                else
                {
                    Result = TestResultEnum.BadHistory;
                }

                Report = rawTestResult.Report;

                if (rawTestResult.PValues.Length == 1)
                {
                    Report += "\n\nHistory: ";
                    if (HistoricalSuccessRate[0] == null)
                    {
                        Report += $"History is too short ({testHistory.Length} < {parameters.LongTermEvaluation.MinPreviousTestResultsToCheck})";
                    }
                    else if (HistoricalSuccessRate[0].Value >= 1 - parameters.LongTermEvaluation.AllowedSinglePValueFailureRate)
                    {
                        Report += $"Passed ({HistoricalSuccessRate[0].Value:F2} >= {1 - parameters.LongTermEvaluation.AllowedSinglePValueFailureRate:F2})";
                    }
                    else
                    {
                        Report += $"Failed ({HistoricalSuccessRate[0].Value:F2} < {1 - parameters.LongTermEvaluation.AllowedSinglePValueFailureRate:F2})";
                    }
                    Report += "\n\nResult: ";

                    Report += Result switch
                    {
                        TestResultEnum.Success => $"SUCCESS",
                        TestResultEnum.BadPValues => $"FAILURE (bad p-value)",
                        TestResultEnum.BadHistory => $"FAILURE (bad history)",
                        _ => $"FAILURE (unknown reason)",
                    };
                }
                else
                {
                    Report += "\n\nHistory:\n";
                    for (int i = 0; i < rawTestResult.PValues.Length; i++)
                    {
                        if (HistoricalSuccessRate[i] == null)
                        {
                            Report += $"History is too short ({testHistory.Length} < {parameters.LongTermEvaluation.MinPreviousTestResultsToCheck})\n";
                        }
                        else if (HistoricalSuccessRate[i].Value >= 1 - parameters.LongTermEvaluation.AllowedSinglePValueFailureRate)
                        {
                            Report += $"Passed ({HistoricalSuccessRate[i].Value:F2} >= {1 - parameters.LongTermEvaluation.AllowedSinglePValueFailureRate:F2})\n";
                        }
                        else
                        {
                            Report += $"Failed ({HistoricalSuccessRate[i].Value:F2} < {1 - parameters.LongTermEvaluation.AllowedSinglePValueFailureRate:F2})\n";
                        }
                    }

                    Report += "\nResult: ";

                    Report += Result switch
                    {
                        TestResultEnum.Success => $"SUCCESS",
                        TestResultEnum.SuccessWithFailedSubtests => $"SUCCESS (some subtests failed)",
                        TestResultEnum.BadPValues => $"FAILURE (too much bad p-values)",
                        TestResultEnum.BadHistory => $"FAILURE (bad history for at least one of p-values)",
                        TestResultEnum.IncufficientCycles => $"FAILURE (incufficient cycles)",
                        _ => $"FAILURE (unknown reason)",
                    };
                }
            }
        }
    }
}
