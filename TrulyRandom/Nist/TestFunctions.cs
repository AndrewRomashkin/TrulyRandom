using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TrulyRandom
{
    //Source: NIST Statistical Testing Suite, NIST Special Publication 800-22
    public partial class NistTests
    {
        /// <summary>
        /// The focus of the test is the proportion of zeroes and ones for the entire sequence.
        /// The purpose of this test is to determine whether the number of ones and zeros in a sequence are approximately
        /// the same as wouldbe expected for a truly random sequence. The test assesses the closeness of the fraction of
        /// ones to ½, that is, the number of ones and zeroes in a sequence should be about the same.<br/>
        /// For more info, see section 2.1 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <returns>Test result</returns>
        public static SingleTestResult Frequency(BitArray data)
        {
            Stopwatch sw = new();
            sw.Start();
            int sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i] ? 1 : -1;
            }

            double s_obs = Math.Abs(sum) / Math.Sqrt(data.Length);
            double pValue = NistTestUtils.Erfc(s_obs / Math.Sqrt(2));

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"------------------------\n" +
                            $"Frequency (monobit) test\n" +
                            $"------------------------\n" +
                            $"N       = {data.Length}\n" +
                            $"Sum     = {sum}\n" +
                            $"Sum/n   = {(double)sum / data.Length}\n" +
                            $"P-value = {pValue:0.####}\n" +
                            $"Time    = {(int)sw.ElapsedMilliseconds} ms";
            return new SingleTestResult(new double[] { pValue }, data.Count, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of the test is the proportion of ones within M-bit blocks.
        /// The purpose of this test is to determine whether the frequency of ones in an M-bit block is approximately M/2, 
        /// as would be expected under an assumption of randomness.<br/>
        /// For more info, see section 2.2 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult BlockFrequency(BitArray data, int blockLength)
        {
            Stopwatch sw = new();
            sw.Start();
            if (blockLength < 2) //blockLength ≥ 20, blockLength > dataLength/100
            {
                blockLength = Math.Min(Math.Max(20, data.Length / 50), data.Length);
            }

            int blockCount = data.Length / blockLength;

            double sum = 0;
            for (int i = 0; i < blockCount; i++)
            {
                int blockSum = 0;
                for (int j = 0; j < blockLength; j++)
                {
                    if (data[j + i * blockLength])
                    {
                        blockSum++;
                    }
                }
                double pi = blockSum / (double)blockLength;
                sum += Math.Pow(pi - 0.5, 2);
            }
            double chi2 = 4.0 * blockLength * sum;
            double pValue = NistTestUtils.Igamc(blockCount / 2.0, chi2 / 2.0);

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"--------------------\n" +
                            $"Block frequency test\n" +
                            $"--------------------\n" +
                            $"N              = {data.Length}\n" +
                            $"Block size     = {blockLength}\n" +
                            $"Blocks         = {blockCount}\n" +
                            $"Chi^2          = {chi2:0.####}\n" +
                            $"Discarded bits = {data.Length % blockLength}\n" +
                            $"P-value        = {pValue:0.####}\n" +
                            $"Time           = {(int)sw.ElapsedMilliseconds} ms";
            return new SingleTestResult(new double[] { pValue }, blockLength * blockCount, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of this test is the total number of runs in the sequence, where a run is an uninterrupted sequence
        /// of identical bits. A run of length k consists of exactly k identical bits and is bounded before and after with
        /// a bit of the opposite value. The purpose of the runs test is to determine whether the number of runs of
        /// ones and zeros of various lengths is as expected for a random sequence. In particular, this test determines
        /// whether the oscillation between such zeros and ones is too fast or too slow.<br/>
        /// For more info, see section 2.3 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <returns>Test result</returns>
        public static SingleTestResult Runs(BitArray data)
        {
            Stopwatch sw = new();
            sw.Start();
            int sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i])
                {
                    sum++;
                }
            }

            double proportionOfOnes = sum / (double)data.Length;

            if (Math.Abs(proportionOfOnes - 0.5) > (2.0 / Math.Sqrt(data.Length)))
            {
                sw.Stop();
                return new SingleTestResult(new double[] { 0 }, data.Count,
                    $"---------\n" +
                    $"Runs test\n" +
                    $"---------\n" +
                    $"Error: Test should not have been run because of a failure to pass the Frequency (monobit) test\n" +
                    $"Pi={proportionOfOnes:0.####}", (int)sw.ElapsedMilliseconds);
            }

            int runs = 1;
            for (int i = 1; i < data.Length; i++)
            {
                if (data[i] != data[i - 1])
                {
                    runs++;
                }
            }

            double expected = 2.0 * data.Length * proportionOfOnes * (1 - proportionOfOnes);
            double pValue = NistTestUtils.Erfc(Math.Abs(runs - expected) / (2.0 * proportionOfOnes * (1 - proportionOfOnes) * Math.Sqrt(2 * data.Length)));
            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"---------\n" +
                            $"Runs test\n" +
                            $"---------\n" +
                            $"N             = {data.Length}\n" +
                            $"Pi            = {proportionOfOnes:0.####}\n" +
                            $"Runs          = {runs}\n" +
                            $"Expected runs = {expected:0.####}\n" +
                            $"P-value       = {pValue:0.####}\n" +
                            $"Time          = {(int)sw.ElapsedMilliseconds} ms";
            return new SingleTestResult(new double[] { pValue }, data.Count, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of the test is the longest run of ones within M-bit blocks. The purpose of this test is to
        /// determine whether the length of the longest run of ones within the tested sequence is consistent with the
        /// length of the longest run of ones that would be expected in a random sequence.<br/>
        /// For more info, see section 2.4 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <returns>Test result</returns>
        public static SingleTestResult LongestRunOfOnes(BitArray data)
        {
            Stopwatch sw = new();
            sw.Start();
            double[] probabilities = new double[7]; //pi in the paper
            int[] V = new int[7];
            uint[] occurences = new uint[7];//nu in the paper

            int K, blockLength;
            if (data.Length < 6272)
            {
                K = 3;
                blockLength = 8;
                V[0] = 1; V[1] = 2; V[2] = 3; V[3] = 4;
                probabilities[0] = 0.21484375;
                probabilities[1] = 0.3671875;
                probabilities[2] = 0.23046875;
                probabilities[3] = 0.1875;
            }
            else if (data.Length < 750000)
            {
                K = 5;
                blockLength = 128;
                V[0] = 4; V[1] = 5; V[2] = 6; V[3] = 7; V[4] = 8; V[5] = 9;
                probabilities[0] = 0.1174035788;
                probabilities[1] = 0.242955959;
                probabilities[2] = 0.249363483;
                probabilities[3] = 0.17517706;
                probabilities[4] = 0.102701071;
                probabilities[5] = 0.112398847;
            }
            else
            {
                K = 6;
                blockLength = 10000;
                V[0] = 10; V[1] = 11; V[2] = 12; V[3] = 13; V[4] = 14; V[5] = 15; V[6] = 16;
                probabilities[0] = 0.0882;
                probabilities[1] = 0.2092;
                probabilities[2] = 0.2483;
                probabilities[3] = 0.1933;
                probabilities[4] = 0.1208;
                probabilities[5] = 0.0675;
                probabilities[6] = 0.0727;
            }

            int blockCount = data.Length / blockLength;
            for (int i = 0; i < blockCount; i++)
            {
                int longestRun = 0;
                int run = 0;
                for (int j = 0; j < blockLength; j++)
                {
                    if (data[i * blockLength + j])
                    {
                        run++;
                        if (run > longestRun)
                        {
                            longestRun = run;
                        }
                    }
                    else
                    {
                        run = 0;
                    }
                }
                if (longestRun < V[0])
                {
                    occurences[0]++;
                }
                for (int j = 0; j <= K; j++)
                {
                    if (longestRun == V[j])
                    {
                        occurences[j]++;
                    }
                }
                if (longestRun > V[K])
                {
                    occurences[K]++;
                }
            }

            double chi2 = 0;
            for (int i = 0; i <= K; i++)
            {
                chi2 += (occurences[i] - blockCount * probabilities[i]) * (occurences[i] - blockCount * probabilities[i]) / (blockCount * probabilities[i]);
            }

            double pValue = NistTestUtils.Igamc((double)(K / 2.0), chi2 / 2.0);

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"------------------------\n" +
                            $"Longest run of ones test\n" +
                            $"------------------------\n" +
                            $"N              = {data.Length}\n" +
                            $"Block size     = {blockLength}\n" +
                            $"Blocks         = {blockCount}\n" +
                            $"Chi^2          = {chi2:0.####}\n" +
                            $"Discarded bits = {data.Length % blockLength}\n" +
                            $"P-value        = {pValue:0.####}\n" +
                            $"Time           = {(int)sw.ElapsedMilliseconds} ms\n\n" +
                            $"Probabilities:\n";

            report += Utils.FormatTable(new string[] { "Run length", "Expected", "Observed" }, new string[][]
                {   new string[] { $"<={V[0]}" }.Concat(Enumerable.Range(1, K-1).Select(i=>V[i].ToString())).Concat(new string[] { $">={V[K]}" }).ToArray(),
                    Enumerable.Range(0, K+1).Select(i=>(blockCount * probabilities[i]).ToString("0.####")).ToArray(),
                    Enumerable.Range(0, K+1).Select(i=>occurences[i].ToString("0.####")).ToArray()
                }, 8);
            return new SingleTestResult(new double[] { pValue }, blockCount * blockLength, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of the test is the rank of disjoint sub-matrices of the entire sequence. The purpose of this test is
        /// to check for linear dependence among fixed length substrings of the original sequence.<br/>
        /// For more info, see section 2.5 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="matrixSize">Size of a single matrix (-1 for autoselection). Matrices are square, so they contain matrixSize^2 elements</param>
        /// <returns>Test result</returns>
        public static SingleTestResult BinaryMatrixRank(BitArray data, int matrixSize)
        {
            Stopwatch sw = new();
            sw.Start();
            if (matrixSize < 3) // dataLength ≥ 38*matrixSize^2
            {
                matrixSize = Math.Min(50, Math.Max(1, (int)Math.Sqrt(data.Length / 38)));
            }

            bool[,] matrix = new bool[matrixSize, matrixSize];

            int matrixCount = data.Length / (matrixSize * matrixSize);
            if (matrixCount == 0)
            {
                sw.Stop();
                return new SingleTestResult(new double[] { 0 }, data.Count,
                            $"-----------------------\n" +
                            $"Binary matrix rank test\n" +
                            $"-----------------------\n" +
                            "Error: Insuffucient bits to define even one matrix", (int)sw.ElapsedMilliseconds);
            }

            // Compute probabilities
            double fullRankProbability = 1;
            for (int i = 0; i <= matrixSize - 1; i++)
            {
                fullRankProbability *= 1 - Math.Pow(2, i - matrixSize);
            }

            double fullRankMinus1Probability = 1;
            for (int i = 0; i <= matrixSize - 2; i++)
            {
                fullRankMinus1Probability *= Math.Pow(1 - Math.Pow(2, i - matrixSize), 2) / (1 - Math.Pow(2, i - matrixSize + 1));
            }
            fullRankMinus1Probability /= 2;

            double otherRankProbability = 1 - (fullRankProbability + fullRankMinus1Probability);

            int fullRankMatrices = 0;
            int fullRankMinus1Matrices = 0;
            DateTime lastBreak = DateTime.Now;
            for (int i = 0; i < matrixCount; i++)
            {
                NistTestUtils.DefineMatrix(data, matrixSize, matrixSize, ref matrix, i);
                int rank = NistTestUtils.ComputeRank(matrixSize, matrixSize, matrix);
                if (rank == matrixSize)
                {
                    fullRankMatrices++;
                }
                if (rank == matrixSize - 1)
                {
                    fullRankMinus1Matrices++;
                }
                Utils.BreakExecution(ref lastBreak);
            }
            int otherRankMatrices = matrixCount - (fullRankMatrices + fullRankMinus1Matrices);

            double chi2 = Math.Pow(fullRankMatrices - matrixCount * fullRankProbability, 2) / (double)(matrixCount * fullRankProbability) +
                          Math.Pow(fullRankMinus1Matrices - matrixCount * fullRankMinus1Probability, 2) / (double)(matrixCount * fullRankMinus1Probability) +
                          Math.Pow(otherRankMatrices - matrixCount * otherRankProbability, 2) / (double)(matrixCount * otherRankProbability);

            double pValue = Math.Exp(-chi2 / 2);

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"-----------------------\n" +
                            $"Binary matrix rank test\n" +
                            $"-----------------------\n" +
                            $"N              = {data.Length}\n" +
                            $"Matrix size    = {matrixSize}*{matrixSize}\n" +
                            $"Matrices       = {matrixCount}\n" +
                            $"Chi^2          = {chi2:0.####}\n" +
                            $"Discarded bits = {data.Length % (matrixSize * matrixSize)}\n" +
                            $"P-value        = {pValue:0.####}\n" +
                            $"Time           = {(int)sw.ElapsedMilliseconds} ms\n\n" +
                            $"Rank probabilities:\n" +
                            $"Rank      {matrixSize,-8}{matrixSize - 1,-8}<={matrixSize - 2}\n" +
                            $"Expected  {fullRankProbability,-8:0.####}{fullRankMinus1Probability,-8:0.####}{otherRankProbability:0.####}\n" +
                            $"Observed  {fullRankMatrices / (double)matrixCount,-8:0.####}{fullRankMinus1Matrices / (double)matrixCount,-8:0.####}{otherRankMatrices / (double)matrixCount:0.####}";

            return new SingleTestResult(new double[] { pValue }, matrixCount * matrixSize * matrixSize, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of this test is the peak heights in the Discrete Fourier Transform of the sequence. The purpose
        /// of this test is to detect periodic features (i.e., repetitive patterns that are near each other) in the tested
        /// sequence that would indicate a deviation from the assumption of randomness.<br/>
        /// For more info, see section 2.6 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <returns>Test result</returns>
        public static SingleTestResult DiscreteFourierTransform(BitArray data)
        {
            Stopwatch sw = new();
            sw.Start();
            System.Numerics.Complex[] samples = new System.Numerics.Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                samples[i] = new System.Numerics.Complex(data[i] ? 1 : -1, 0);
            }

            Nist.FFT.Apply(samples, MaximumThreads);

            double[] magnitudes = samples.Take(data.Length / 2).Select(x => x.Magnitude).ToArray();
            double upperBound = Math.Sqrt(2.995732274 * data.Length);
            double expectedPeaks = 0.95 * data.Length / 2;
            int observedPeaks = magnitudes.Take(data.Length / 2).Count(x => x < upperBound);         // number of peaks less than h = sqrt(log(1/0.05)*n)
            double d = (observedPeaks - expectedPeaks) / Math.Sqrt(data.Length / 4.0 * 0.95 * 0.05);
            double pValue = NistTestUtils.Erfc(Math.Abs(d) / Math.Sqrt(2.0));

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"-------------------------------\n" +
                            $"Discrete Fourier transform test\n" +
                            $"-------------------------------\n" +
                            $"N              = {data.Length}\n" +
                            $"Percentile     = {observedPeaks / (double)data.Length * 2 * 100}\n" +
                            $"Expected peaks = {expectedPeaks:0.####}\n" +
                            $"Observed peaks = {observedPeaks}\n" +
                            $"P-value        = {pValue:0.####}\n" +
                            $"Time           = {(int)sw.ElapsedMilliseconds} ms";
            return new SingleTestResult(new double[] { pValue }, data.Count, report, (int)sw.ElapsedMilliseconds);
        }

        #region Non-overlapping template matchings
        /// <summary>
        /// Generates all possible aperiodic templates of the given length
        /// </summary>
        /// <param name="length">Length of the templates</param>
        /// <returns>All possible aperiodic templates of the given length</returns>
        public static BitArray[] GenerateAperiodicTemplates(int length)
        {
            if (length < 2)
            {
                return Array.Empty<BitArray>();
            }

            List<BitArray> result = new();
            for (int i = 1; i < Math.Pow(2, length); i++)
            {
                BitArray template = new(new int[] { i });

                bool match = true;
                for (int j = 1; j < length; j++)
                {
                    match = true;
                    if ((template[length - 1] != template[0]) && ((template[length - 1] != template[0]) || (template[length - 2] != template[0])))
                    {
                        for (int k = 0; k <= length - 1 - j; k++)
                        {
                            if (template[k] != template[k + j])
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (match)
                    {
                        break;
                    }
                }
                if (!match)
                {
                    result.Add(new BitArray(template.Cast<bool>().Take(length).Reverse().ToArray()));
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// The focus of this test is the number of occurrences of pre-specified target strings. The purpose of this
        /// test is to detect generators that produce too many occurrences of a given non-periodic (aperiodic) pattern.
        /// For this test and for the Overlapping Template Matching test of Section 2.8, an m-bit window is used to
        /// search for a specific m-bit pattern. If the pattern is not found, the window slides one bit position. If the
        /// pattern is found, the window is reset to the bit after the found pattern, and the search resumes.<br/>
        /// For more info, see section 2.7 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="templates">An array of templates to be used for testing (null for autoselection)</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult NonOverlappingTemplateMatchings(BitArray data, BitArray[] templates, int blockLength)
        {
            Stopwatch sw = new();
            sw.Start();
            if (templates != null)
            {
                templates = templates.Where(x => x.Length >= 2).ToArray();
            }
            if (templates == null || templates.Length == 0)
            {
                templates = GenerateAperiodicTemplates(9);
            }

            NonOverlappingTemplateMatchingsTestResult[] results = new NonOverlappingTemplateMatchingsTestResult[templates.Length];

            Utils.ParallelFor(0, templates.Length, 1, MaximumThreads, (u, v) =>
            {
                results[u] = NonOverlappingTemplateMatchingsInternal(data, templates[u], blockLength);
            });

            sw.Stop();
            string report;

            report = $"--------------------------------------\n" +
                     $"Non-overlapping template matching test\n" +
                     $"--------------------------------------\n";


            if (templates.Select(x => x.Length).Distinct().Count() == 1)
            {
                if (results[0].Mean <= 0)
                {
                    report += $"Error: mean value ({results[0].Mean:0.####}) <= 0";
                }
                else
                {
                    report += $"N               = {data.Length}\n" +
                              $"Block size      = {blockLength}\n" +
                              $"Blocks          = {data.Length / blockLength}\n" +
                              $"Template length = {templates[0].Length}\n" +
                              $"Templates       = {templates.Length}\n" +
                              $"Mean value      = {results[0].Mean:0.####}\n" +
                              $"Variance        = {results[0].Variance:0.####}\n" +
                              $"Discarded bits  = {data.Length % blockLength}\n" +
                              $"Time            = {(int)sw.ElapsedMilliseconds} ms\n\n";

                    for (int i = 0; i < templates.Length; i++)
                    {
                        report += $"Template   = {templates[i].ToBitString()}\n" +
                                  $"Chi^2      = {results[i].Chi2:0.####}\n" +
                                  $"P-value    = {results[i].PValue:0.####}";
                        if (i != templates.Length - 1)
                        {
                            report += "\n\n";
                        }
                    }
                }
            }
            else
            {
                report += $"N               = {data.Length}\n" +
                          $"Block size      = {blockLength}\n" +
                          $"Blocks          = {data.Length / blockLength}\n" +
                          $"Templates       = {templates.Length}\n" +
                          $"Discarded bits  = {data.Length % blockLength}\n" +
                          $"Time            = {(int)sw.ElapsedMilliseconds} ms\n\n";

                for (int i = 0; i < templates.Length; i++)
                {
                    report += $"Template        = {templates[i].ToBitString()}\n" +
                              $"Template length = {templates[0].Length}\n";
                    if (results[0].Mean <= 0)
                    {
                        report += $"Error: mean value ({results[0].Mean:0.####}) <= 0";
                    }
                    else
                    {
                        report += $"Mean value      = {results[i].Mean:0.####}\n" +
                                  $"Variance        = {results[i].Variance:0.####}\n" +
                                  $"Chi^2           = {results[i].Chi2:0.####}\n" +
                                  $"P-value         = {results[i].PValue:0.####}";
                    }
                    if (i != templates.Length - 1)
                    {
                        report += "\n\n";
                    }
                }
            }

            return new SingleTestResult(results.Select(x => x.PValue).ToArray(), data.Length / blockLength * blockLength, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of this test is the number of occurrences of pre-specified target strings. The purpose of this
        /// test is to detect generators that produce too many occurrences of a given non-periodic (aperiodic) pattern.
        /// For this test and for the Overlapping Template Matching test of Section 2.8, an m-bit window is used to
        /// search for a specific m-bit pattern. If the pattern is not found, the window slides one bit position. If the
        /// pattern is found, the window is reset to the bit after the found pattern, and the search resumes.<br/>
        /// For more info, see section 2.7 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="templateLength">Length of the templates to be used (-1 for autoselection)</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult NonOverlappingTemplateMatchings(BitArray data, int templateLength, int blockLength)
        {
            Stopwatch sw = new();
            sw.Start();
            if (templateLength < 2)
            {
                templateLength = 9;
            }

            if (blockLength < 2)
            {
                blockLength = data.Length / 100;
            }

            BitArray[] templates = GenerateAperiodicTemplates(templateLength);
            NonOverlappingTemplateMatchingsTestResult[] results = new NonOverlappingTemplateMatchingsTestResult[templates.Length];

            Utils.ParallelFor(0, templates.Length, 1, MaximumThreads, (u, v) =>
            {
                results[u] = NonOverlappingTemplateMatchingsInternal(data, templates[u], blockLength);
            });

            sw.Stop();
            string report;

            report = $"--------------------------------------\n" +
                     $"Non-overlapping template matching test\n" +
                     $"--------------------------------------\n";

            if (results[0].Mean <= 0)
            {
                report += $"Error: mean value ({results[0].Mean:0.####}) <= 0";
            }
            else
            {
                report += $"N               = {data.Length}\n" +
                          $"Block size      = {blockLength}\n" +
                          $"Blocks          = {data.Length / blockLength}\n" +
                          $"Template length = {templateLength}\n" +
                          $"Templates       = {templates.Length}\n" +
                          $"Mean value      = {results[0].Mean:0.####}\n" +
                          $"Variance        = {results[0].Variance:0.####}\n" +
                          $"Discarded bits  = {data.Length % blockLength}\n" +
                          $"Time            = {(int)sw.ElapsedMilliseconds} ms\n\n";

                for (int i = 0; i < templates.Length; i++)
                {
                    report += $"Template = {templates[i].ToBitString()}\n" +
                              $"Chi^2    = {results[i].Chi2:0.####}\n" +
                              $"P-value  = {results[i].PValue:0.####}";
                    if (i != templates.Length - 1)
                    {
                        report += "\n\n";
                    }
                }
            }

            return new SingleTestResult(results.Select(x => x.PValue).ToArray(), data.Length / blockLength * blockLength, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of this test is the number of occurrences of pre-specified target strings. The purpose of this
        /// test is to detect generators that produce too many occurrences of a given non-periodic (aperiodic) pattern.
        /// For this test and for the Overlapping Template Matching test of Section 2.8, an m-bit window is used to
        /// search for a specific m-bit pattern. If the pattern is not found, the window slides one bit position. If the
        /// pattern is found, the window is reset to the bit after the found pattern, and the search resumes.<br/>
        /// For more info, see section 2.7 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="template">Template to be used</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult NonOverlappingTemplateMatchings(BitArray data, BitArray template, int blockLength)
        {
            Stopwatch sw = new();
            sw.Start();
            if (blockLength < 2)
            {
                blockLength = data.Length / 100;
            }

            if (template.Length < 2)
            {
                throw new ArgumentException("Template must have size >= 2");
            }

            NonOverlappingTemplateMatchingsTestResult testResult = NonOverlappingTemplateMatchingsInternal(data, template, blockLength);

            sw.Stop();
            string report;

            report = $"--------------------------------------\n" +
                     $"Non-overlapping template matching test\n" +
                     $"--------------------------------------\n";

            if (testResult.Mean <= 0)
            {
                report += $"Error: mean value ({testResult.Mean:0.####}) <= 0";
            }
            else
            {
                report = $"N               = {data.Length}\n" +
                         $"Template        = {template.ToBitString()}\n" +
                         $"Template length = {template.Length}\n" +
                         $"Block size      = {blockLength}\n" +
                         $"Blocks          = {data.Length / blockLength}\n" +
                         $"Mean value      = {testResult.Mean:0.####}\n" +
                         $"Variance        = {testResult.Variance:0.####}\n" +
                         $"Chi^2           = {testResult.Chi2:0.####}\n" +
                         $"Discarded bits  = {data.Length % blockLength}\n" +
                         $"P-value         = {testResult.PValue:0.####}\n" +
                         $"Time            = {(int)sw.ElapsedMilliseconds} ms";
            }

            return new SingleTestResult(new double[] { testResult.PValue }, data.Length / blockLength * blockLength, report, (int)sw.ElapsedMilliseconds);
        }

        struct NonOverlappingTemplateMatchingsTestResult
        {
            public double Mean;
            public double Variance;
            public double Chi2;
            public double PValue;

            public NonOverlappingTemplateMatchingsTestResult(double mean, double variance, double chi2, double pValue)
            {
                Mean = mean;
                Variance = variance;
                Chi2 = chi2;
                PValue = pValue;
            }
        }

        static NonOverlappingTemplateMatchingsTestResult NonOverlappingTemplateMatchingsInternal(BitArray data, BitArray template, int blockLength)
        {
            int blockCount = data.Length / blockLength;
            int[] occurrences = new int[blockCount]; //Wj in the paper

            double mean = (blockLength - template.Length + 1) / Math.Pow(2, template.Length); //lambda in the paper
            if (mean <= 0)
            {
                return new NonOverlappingTemplateMatchingsTestResult(mean, 0, 0, 0);
            }
            double variance = blockLength * (1 / Math.Pow(2, template.Length) - (2 * template.Length - 1) / Math.Pow(2, 2 * template.Length));

            for (int i = 0; i < blockCount; i++)
            {
                for (int j = 0; j < blockLength - template.Count + 1; j++)
                {
                    bool match = true;
                    for (int k = 0; k < template.Count; k++)
                    {
                        if (template[k] != data[i * blockLength + j + k])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        occurrences[i]++;
                        j += template.Count - 1;
                    }
                }
            }

            double chi2 = 0;
            for (int i = 0; i < blockCount; i++)
            {
                chi2 += Math.Pow(occurrences[i] - mean, 2);
            }
            chi2 /= variance;

            double pValue = NistTestUtils.Igamc(blockCount / 2.0, chi2 / 2);

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            return new NonOverlappingTemplateMatchingsTestResult(mean, variance, chi2, pValue);
        }
        #endregion Non-overlapping template matchings

        #region Overlapping template matchings
        /// <summary>
        /// The focus of the Overlapping Template Matching test is the number of occurrences of pre-specified target
        /// strings. Both this test and the Non-overlapping Template Matching use an m-bit window to search 
        /// for a specific m-bit pattern. As with the Non-overlapping Template Matching test, if the pattern is not found,
        /// the window slides one bit position. The difference between this test and the test in Section 2.7 is that
        /// when the pattern is found, the window slides only one bit before resuming the search.<br/>
        /// For more info, see section 2.8 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="templateLength">Length of the template to be used (-1 for autoselection)</param>
        /// <param name="degreesOfFreedom">Degrees of freedom of the chi-squared distribution (-1 for autoselection)</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult OverlappingTemplateMatchings(BitArray data, int templateLength, int degreesOfFreedom, int blockLength)
        {
            if (templateLength < 2)
            {
                templateLength = 9;
            }
            return OverlappingTemplateMatchings(data, new BitArray(Enumerable.Repeat(true, templateLength).ToArray()), degreesOfFreedom, blockLength);
        }

        /// <summary>
        /// The focus of the Overlapping Template Matching test is the number of occurrences of pre-specified target
        /// strings. Both this test and the Non-overlapping Template Matching use an m-bit window to search 
        /// for a specific m-bit pattern. As with the Non-overlapping Template Matching test, if the pattern is not found,
        /// the window slides one bit position. The difference between this test and the test in Section 2.7 is that
        /// when the pattern is found, the window slides only one bit before resuming the search.<br/>
        /// For more info, see section 2.8 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="templates">Templates to be used (null for autoselection)</param>
        /// <param name="degreesOfFreedom">Degrees of freedom of the chi-squared distribution (-1 for autoselection)</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult OverlappingTemplateMatchings(BitArray data, BitArray[] templates, int degreesOfFreedom, int blockLength)
        {
            Stopwatch sw = new();
            sw.Start();
            if (templates != null)
            {
                templates = templates.Where(x => x.Length >= 2).ToArray();
            }
            if (templates == null || templates.Length == 0)
            {
                templates = new BitArray[] { new BitArray(Enumerable.Repeat(true, 9).ToArray()) };
            }

            if (degreesOfFreedom < 1)
            {
                degreesOfFreedom = 5;
            }

            OverlappingTemplateMatchingsTestResult[] results = new OverlappingTemplateMatchingsTestResult[templates.Length];
            int[] blockLengths = Enumerable.Repeat(blockLength, templates.Length).ToArray();

            Utils.ParallelFor(0, templates.Length, 1, MaximumThreads, (u, v) =>
            {
                if (blockLengths[u] < templates[u].Length)
                {
                    blockLengths[u] = (int)Math.Pow(2, templates[u].Length + 1) + templates[u].Length - 1;
                }
                results[u] = OverlappingTemplateMatchingsInternal(data, templates[u], degreesOfFreedom, blockLengths[u]);
            });

            sw.Stop();
            string report = $"----------------------------------\n" +
                            $"Overlapping template matching test\n" +
                            $"----------------------------------\n" +
                            $"N                  = {data.Length}\n" +
                            $"Degrees of freedom = {degreesOfFreedom}\n\n";

            for (int i = 0; i < templates.Length; i++)
            {
                int blockCount = data.Length / blockLengths[i];
                report += $"Template        = {templates[i].ToBitString()}\n" +
                          $"Template length = {templates[i].Length}\n" +
                          $"Block size      = {blockLength}\n" +
                          $"Blocks          = {blockCount}\n" +
                          $"Mean value      = {results[i].Mean}\n" +
                          $"Eta             = {results[i].Eta}\n" +
                          $"Chi^2           = {results[i].Chi2:0.####}\n" +
                          $"Discarded bits  = {data.Length % blockLength}\n" +
                          $"P-value         = {results[i].PValue:0.####}\n" +
                          $"Time            = {(int)sw.ElapsedMilliseconds} ms\n\n" +
                          $"Frequencies:\n";
                report += Utils.FormatTable(new string[] { "Occurences", "Expected", "Observed" }, new string[][]
                    {   Enumerable.Range(0, degreesOfFreedom+1).Select(j=>(j == degreesOfFreedom ? ">=" : "") + j).ToArray(),
                        Enumerable.Range(0, degreesOfFreedom+1).Select(j=>results[i].Probabilities[j].ToString("0.####")).ToArray(),
                        Enumerable.Range(0, degreesOfFreedom+1).Select(j=>(results[i].Occurrences[j] / (double)blockCount).ToString("0.####")).ToArray()
                    }, 8);

                if (i != templates.Length - 1)
                {
                    report += "\n\n";
                }
            }

            return new SingleTestResult(results.Select(x => x.PValue).ToArray(), Enumerable.Range(0, templates.Length).Select(i => data.Length / blockLengths[i] * blockLengths[i]).Min(), report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of the Overlapping Template Matching test is the number of occurrences of pre-specified target
        /// strings. Both this test and the Non-overlapping Template Matching use an m-bit window to search 
        /// for a specific m-bit pattern. As with the Non-overlapping Template Matching test, if the pattern is not found,
        /// the window slides one bit position. The difference between this test and the test in Section 2.7 is that
        /// when the pattern is found, the window slides only one bit before resuming the search.<br/>
        /// For more info, see section 2.8 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="template">Template to be used (-1 for autoselection)</param>
        /// <param name="degreesOfFreedom">Degrees of freedom of the chi-squared distribution (-1 for autoselection)</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult OverlappingTemplateMatchings(BitArray data, BitArray template, int degreesOfFreedom, int blockLength)
        {
            Stopwatch sw = new();
            sw.Start();
            if (template == null || template.Length < 2)
            {
                template = new BitArray(Enumerable.Repeat(true, 9).ToArray());
            }

            if (degreesOfFreedom < 1)
            {
                degreesOfFreedom = 5;
            }

            if (blockLength < template.Length)
            {
                blockLength = (int)Math.Pow(2, template.Length + 1) + template.Length - 1;
            }

            OverlappingTemplateMatchingsTestResult result = OverlappingTemplateMatchingsInternal(data, template, degreesOfFreedom, blockLength);

            sw.Stop();
            int blockCount = data.Length / blockLength;
            string report = $"----------------------------------\n" +
                            $"Overlapping template matching test\n" +
                            $"----------------------------------\n" +
                            $"N                  = {data.Length}\n" +
                            $"Template           = {template.ToBitString()}\n" +
                            $"Template length    = {template.Length}\n" +
                            $"Block size         = {blockLength}\n" +
                            $"Blocks             = {blockCount}\n" +
                            $"Degrees of freedom = {degreesOfFreedom}\n" +
                            $"Mean value         = {result.Mean}\n" +
                            $"Eta                = {result.Eta}\n" +
                            $"Chi^2              = {result.Chi2:0.####}\n" +
                            $"Discarded bits     = {data.Length % blockLength}\n" +
                            $"P-value            = {result.PValue:0.####}\n" +
                            $"Time               = {(int)sw.ElapsedMilliseconds} ms\n\n" +
                            $"Frequencies:\n";
            report += Utils.FormatTable(new string[] { "Occurences", "Expected", "Observed" }, new string[][]
                    {   Enumerable.Range(0, degreesOfFreedom+1).Select(i=>(i == degreesOfFreedom ? ">=" : "") + i).ToArray(),
                        Enumerable.Range(0, degreesOfFreedom+1).Select(i=>result.Probabilities[i].ToString("0.####")).ToArray(),
                        Enumerable.Range(0, degreesOfFreedom+1).Select(i=>(result.Occurrences[i] / (double)blockCount).ToString("0.####")).ToArray()
                    }, 8);

            return new SingleTestResult(new double[] { result.PValue }, data.Length / blockLength * blockLength, report, (int)sw.ElapsedMilliseconds);
        }

        struct OverlappingTemplateMatchingsTestResult
        {
            public double Mean;
            public double Eta;
            public uint[] Occurrences;
            public double[] Probabilities;
            public double Chi2;
            public double PValue;

            public OverlappingTemplateMatchingsTestResult(double mean, double eta, uint[] occurrences, double[] probabilities, double chi2, double pValue)
            {
                Mean = mean;
                Eta = eta;
                Occurrences = occurrences;
                Probabilities = probabilities;
                Chi2 = chi2;
                PValue = pValue;
            }
        }

        static OverlappingTemplateMatchingsTestResult OverlappingTemplateMatchingsInternal(BitArray data, BitArray template, int degreesOfFreedom, int blockLength)
        {
            uint[] occurrences = new uint[degreesOfFreedom + 1];
            double[] probabilities = new double[degreesOfFreedom + 1]; //pi in paper

            int blockCount = data.Length / blockLength;

            //Compute probabilities
            double mean = (blockLength - template.Length + 1) / Math.Pow(2, template.Length); //lambda in paper
            double eta = mean / 2;

            probabilities[0] = Math.Exp(-eta);
            for (int i = 1; i < degreesOfFreedom; i++)
            {
                for (int j = 1; j <= i; j++)
                {
                    probabilities[i] += Math.Exp(-eta - i * Math.Log(2) + j * Math.Log(eta) - NistTestUtils.Lgam(j + 1) + NistTestUtils.Lgam(i) - NistTestUtils.Lgam(j) - NistTestUtils.Lgam(i - j + 1));
                }
            }
            probabilities[degreesOfFreedom] = 1 - probabilities.Sum();

            for (int i = 0; i < blockCount; i++)
            {
                int matches = 0;
                for (int j = 0; j < blockLength - template.Length + 1; j++)
                {
                    bool match = true;
                    for (int k = 0; k < template.Length; k++)
                    {
                        if (template[k] != data[i * blockLength + j + k])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        matches++;
                    }
                }
                if (matches < degreesOfFreedom)
                {
                    occurrences[matches]++;
                }
                else
                {
                    occurrences[degreesOfFreedom]++;
                }
            }

            double chi2 = 0;
            for (int i = 0; i < degreesOfFreedom + 1; i++)
            {
                chi2 += Math.Pow(occurrences[i] - blockCount * probabilities[i], 2) / (blockCount * probabilities[i]);
            }

            double pValue = NistTestUtils.Igamc(degreesOfFreedom / 2.0, chi2 / 2);

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }
            return new OverlappingTemplateMatchingsTestResult(mean, eta, occurrences, probabilities, chi2, pValue);
        }
        #endregion Overlapping template matchings

        /// <summary>
        /// The focus of this test is the number of bits between matching patterns (a measure that is related to the
        /// length of a compressed sequence). The purpose of the test is to detect whether or not the sequence can be
        /// significantly compressed without loss of information. A significantly compressible sequence is
        /// considered to be non-random.<br/>
        /// For more info, see section 2.9 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <param name="initializationBlocks">A number of blocks used to initialize the table (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult MaurersUniversal(BitArray data, int blockLength, int initializationBlocks)
        {
            Stopwatch sw = new();
            sw.Start();
            if (blockLength < 2 || blockLength > 16)
            {
                if (data.Length < 904960)
                {
                    blockLength = 6;
                }
                else if (data.Length < 2068480)
                {
                    blockLength = 7;
                }
                else if (data.Length < 4654080)
                {
                    blockLength = 8;
                }
                else if (data.Length < 10342400)
                {
                    blockLength = 9;
                }
                else if (data.Length < 22753280)
                {
                    blockLength = 10;
                }
                else if (data.Length < 49643520)
                {
                    blockLength = 11;
                }
                else if (data.Length < 107560960)
                {
                    blockLength = 12;
                }
                else if (data.Length < 231669760)
                {
                    blockLength = 13;
                }
                else if (data.Length < 496435200)
                {
                    blockLength = 14;
                }
                else if (data.Length < 1059061760)
                {
                    blockLength = 15;
                }
                else
                {
                    blockLength = 16;
                }
            }

            if (initializationBlocks < 2)
            {
                initializationBlocks = 10 * (int)Math.Pow(2, blockLength);
            }

            //Precomputed statistical properties, from Handbook of applied cryptography
            double[] expectedValue = { 0, 0.7326495, 1.5374383, 2.4016068, 3.3112247, 4.2534266, 5.2177052, 6.1962507, 7.1836656,
                                            8.1764248, 9.1723243, 10.170032, 11.168765, 12.168070, 13.167693, 14.167488, 15.167379 };
            double[] variance = { 0, 0.69, 1.338, 1.901, 2.358, 2.705, 2.954, 3.125, 3.238, 3.311, 3.356, 3.384, 3.401, 3.410, 3.416, 3.419, 3.421 };

            int blockCount = (int)(Math.Floor((double)data.Length / blockLength) - initializationBlocks);
            int[] table = new int[(int)Math.Pow(2, blockLength)];

            //sigma in paper
            //Formula 16, in Marsaglia's Paper
            //Example specifies mean = Math.Sqrt(variance[blockLength]), description - the following:
            double mean = (0.7 - 0.8 / blockLength + (4 + 32 / (double)blockLength) * Math.Pow(blockCount, -3 / (double)blockLength) / 15) * Math.Sqrt(variance[blockLength] / blockCount);
            double phi = 0;

            //Table initialization
            for (int i = 1; i <= initializationBlocks; i++)
            {
                int decRep = 0;
                for (int j = 0; j < blockLength; j++)
                {
                    decRep += data[(i - 1) * blockLength + j] ? (int)Math.Pow(2, blockLength - 1 - j) : 0;
                }
                table[decRep] = i;
            }
            for (int i = initializationBlocks + 1; i <= initializationBlocks + blockCount; i++)
            {
                int decRep = 0;
                for (int j = 0; j < blockLength; j++)
                {
                    decRep += data[(i - 1) * blockLength + j] ? (int)Math.Pow(2, blockLength - 1 - j) : 0;
                }
                phi += Math.Log(i - table[decRep]) / Math.Log(2);
                table[decRep] = i;
            }
            phi /= blockCount;

            double pValue = NistTestUtils.Erfc(Math.Abs(phi - expectedValue[blockLength]) / (Math.Sqrt(2) * mean));

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"-------------------------------------\n" +
                            $"Maurer’s “universal statistical” test\n" +
                            $"-------------------------------------\n" +
                            $"N                     = {data.Length}\n" +
                            $"Block size            = {blockLength}\n" +
                            $"Blocks                = {blockCount}\n" +
                            $"Initialization blocks = {initializationBlocks}\n" +
                            $"Mean value            = {mean:0.####}\n" +
                            $"Variance              = {variance[blockLength]:0.####}\n" +
                            $"Expected              = {expectedValue[blockLength]:0.####}\n" +
                            $"Phi                   = {phi:0.####}\n" +
                            $"Discarded bits        = {data.Length - (blockCount + initializationBlocks) * blockLength}\n" +
                            $"P-value               = {pValue:0.####}\n" +
                            $"Time                  = {(int)sw.ElapsedMilliseconds} ms";

            return new SingleTestResult(new double[] { pValue }, (blockCount + initializationBlocks) * blockLength, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of this test is the length of a linear feedback shift register (LFSR). The purpose of this test is to
        /// determine whether or not the sequence is complex enough to be considered random. Random sequences
        /// are characterized by longer LFSRs. An LFSR that is too short implies non-randomness.<br/>
        /// For more info, see section 2.10 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <param name="degreesOfFreedom">Degrees of freedom of the chi-squared distribution (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult LinearComplexity(BitArray data, int blockLength, int degreesOfFreedom)
        {
            Stopwatch sw = new();
            sw.Start();
            if (blockLength < 2)
            {
                blockLength = 500;
                if (data.Length / blockLength < 200)
                {
                    blockLength = (int)(2 * Math.Sqrt(data.Length / 10));
                }
            }

            if (degreesOfFreedom < 2)
            {
                degreesOfFreedom = 3;
            }

            //nu in the paper
            //Degrees of freedom are one-sided here, instead of two-sided in the paper
            int[] occurrences = new int[degreesOfFreedom * 2 + 1];
            double[] probabilities = new double[degreesOfFreedom * 2 + 1]; //pi in the paper

            //Central element is always 0.5
            probabilities[degreesOfFreedom] = 0.5;
            //Open intervals
            probabilities[0] = 1 / (3 * Math.Pow(2, 2 * degreesOfFreedom - 1));
            probabilities[degreesOfFreedom * 2] = 1 / (3 * Math.Pow(2, 2 * degreesOfFreedom - 2));
            //Other elements
            for (int i = 1; i < degreesOfFreedom; i++)
            {
                //Negative half
                probabilities[degreesOfFreedom - i] = 1 / Math.Pow(2, 2 * i + 1);
                //Positive half
                probabilities[degreesOfFreedom + i] = 1 / Math.Pow(2, 2 * i);
            }

            int blockCount = data.Length / blockLength;

            Utils.ParallelFor(0, blockCount, 10, MaximumThreads, (u, v) =>
            {
                DateTime lastBreak = DateTime.Now;
                for (int block = u; block < v; block++)
                {
                    BitArray T = new(blockLength);
                    BitArray P = new(blockLength);
                    BitArray B = new(blockLength);
                    BitArray C = new(blockLength);
                    int L = 0;
                    int m = -1;
                    C[0] = true;
                    B[0] = true;

                    // Determining linear complexity
                    for (int bit = 0; bit < blockLength; bit++)
                    {
                        int d = data[block * blockLength + bit] ? 1 : 0;
                        for (int i = 1; i <= L; i++)
                        {
                            if (data[block * blockLength + bit - i] && C[i])
                            {
                                d++;
                            }
                        }

                        if (d % 2 == 1)
                        {
                            for (int i = 0; i < blockLength; i++)
                            {
                                T[i] = C[i];
                                P[i] = false;
                            }
                            for (int j = 0; j < blockLength; j++)
                            {
                                if (B[j])
                                {
                                    P[j + bit - m] = true;
                                }
                            }
                            for (int i = 0; i < blockLength; i++)
                            {
                                C[i] = C[i] ^ P[i];
                            }
                            if (L <= bit / 2)
                            {
                                L = bit + 1 - L;
                                m = bit;
                                for (int i = 0; i < blockLength; i++)
                                {
                                    B[i] = T[i];
                                }
                            }
                        }
                    }

                    double mean = blockLength / 2.0 + (9.0 + Math.Pow((blockLength + 1) % 2 == 0 ? -1 : 1, blockLength + 1)) / 36.0 - 1.0 / Math.Pow(2, blockLength) * (blockLength / 3.0 + 2.0 / 9.0);
                    double T_ = (blockLength % 2 == 0 ? 1 : -1) * (L - mean) + 2.0 / 9.0;

                    //Central element
                    if (T_ > -0.5 && T_ <= 0.5)
                    {
                        Interlocked.Increment(ref occurrences[degreesOfFreedom]);
                    }
                    //Open intervals
                    if (T_ <= -degreesOfFreedom + 0.5)
                    {
                        Interlocked.Increment(ref occurrences[0]);
                    }
                    if (T_ > degreesOfFreedom - 0.5)
                    {
                        Interlocked.Increment(ref occurrences[degreesOfFreedom * 2]);
                    }
                    //Other elements
                    for (int i = 1; i < degreesOfFreedom; i++)
                    {
                        //Negative half
                        if (T_ > -0.5 - i && T_ <= 0.5 - i)
                        {
                            Interlocked.Increment(ref occurrences[degreesOfFreedom - i]);
                        }
                        //Positive half
                        if (T_ > -0.5 + i && T_ <= 0.5 + i)
                        {
                            Interlocked.Increment(ref occurrences[degreesOfFreedom + i]);
                        }
                    }
                    Utils.BreakExecution(ref lastBreak);
                }
            });

            double chi2 = 0;
            for (int i = 0; i < degreesOfFreedom * 2 + 1; i++)
            {
                chi2 += Math.Pow(occurrences[i] - blockCount * probabilities[i], 2) / (blockCount * probabilities[i]);
            }

            double pValue = NistTestUtils.Igamc(degreesOfFreedom, chi2 / 2.0);

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"----------------------\n" +
                            $"Linear complexity test\n" +
                            $"----------------------\n" +
                            $"N              = {data.Length}\n" +
                            $"Block size     = {blockLength}\n" +
                            $"Blocks         = {blockCount}\n" +
                            $"Chi^2          = {chi2:0.####}\n" +
                            $"Discarded bits = {data.Length % blockLength}\n" +
                            $"P-value        = {pValue:0.####}\n" +
                            $"Time           = {(int)sw.ElapsedMilliseconds} ms\n\n" +
                            $"Frequencies:\n";

            report += Utils.FormatTable(new string[] { "", "Expected", "Observed" }, new string[][]
                    {   new string[]{ $"T<={-degreesOfFreedom + 0.5}" }.Concat(
                        Enumerable.Range(1, degreesOfFreedom-1).Select(i=>$"{-0.5 - i}<T<={ 0.5 - i}")).Concat(
                        new string[]{ $"-0.5<T<=0.5" }).Concat(
                        Enumerable.Range(1, degreesOfFreedom-1).Select(i=>$"{-0.5 + i}<T<={ 0.5 + i}")).Concat(
                        new string[] {$"T>={degreesOfFreedom - 0.5}" }).ToArray(),

                        Enumerable.Range(0, degreesOfFreedom*2+1).Select(i=>probabilities[i].ToString("0.####")).ToArray(),
                        Enumerable.Range(0, degreesOfFreedom*2+1).Select(i=>(occurrences[i] / (double)blockCount).ToString("0.####")).ToArray()
                    }, 15);

            return new SingleTestResult(new double[] { pValue }, blockCount * blockLength, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of this test is the frequency of all possible overlapping m-bit patterns across the entire
        /// sequence. The purpose of this test is to determine whether the number of occurrences of the 2m m-bit
        /// overlapping patterns is approximately the same as would be expected for a random sequence. Random
        /// sequences have uniformity; that is, every m-bit pattern has the same chance of appearing as every other
        /// m-bit pattern.<br/>
        /// For more info, see section 2.11 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult Serial(BitArray data, int blockLength)
        {
            Stopwatch sw = new();
            sw.Start();
            if (blockLength < 2) //m < [log2 n]-2
            {
                blockLength = 2;
            }

            DateTime lastBreak = DateTime.Now;
            double psim0 = Psi2(data, blockLength);
            Utils.BreakExecution(ref lastBreak);
            double psim1 = Psi2(data, blockLength - 1);
            Utils.BreakExecution(ref lastBreak);
            double psim2 = Psi2(data, blockLength - 2);
            Utils.BreakExecution(ref lastBreak);
            double del1 = psim0 - psim1;
            double del2 = psim0 - 2.0 * psim1 + psim2;
            double pValue1 = NistTestUtils.Igamc(Math.Pow(2, blockLength - 1) / 2, del1 / 2.0);
            double pValue2 = NistTestUtils.Igamc(Math.Pow(2, blockLength - 2) / 2, del2 / 2.0);

            if (pValue1 < 0 || pValue1 > 1 || pValue2 < 0 || pValue2 > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"-----------\n" +
                            $"Serial test\n" +
                            $"-----------\n" +
                            $"N          = {data.Length}\n" +
                            $"Block size = {blockLength}\n" +
                            $"Psi m      = {psim0:0.####}\n" +
                            $"Psi m-1    = {psim1:0.####}\n" +
                            $"Psi m-2    = {psim2:0.####}\n" +
                            $"Delta 1    = {del1:0.####}\n" +
                            $"Delta 2    = {del2:0.####}\n" +
                            $"P-value1   = {pValue1:0.####}\n" +
                            $"P-value2   = {pValue2:0.####}\n" +
                            $"Time       = {(int)sw.ElapsedMilliseconds} ms";

            return new SingleTestResult(new double[] { pValue1, pValue2 }, data.Count, report, (int)sw.ElapsedMilliseconds);
        }

        static double Psi2(BitArray data, int blockLength)
        {
            uint[] P = new uint[(int)Math.Pow(2, blockLength + 1) - 1];
            //Computing frequency
            for (int i = 0; i < data.Length; i++)
            {
                int k = 1;
                for (int j = 0; j < blockLength; j++)
                {
                    if (!data[(i + j) % data.Length])
                    {
                        k *= 2;
                    }
                    else
                    {
                        k = 2 * k + 1;
                    }
                }
                P[k - 1]++;
            }

            double sum = 0;
            for (int i = (int)Math.Pow(2, blockLength) - 1; i < (int)Math.Pow(2, blockLength + 1) - 1; i++)
            {
                sum += Math.Pow(P[i], 2);
            }
            sum = (sum * Math.Pow(2, blockLength) / data.Length) - data.Length;

            return sum;
        }

        /// <summary>
        /// As with the Serial test, the focus of this test is the frequency of all possible overlapping
        /// m-bit patterns across the entire sequence. The purpose of the test is to compare the frequency of
        /// overlapping blocks of two consecutive/adjacent lengths (m and m+1) against the expected result for a
        /// random sequence.<br/>
        /// For more info, see section 2.12 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <param name="blockLength">Length of a single block (-1 for autoselection)</param>
        /// <returns>Test result</returns>
        public static SingleTestResult ApproximateEntropy(BitArray data, int blockLength)
        {
            Stopwatch sw = new();
            sw.Start();
            if (blockLength < 2) //m < [log2 n]-5
            {
                blockLength = 2;
            }

            double[] ApEn = new double[2];
            uint[] P;

            int r = 0;
            DateTime lastBreak = DateTime.Now;
            for (int blockSize = blockLength; blockSize <= blockLength + 1; blockSize++)
            {
                if (blockSize == 0)
                {
                    ApEn[0] = 0.00;
                    r++;
                }
                else
                {
                    P = new uint[(int)Math.Pow(2, blockSize + 1) - 1];
                    for (int i = 1; i < P.Length - 1; i++)
                    {
                        P[i] = 0;
                    }
                    for (int i = 0; i < data.Length; i++)
                    {
                        int k = 1;
                        for (int j = 0; j < blockSize; j++)
                        {
                            k <<= 1;
                            if (data[(i + j) % data.Length])
                            {
                                k++;
                            }
                        }
                        P[k - 1]++;
                    }

                    double sum = 0.0;
                    int index = (int)Math.Pow(2, blockSize) - 1;
                    for (int i = 0; i < (int)Math.Pow(2, blockSize); i++)
                    {
                        if (P[index] > 0)
                        {
                            sum += P[index] * Math.Log(P[index] / (double)data.Length);
                        }
                        index++;
                    }
                    ApEn[r] = sum / data.Length;
                    r++;
                }
                Utils.BreakExecution(ref lastBreak);
            }

            double chi2 = 2.0 * data.Length * (Math.Log(2) - ApEn[0] + ApEn[1]);
            double pValue = NistTestUtils.Igamc(Math.Pow(2, blockLength - 1), chi2 / 2.0);

            if (pValue < 0 || pValue > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"------------------------\n" +
                            $"Approximate entropy test\n" +
                            $"------------------------\n" +
                            $"N          = {data.Length}\n" +
                            $"Block size = {blockLength}\n" +
                            $"Chi^2      = {chi2:0.####}\n" +
                            $"Phi(m)     = {ApEn[0]:0.####}\n" +
                            $"Phi(m+1)   = {ApEn[1]:0.####}\n" +
                            $"ApEn       = {ApEn[0] - ApEn[1]:0.####}\n" +
                            $"P-value    = {pValue:0.####}\n" +
                            $"Time       = {(int)sw.ElapsedMilliseconds} ms";

            return new SingleTestResult(new double[] { pValue }, data.Count, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of this test is the maximal excursion (from zero) of the random walk defined by the cumulative
        /// sum of adjusted (-1, +1) digits in the sequence.The purpose of the test is to determine whether the
        /// cumulative sum of the partial sequences occurring in the tested sequence is too large or too small relative
        /// to the expected behavior of that cumulative sum for random sequences.For a random sequence, the excursions 
        /// of the random walk should be near zero.<br/>
        /// For more info, see section 2.13 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <returns>Test result</returns>
        public static SingleTestResult CumulativeSums(BitArray data)
        {
            Stopwatch sw = new();
            sw.Start();
            int S = 0;
            int sup = 0;
            int inf = 0;
            int maxPartialSum = 0;
            int maxPartialSum_rev = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i])
                {
                    S++;
                }
                else
                {
                    S--;
                }
                if (S > sup)
                {
                    sup++;
                }
                if (S < inf)
                {
                    inf--;
                }
                maxPartialSum = (sup > -inf) ? sup : -inf;
                maxPartialSum_rev = (sup - S > S - inf) ? sup - S : S - inf;
            }

            // forward
            double sum1 = 0;
            for (int i = (-data.Length / maxPartialSum + 1) / 4; i <= (data.Length / maxPartialSum - 1) / 4; i++)
            {
                sum1 += NistTestUtils.Normal((4 * i + 1) * maxPartialSum / Math.Sqrt(data.Length));
                sum1 -= NistTestUtils.Normal((4 * i - 1) * maxPartialSum / Math.Sqrt(data.Length));
            }
            double sum2 = 0;
            for (int i = (-data.Length / maxPartialSum - 3) / 4; i <= (data.Length / maxPartialSum - 1) / 4; i++)
            {
                sum2 += NistTestUtils.Normal((4 * i + 3) * maxPartialSum / Math.Sqrt(data.Length));
                sum2 -= NistTestUtils.Normal((4 * i + 1) * maxPartialSum / Math.Sqrt(data.Length));
            }

            double pValue1 = 1.0 - sum1 + sum2;

            // backwards
            sum1 = 0;
            for (int i = (-data.Length / maxPartialSum_rev + 1) / 4; i <= (data.Length / maxPartialSum_rev - 1) / 4; i++)
            {
                sum1 += NistTestUtils.Normal((4 * i + 1) * maxPartialSum_rev / Math.Sqrt(data.Length));
                sum1 -= NistTestUtils.Normal((4 * i - 1) * maxPartialSum_rev / Math.Sqrt(data.Length));
            }
            sum2 = 0;
            for (int i = (-data.Length / maxPartialSum_rev - 3) / 4; i <= (data.Length / maxPartialSum_rev - 1) / 4; i++)
            {
                sum2 += NistTestUtils.Normal((4 * i + 3) * maxPartialSum_rev / Math.Sqrt(data.Length));
                sum2 -= NistTestUtils.Normal((4 * i + 1) * maxPartialSum_rev / Math.Sqrt(data.Length));
            }

            double pValue2 = 1.0 - sum1 + sum2;

            if (pValue1 < 0 || pValue1 > 1 || pValue2 < 0 || pValue2 > 1)
            {
                throw new InvalidOperationException("PValue is out of [0, 1] range");
            }

            sw.Stop();
            string report = $"--------------------\n" +
                            $"Cumulative sums test\n" +
                            $"--------------------\n" +
                            $"N                             = {data.Length}\n" +
                            $"Maximum partial sum (forward) = {maxPartialSum}\n" +
                            $"P-value (forward)             = {pValue1:0.####}\n" +
                            $"Maximum partial sum (reverse) = {maxPartialSum_rev}\n" +
                            $"P-value (reverse)             = {pValue2:0.####}\n" +
                            $"Time                          = {(int)sw.ElapsedMilliseconds} ms";

            return new SingleTestResult(new double[] { pValue1, pValue2 }, data.Count, report, (int)sw.ElapsedMilliseconds);
        }

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
        /// <param name="data">Data to be tested</param>
        /// <returns>Test result</returns>
        public static SingleTestResult RandomExcursions(BitArray data)
        {
            Stopwatch sw = new();
            sw.Start();
            int[] stateX = { -4, -3, -2, -1, 1, 2, 3, 4 };
            int[] counter = new int[8];
            double[,] occurrences = new double[6, 8]; //nu in the paper
            double[,] probabilities = { //pi in the paper
                         { 0.0000000000, 0.00000000000, 0.00000000000, 0.00000000000, 0.00000000000, 0.0000000000},
                         { 0.5000000000, 0.25000000000, 0.12500000000, 0.06250000000, 0.03125000000, 0.0312500000},
                         { 0.7500000000, 0.06250000000, 0.04687500000, 0.03515625000, 0.02636718750, 0.0791015625},
                         { 0.8333333333, 0.02777777778, 0.02314814815, 0.01929012346, 0.01607510288, 0.0803755143},
                         { 0.8750000000, 0.01562500000, 0.01367187500, 0.01196289063, 0.01046752930, 0.0732727051}
            };

            int[] S_k = new int[data.Length];

            //Determine sycles
            List<int> cycles = new();
            cycles.Add(0);
            S_k[0] = data[0] ? 1 : -1;
            for (int i = 1; i < data.Length; i++)
            {
                S_k[i] = S_k[i - 1] + (data[i] ? 1 : -1);
                if (S_k[i] == 0)
                {
                    cycles.Add(i);
                }
            }
            if (S_k[data.Length - 1] != 0)
            {
                cycles.Add(data.Length - 1);
            }
            else
            {
                cycles[^1] = data.Length;
            }

            double constraint = Math.Max(0.005 * Math.Pow(data.Length, 0.5), 500);
            string report = $"----------------------\n" +
                            $"Random excursions test\n" +
                            $"----------------------\n" +
                            $"N                        = {data.Length}\n" +
                            $"Number of cycles         = {cycles.Count}\n" +
                            $"Minimum number of cycles = {constraint:0.####}\n";

            if (cycles.Count < constraint) //chi-squared is invalid under this conditions
            {
                report += $"Error: insufficient number of cycles. Test is not applicable because Chi^2 is invalid under this conditions. Try increasing sequence length (see 3.14 in the paper for more details)";
                return new SingleTestResult(new double[8], data.Count, report, (int)sw.ElapsedMilliseconds);
            }

            int cycleStart = 0;
            int cycleStop = cycles[1];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    occurrences[i, j] = 0;
                }
            }

            for (int i = 1; i < cycles.Count; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    counter[j] = 0;
                }
                for (int j = cycleStart; j < cycleStop; j++)
                {
                    if ((S_k[j] >= 1 && S_k[j] <= 4) || (S_k[j] >= -4 && S_k[j] <= -1))
                    {
                        counter[S_k[j] + (S_k[j] < 0 ? 4 : 3)]++;
                    }
                }
                cycleStart = cycles[i] + 1;
                if (i < cycles.Count - 1)
                {
                    cycleStop = cycles[i + 1];
                }

                for (int j = 0; j < 8; j++)
                {
                    if ((counter[j] >= 0) && (counter[j] <= 4))
                    {
                        occurrences[counter[j], j]++;
                    }
                    else if (counter[j] >= 5)
                    {
                        occurrences[5, j]++;
                    }
                }
            }

            double[] pValues = new double[8];
            double[] chi2 = new double[8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    chi2[i] += Math.Pow(occurrences[j, i] - (cycles.Count - 1) * probabilities[Math.Abs(stateX[i]), j], 2) / ((cycles.Count - 1) * probabilities[Math.Abs(stateX[i]), j]);
                }
                pValues[i] = NistTestUtils.Igamc(2.5, chi2[i] / 2.0);

                if (pValues[i] < 0 || pValues[i] > 1)
                {
                    throw new InvalidOperationException("PValue is out of [0, 1] range");
                }
            }

            sw.Stop();
            report += $"Time                     = {(int)sw.ElapsedMilliseconds} ms\n\n" +
                      $"States:\n";
            report += Utils.FormatTable(new string[] { "State", "Chi^2", "PValue" }, new string[][] {
                stateX.Select(x=>x.ToString()).ToArray(),
                chi2.Select(x=>x.ToString("0.####")).ToArray(),
                pValues.Select(x=>x.ToString("0.####")).ToArray()
            }, 8);

            return new SingleTestResult(pValues, data.Count, report, (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// The focus of this test is the total number of times that a particular state is visited (i.e., occurs) in a
        /// cumulative sum random walk. The purpose of this test is to detect deviations from the expected number
        /// of visits to various states in the random walk. This test is actually a series of eighteen tests (and
        /// conclusions), one test and conclusion for each of the states: -9, -8, …, -1 and +1, +2, …, +9.<br/>
        /// Note that for this test it is recommended to keep block size as big as possible to avoid getting an <see cref="TestResultEnum.IncufficientCycles"/> resut.<br/>
        /// For more info, see section 2.15 of the paper
        /// </summary>
        /// <param name="data">Data to be tested</param>
        /// <returns>Test result</returns>
        public static SingleTestResult RandomExcursionsVariant(BitArray data)
        {
            Stopwatch sw = new();
            sw.Start();
            int[] stateX = { -9, -8, -7, -6, -5, -4, -3, -2, -1, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[] S_k = new int[data.Length];

            int cycles = 0;
            S_k[0] = data[0] ? 1 : -1;
            for (int i = 1; i < data.Length; i++)
            {
                S_k[i] = S_k[i - 1] + (data[i] ? 1 : -1);
                if (S_k[i] == 0)
                {
                    cycles++;
                }
            }
            if (S_k[data.Length - 1] != 0)
            {
                cycles++;
            }

            double constraint = Math.Max(0.005 * Math.Pow(data.Length, 0.5), 500);
            string report = $"----------------------\n" +
                            $"Random excursions test\n" +
                            $"----------------------\n" +
                            $"N                        = {data.Length}\n" +
                            $"Number of cycles         = {cycles}\n" +
                            $"Minimum number of cycles = {constraint:0.####}\n";

            if (cycles < constraint)
            {
                report += $"Error: insufficient number of cycles. Test is not applicable because Chi^2 is invalid under this conditions. Try increasing sequence length (see 3.14 in the paper for more details)";
                return new SingleTestResult(new double[18], data.Count, report, (int)sw.ElapsedMilliseconds);
            }

            double[] pValues = new double[18];
            int[] visits = new int[18];
            DateTime lastBreak = DateTime.Now;
            for (int i = 0; i <= 17; i++)
            {
                visits[i] = S_k.Count(x => x == stateX[i]);
                pValues[i] = NistTestUtils.Erfc(Math.Abs(visits[i] - cycles) / Math.Sqrt(2.0 * cycles * (4.0 * Math.Abs(stateX[i]) - 2)));
                if (pValues[i] < 0 || pValues[i] > 1)
                {
                    throw new InvalidOperationException("PValue is out of [0, 1] range");
                }
                Utils.BreakExecution(ref lastBreak);
            }

            sw.Stop();

            report += $"Time                     = {(int)sw.ElapsedMilliseconds} ms\n\n" +
                      $"States:\n";
            report += Utils.FormatTable(new string[] { "State", "Total visits", "PValue" }, new string[][] {
                stateX.Select(x=>x.ToString()).ToArray(),
                visits.Select(x=>x.ToString("0.####")).ToArray(),
                pValues.Select(x=>x.ToString("0.####")).ToArray()
            }, 8);
            return new SingleTestResult(pValues, data.Count, report, (int)sw.ElapsedMilliseconds);
        }
    }
}
