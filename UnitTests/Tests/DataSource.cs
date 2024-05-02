using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TrulyRandom;

namespace UnitTests
{
    /// <summary>
    /// Evaluates the uniformity of the result data distribution.
    /// </summary>
    [TestClass]
    public class DataSource
    {
        TrulyRandom.Modules.Buffer buffer;
        TrulyRandom.DataSource dataSource;
        [TestInitialize()]
        public void TestInitialize()
        {
            buffer = new TrulyRandom.Modules.Buffer();
            buffer.Start();
            dataSource = buffer.DataSource;
            buffer.BufferSize = 100_000_000;
            buffer.AddData(Utils.GetPseudorandomBytes(100_000_000));
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            buffer.Dispose();
        }

        [TestMethod]
        public void Bit()
        {
            int ones = 0;
            int zeros = 0;
            while (dataSource.BitsAvailable > 1)
            {
                if (dataSource.GetBit())
                {
                    ones++;
                }
                else
                {
                    zeros++;
                }
            }

            double rate = ones / (double)(ones + zeros);
            Assert.IsTrue(rate > 0.4999 && rate < 0.5001);
        }

        [TestMethod]
        public void Byte()
        {
            int[] counts = new int[256];
            while (dataSource.BitsAvailable > 10)
            {
                counts[dataSource.GetByte()]++;
            }

            double rate = counts.Min() / (double)counts.Max();
            Assert.IsTrue(rate > 0.95);
        }

        [TestMethod]
        public void ULong()
        {
            int[] counts = new int[1000];
            while (dataSource.BitsAvailable > 100)
            {
                counts[dataSource.GetULong(200, 1200) - 200]++;
            }

            double rate = counts.Min() / (double)counts.Max();
            Assert.IsTrue(rate > 0.95);
        }

        [TestMethod]
        public void Long()
        {
            int[] counts = new int[1000];
            while (dataSource.BitsAvailable > 100)
            {
                counts[dataSource.GetLong(-200, 800) + 200]++;
            }

            double rate = counts.Min() / (double)counts.Max();
            Assert.IsTrue(rate > 0.95);
        }

        [TestMethod]
        public void UInt()
        {
            int[] counts = new int[1000];
            while (dataSource.BitsAvailable > 100)
            {
                counts[dataSource.GetUInt(200, 1200) - 200]++;
            }

            double rate = counts.Min() / (double)counts.Max();
            Assert.IsTrue(rate > 0.95);
        }

        [TestMethod]
        public void Int()
        {
            int[] counts = new int[1000];
            while (dataSource.BitsAvailable > 100)
            {
                counts[dataSource.GetInt(-200, 800) + 200]++;
            }

            double rate = counts.Min() / (double)counts.Max();
            Assert.IsTrue(rate > 0.95);
        }

        [TestMethod]
        public void Double()
        {
            List<double> samples = new();
            while (dataSource.BitsAvailable > 100)
            {
                samples.Add(dataSource.GetDouble(true, true));
            }

            int[] counts = new int[1000];
            foreach (double sample in samples)
            {
                counts[(int)Math.Floor(counts.Length * sample)]++;
            }
            double rate = counts.Min() / (double)counts.Max();
            Assert.IsTrue(rate > 0.95);
        }

        [TestMethod]
        public void Normal()
        {
            //Intervals corresponding to -3,-2,-1,1,2 and 3 sigma intervals.
            //We won't check intervals < -3 and > 3 sigma because of a small sample size: these should be just 0.135% each
            double[] intervals = new double[6];
            int count = 0;
            while (dataSource.BitsAvailable > 100)
            {
                double value = dataSource.GetNormal(0, 1);
                // -1/1 sigma
                if (Math.Abs(value) < 1)
                {
                    intervals[value < 0 ? 2 : 3]++;
                }
                // -2/2 sigma
                else if (Math.Abs(value) < 2)
                {
                    intervals[value < 0 ? 1 : 4]++;
                }
                // -3/3 sigma
                else if (Math.Abs(value) < 3)
                {
                    intervals[value < 0 ? 0 : 5]++;
                }
                count++;
            }

            //calculating relative frequencies
            for (int i = 0; i < 6; i++)
            {
                intervals[i] /= count;
            }

            //dividing by expected values
            intervals[0] /= 0.021;
            intervals[5] /= 0.021;
            intervals[1] /= 0.136;
            intervals[4] /= 0.136;
            intervals[2] /= 0.341;
            intervals[3] /= 0.341;

            //results corresponds to a deviation from the expected frequency for each of the intervals

            Assert.IsTrue(intervals.Min() > 0.90 && intervals.Max() < 1.1);
        }

        [TestMethod]
        public void Shuffle()
        {
            int[,] counts = new int[100, 100];
            while (dataSource.BitsAvailable > 1000)
            {
                int[] arr = Enumerable.Range(0, 100).ToArray();
                dataSource.Shuffle(arr);
                for (int i = 0; i < arr.Length; i++)
                {
                    counts[arr[i], i]++;
                }
            }

            int min = int.MaxValue;
            int max = int.MinValue;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (max < counts[i, j])
                    {
                        max = counts[i, j];
                    }
                    if (min > counts[i, j])
                    {
                        min = counts[i, j];
                    }
                }
            }

            double rate = min / (double)max;
            Assert.IsTrue(rate > 0.9);
        }
    }
}
