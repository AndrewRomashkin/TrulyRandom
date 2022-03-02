using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TrulyRandom;

namespace UnitTests
{
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
            Utils.InvokePrivate(buffer, "AddData", Utils.GetPseudorandomBytes(100_000_000));
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
