using System;
using System.Linq;

namespace TrulyRandom
{
    public partial class NistTests
    {
        public enum TestResultEnum { Success, SuccessWithFailedSubtests, BadPValues, IncufficientCycles, BadHistory }
        public enum SubtestResultEnum { Success, SuccessByDefault, SuccessByHistory, Failure }

        public class SingleTestResult
        {
            public bool Success { get; private set; } = false;
            public double SuccessfulSubtestProportion { get; private set; } = 0;
            public SubtestResultEnum[] SubtestResults { get; private set; } = new SubtestResultEnum[0];
            public double[] PValues { get; private set; } = new double[0];
            public double?[] HistoricalSuccessRate { get; private set; } = new double?[0];
            public int ActuallyTestedBits { get; private set; } = 0;
            public string Report { get; private set; } = "";
            public TestResultEnum Result { get; private set; } = TestResultEnum.BadHistory;
            public int Time { get; private set; } = 0;

            internal SingleTestResult(RawTestResult rawTestResult, TestParameters parameters, TestsEnum test, double[][] testHistory)
            {
                Time = rawTestResult.Time;
                PValues = rawTestResult.PValues;
                ActuallyTestedBits = rawTestResult.ActuallyTestedBits;

                SuccessfulSubtestProportion = rawTestResult.PValues.Where(x => x >= parameters.SignificanceLevel).Count() / (double)rawTestResult.PValues.Length;
                SubtestResults = new SubtestResultEnum[rawTestResult.PValues.Length];

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
                        SubtestResults[i] = SubtestResultEnum.Success;
                    }
                    else if (parameters.LongTermEvaluation.PassByDefault && testHistory.Length < parameters.LongTermEvaluation.MinPreviousTestResultsToCheck)
                    {
                        SubtestResults[i] = SubtestResultEnum.SuccessByDefault;
                    }
                    else if (HistoricalSuccessRate[i].HasValue && HistoricalSuccessRate[i].Value >= 1 - parameters.LongTermEvaluation.AllowedSinglePValueFailureRate)
                    {
                        SubtestResults[i] = SubtestResultEnum.SuccessByHistory;
                    }
                    else
                    {
                        SubtestResults[i] = SubtestResultEnum.Failure;
                    }
                }

                Success = (1 - SuccessfulSubtestProportion) <= parameters.AllowedFailedSubtestProportion && !SubtestResults.Contains(SubtestResultEnum.Failure);

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

                    switch (Result)
                    {
                        case TestResultEnum.Success:
                            Report += $"SUCCESS";
                            break;
                        case TestResultEnum.BadPValues:
                            Report += $"FAILURE (bad p-value)";
                            break;
                        case TestResultEnum.BadHistory:
                            Report += $"FAILURE (bad history)";
                            break;
                        default:
                            Report += $"FAILURE (unknown reason)";
                            break;
                    }
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

                    switch (Result)
                    {
                        case TestResultEnum.Success:
                            Report += $"SUCCESS";
                            break;
                        case TestResultEnum.SuccessWithFailedSubtests:
                            Report += $"SUCCESS (some subtests failed)";
                            break;
                        case TestResultEnum.BadPValues:
                            Report += $"FAILURE (too much bad p-values)";
                            break;
                        case TestResultEnum.BadHistory:
                            Report += $"FAILURE (bad history for at least one of p-values)";
                            break;
                        case TestResultEnum.IncufficientCycles:
                            Report += $"FAILURE (incufficient cycles)";
                            break;
                        default:
                            Report += $"FAILURE (unknown reason)";
                            break;
                    }
                }
            }
        }
    }
}
