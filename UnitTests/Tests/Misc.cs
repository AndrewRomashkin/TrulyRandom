using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using TrulyRandom.Models;
using TrulyRandom.Modules;
using TrulyRandom.Modules.Extractors;

namespace UnitTests;

/// <summary>
/// Miscellaneous tests
/// </summary>
[TestClass]
public class Misc
{
    private DateTime testStart;

    [TestInitialize()]
    public void TestInitialize()
    {
        testStart = DateTime.Now;
    }

    [TestMethod]
    public void DiskBuffer()
    {
        TrulyRandom.Modules.Buffer buffer = new();
        buffer.BufferFileSize = 100;
        buffer.MinBytesInBuffer = 100;
        buffer.MaxFilesToStore = 10;
        buffer.ClearDiskData();
        buffer.Start();

        Assert.AreEqual(0, buffer.BytesInBuffer);
        Assert.AreEqual(0, buffer.GetNumberOfDataFilesAvailable());

        buffer.AddData(Enumerable.Repeat((byte)0xFF, 300).ToArray());
        Utils.AssignPrivate(buffer, "nextWriteCheck", DateTime.MinValue);
        Utils.InvokePrivate(buffer, "SaveToDisk", false);

        Assert.AreEqual(300, buffer.BytesInBuffer);
        Assert.AreEqual(0, buffer.GetNumberOfDataFilesAvailable());

        buffer.ClearDiskData();
        Assert.AreEqual(0, buffer.GetNumberOfDataFilesAvailable());

        buffer.MaxBytesInBuffer = 200;
        Utils.AssignPrivate(buffer, "nextWriteCheck", DateTime.MinValue);
        Utils.InvokePrivate(buffer, "SaveToDisk", false);

        Assert.AreEqual(2, buffer.GetNumberOfDataFilesAvailable());

        buffer.ClearBuffer();
        Utils.AssignPrivate(buffer, "nextReadCheck", DateTime.MinValue);
        Utils.InvokePrivate(buffer, "LoadFromDisk");

        Assert.AreEqual(1, buffer.GetNumberOfDataFilesAvailable());
        Assert.AreEqual(100, buffer.BytesInBuffer);

        buffer.ClearDiskData();
        Assert.AreEqual(0, buffer.GetNumberOfDataFilesAvailable());
        buffer.Dispose();
    }

    [TestMethod]
    public void MultipleSourceMixing()
    {
        TrulyRandom.Modules.Buffer buffer1 = new();
        buffer1.AddData(Enumerable.Repeat((byte)0, 100).ToArray());
        buffer1.Start();
        TrulyRandom.Modules.Buffer buffer2 = new();
        buffer2.AddData(Enumerable.Repeat((byte)1, 57).ToArray());
        buffer2.Start();
        TrulyRandom.Modules.Buffer buffer3 = new();
        buffer3.AddData(Enumerable.Repeat((byte)2, 5).ToArray());
        buffer3.Start();
        TrulyRandom.Modules.Buffer buffer4 = new();
        buffer4.AddSource(buffer1);
        buffer4.AddSource(buffer2);
        buffer4.AddSource(buffer3);
        buffer4.Start();

        while ((buffer1.BytesInBuffer > 0 || buffer2.BytesInBuffer > 0 || buffer3.BytesInBuffer > 0) && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        Thread.Sleep(10);

        byte[] data = buffer4.ReadAll();
        (int Zeros, int Ones, int Twos) = GetLongestRuns(data);
        Assert.AreEqual(0, buffer1.BytesInBuffer);
        Assert.AreEqual(0, buffer2.BytesInBuffer);
        Assert.AreEqual(0, buffer3.BytesInBuffer);
        Assert.AreEqual(0, buffer4.BytesInBuffer);
        Assert.AreEqual(162, data.Length);
        Assert.AreEqual(2, Zeros);
        Assert.AreEqual(1, Ones);
        Assert.AreEqual(1, Twos);
        Assert.AreEqual(100, data.Where(x => x == 0).Count());
        Assert.AreEqual(57, data.Where(x => x == 1).Count());
        Assert.AreEqual(5, data.Where(x => x == 2).Count());

        buffer1.Dispose();
        buffer2.Dispose();
        buffer3.Dispose();
        buffer4.Dispose();
    }

    [TestMethod]
    public void MultipleSourceNoMixing()
    {
        TrulyRandom.Modules.Buffer buffer1 = new();
        buffer1.AddData(Enumerable.Repeat((byte)0, 100).ToArray());
        buffer1.Start();
        TrulyRandom.Modules.Buffer buffer2 = new();
        buffer2.AddData(Enumerable.Repeat((byte)1, 57).ToArray());
        buffer2.Start();
        TrulyRandom.Modules.Buffer buffer3 = new();
        buffer3.AddData(Enumerable.Repeat((byte)2, 5).ToArray());
        buffer3.Start();
        TrulyRandom.Modules.Buffer buffer4 = new();
        buffer4.AddSource(buffer1);
        buffer4.AddSource(buffer2);
        buffer4.AddSource(buffer3);
        buffer4.MixDataFromDifferentSources = false;
        buffer4.Start();

        while ((buffer1.BytesInBuffer > 0 || buffer2.BytesInBuffer > 0 || buffer3.BytesInBuffer > 0) && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        Thread.Sleep(10);

        byte[] data = buffer4.ReadAll();
        (int Zeros, int Ones, int Twos) = GetLongestRuns(data);
        Assert.AreEqual(0, buffer1.BytesInBuffer);
        Assert.AreEqual(0, buffer2.BytesInBuffer);
        Assert.AreEqual(0, buffer3.BytesInBuffer);
        Assert.AreEqual(0, buffer4.BytesInBuffer);
        Assert.AreEqual(162, data.Length);
        Assert.AreEqual(100, Zeros);
        Assert.AreEqual(57, Ones);
        Assert.AreEqual(5, Twos);
        Assert.AreEqual(100, data.Where(x => x == 0).Count());
        Assert.AreEqual(57, data.Where(x => x == 1).Count());
        Assert.AreEqual(5, data.Where(x => x == 2).Count());

        buffer1.Dispose();
        buffer2.Dispose();
        buffer3.Dispose();
        buffer4.Dispose();
    }

    private static (int Zeros, int Ones, int Twos) GetLongestRuns(byte[] data)
    {
        int longestRunOfZeros = 0;
        int longestRunOfOnes = 0;
        int longestRunOfTwos = 0;
        int curentRunOfZeros = 0;
        int currentRunOfOnes = 0;
        int currentRunOfTwos = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == 0)
            {
                curentRunOfZeros++;
                currentRunOfOnes = 0;
                currentRunOfTwos = 0;
                if (longestRunOfZeros < curentRunOfZeros)
                {
                    longestRunOfZeros = curentRunOfZeros;
                }
            }
            else if (data[i] == 1)
            {
                curentRunOfZeros = 0;
                currentRunOfOnes++;
                currentRunOfTwos = 0;
                if (longestRunOfOnes < currentRunOfOnes)
                {
                    longestRunOfOnes = currentRunOfOnes;
                }
            }
            else if (data[i] == 2)
            {
                curentRunOfZeros = 0;
                currentRunOfOnes = 0;
                currentRunOfTwos++;
                if (longestRunOfTwos < currentRunOfTwos)
                {
                    longestRunOfTwos = currentRunOfTwos;
                }
            }
        }
        return (longestRunOfZeros, longestRunOfOnes, longestRunOfTwos);
    }

    [TestMethod]
    public void DynamicCoefficient()
    {
        TrulyRandom.Modules.Buffer buffer1 = new();
        buffer1.Start();

        HashExtractor extractor = new();
        extractor.BatchSize = 640;
        extractor.HashFunction = HashExtractor.HashFunctionType.SHA512;
        extractor.InputBlockSize = 64; // compression == 1
        extractor.AddSource(buffer1);

        TrulyRandom.Modules.Buffer buffer2 = new();
        buffer2.BufferSize = 1000;
        buffer2.AddSource(extractor);
        buffer2.Start();
        extractor.DynamicCoefficientSource = buffer2;

        buffer1.AddData(Enumerable.Repeat((byte)0, 640).ToArray());
        extractor.Start();

        Thread.Sleep(100);

        Assert.AreEqual(1, extractor.ActualCompression);
        Assert.AreEqual(0, extractor.BytesInBuffer);
        Assert.AreEqual(640, buffer2.BytesInBuffer);

        buffer1.AddData(Enumerable.Repeat((byte)0, 640).ToArray());

        Thread.Sleep(100);

        Assert.AreEqual(3, extractor.ActualCompression);   //dynamic coefficient = 0.64, that means compression multiplier by 3
        Assert.AreEqual(640 + 192, buffer2.BytesInBuffer);    //640 bytes is 3 blocks of 64*3 = 192 bytes. 192*3 compressed is 3 blocks of 64 bytes = 192 bytes.

        buffer1.Dispose();
        buffer2.Dispose();
        extractor.Dispose();
    }

    [TestMethod]
    public void Seeding()
    {
        TrulyRandom.Modules.Buffer seedSource = new();
        seedSource.Start();
        seedSource.AddData(Utils.First1mDigitsOfE.ToByteArray());

        TrulyRandom.Modules.Buffer buffer = new();
        buffer.Start();

        ShuffleExtractor extractor = new();
        extractor.SeedRotationInterval = 200;
        extractor.BatchSize = 100;
        extractor.BlockSize = 1;
        extractor.AddSource(buffer);
        extractor.Start();

        byte[][] result = new byte[6][];

        extractor.SetSeed(new byte[] { 100 });
        buffer.AddData(Utils.First1mDigitsOfE[..800].ToByteArray());
        buffer.AddData(Utils.First1mDigitsOfE[..800].ToByteArray());
        Thread.Sleep(100);
        result[0] = extractor.ReadExactly(extractor.BytesInBuffer / 2);
        result[1] = extractor.ReadAll();

        extractor.SeedSource = seedSource;
        extractor.ForceRotateSeed();

        buffer.AddData(Utils.First1mDigitsOfE[..800].ToByteArray());
        buffer.AddData(Utils.First1mDigitsOfE[..800].ToByteArray());
        Thread.Sleep(100);
        result[2] = extractor.ReadExactly(extractor.BytesInBuffer / 2);
        result[3] = extractor.ReadAll();

        buffer.AddData(Utils.First1mDigitsOfE[..800].ToByteArray());
        buffer.AddData(Utils.First1mDigitsOfE[..800].ToByteArray());
        Thread.Sleep(100);
        result[4] = extractor.ReadExactly(extractor.BytesInBuffer / 2);
        result[5] = extractor.ReadAll();

        seedSource.Dispose();
        buffer.Dispose();
        extractor.Dispose();

        Assert.IsTrue(result[0].SequenceEqual(result[1]));
        Assert.IsTrue(result[2].SequenceEqual(result[3]));
        Assert.IsTrue(result[4].SequenceEqual(result[5]));
        Assert.IsTrue(!result[0].SequenceEqual(result[2]));
        Assert.IsTrue(!result[2].SequenceEqual(result[4]));
    }

    [TestMethod]
    public void TesterE()
    {
        TrulyRandom.Modules.Buffer buffer = new();
        buffer.Start();

        Tester tester = new();
        tester.BatchSize = 125000;
        tester.AddSource(buffer);
        tester.Start();

        buffer.AddData(Utils.First1mDigitsOfE.ToByteArray());

        while (tester.TotalBytesConsumed == 0 && !testStart.WasAgo(TimeSpan.FromSeconds(300)))
        {
            Thread.Sleep(10);
        }

        TrulyRandom.NistTests.FullTestResult lastTestResult = tester.LastTestResult;

        Assert.IsTrue(tester.BytesInBuffer <= 125000);
        Assert.IsTrue(tester.BytesInBuffer > 124000);
        Assert.AreEqual(TrulyRandom.NistTests.TestType.All, tester.TestParameters.TestsToPerform);
        Assert.IsTrue(lastTestResult.Success);
        Assert.AreEqual(1, lastTestResult.SuccessfulTestProportion);

        tester.Dispose();
        buffer.Dispose();
    }

    [TestMethod]
    public void TesterBadSequence()
    {
        TrulyRandom.Modules.Buffer buffer = new();
        buffer.Start();

        Tester tester = new();
        tester.AddSource(buffer);
        tester.AutoSelectTests = false;
        tester.BatchSize = 12_500;
        tester.Start();

        buffer.AddData(Enumerable.Range(0, 12_500).Select(x => (byte)(x % 256)).ToArray());

        while (tester.TotalBytesConsumed == 0 && !testStart.WasAgo(TimeSpan.FromSeconds(300)))
        {
            Thread.Sleep(10);
        }

        TrulyRandom.NistTests.FullTestResult lastTestResult = tester.LastTestResult;

        Assert.AreEqual(0, buffer.BytesInBuffer);
        Assert.AreEqual(TrulyRandom.NistTests.TestType.All, tester.TestParameters.TestsToPerform);
        Assert.IsTrue(!lastTestResult.Success);
        Assert.IsTrue(lastTestResult.SuccessfulTestProportion < 1);

        tester.Dispose();
        buffer.Dispose();
    }

    [TestMethod]
    public void DisposeMisc()
    {
        Module[] modules = { new TrulyRandom.Modules.Buffer(), new Tester() };
        foreach (Module module in modules)
        {
            module.Start();
        }
        Thread.Sleep(100);
        foreach (Module module in modules)
        {
            module.Dispose();
        }
        Thread.Sleep(500);
        Assert.IsTrue(!modules.Where(x => !x.Disposed).Any());
    }

    [TestMethod]
    public void TestResultAnalysis()
    {
        TrulyRandom.NistTests tester = new();
        tester.Parameters.TestsToPerform = TrulyRandom.NistTests.TestType.All & ~TrulyRandom.NistTests.TestType.DiscreteFourierTransform &
            ~TrulyRandom.NistTests.TestType.NonOverlappingTemplateMatchings & ~TrulyRandom.NistTests.TestType.LinearComplexity;

        System.Collections.BitArray data = Utils.First1mDigitsOfE.ToBitArray();

        tester.Parameters.AllowedFailedTestProportion = 0;
        tester.Parameters.AllowedFailedSubtestProportion = 0;
        tester.Parameters.LongTermEvaluation.PassByDefault = false;
        TrulyRandom.NistTests.FullTestResult result = tester.Perform(data);

        Assert.IsFalse(result.Success);

        tester.ClearHistory();
        tester.Parameters.AllowedFailedTestProportion = 0;
        tester.Parameters.AllowedFailedSubtestProportion = 0;
        tester.Parameters.LongTermEvaluation.PassByDefault = true;
        result = tester.Perform(data);

        Assert.IsFalse(result.Success);

        tester.ClearHistory();
        tester.Parameters.AllowedFailedTestProportion = 0;
        tester.Parameters.AllowedFailedSubtestProportion = 0.1;
        tester.Parameters.LongTermEvaluation.PassByDefault = false;
        result = tester.Perform(data);

        Assert.IsFalse(result.Success);

        tester.ClearHistory();
        tester.Parameters.AllowedFailedTestProportion = 0.1;
        tester.Parameters.AllowedFailedSubtestProportion = 0;
        tester.Parameters.LongTermEvaluation.PassByDefault = false;
        result = tester.Perform(data);

        Assert.IsTrue(result.Success);

        tester.Parameters.AllowedFailedTestProportion = 0;
        tester.Parameters.AllowedFailedSubtestProportion = 0.2;
        tester.Parameters.LongTermEvaluation.PassByDefault = true;
        tester.Parameters.LongTermEvaluation.MinPreviousTestResultsToCheck = 2;
        result = tester.Perform(data);
        Assert.IsTrue(result.Success);
        result = tester.Perform(data);
        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public void Compression()
    {
        TrulyRandom.Modules.Buffer buffer;
        buffer = new TrulyRandom.Modules.Buffer();
        buffer.Start();
        buffer.AddData(Enumerable.Repeat((byte)0xAA, 10_000).ToArray());

        VonNeumannExtractor extractor = new();
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        Thread.Sleep(100);

        extractor.Dispose();
        buffer.Dispose();

        byte[] data = extractor.ReadAll();
        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.AreEqual(2, extractor.ActualCompression);
        Assert.IsTrue(!data.Where(x => x != 0).Any());
        Assert.AreEqual(5000, data.Length);
    }

    [TestMethod]
    public void BPS()
    {
        TrulyRandom.Modules.Buffer buffer;
        buffer = new TrulyRandom.Modules.Buffer();
        buffer.Start();
        buffer.AddData(Enumerable.Repeat((byte)0xAA, 10_000).ToArray());

        VonNeumannExtractor extractor = new();
        extractor.AddSource(buffer);
        extractor.BatchSize = 10;
        extractor.Start();

        Thread.Sleep(1500);

        extractor.Dispose();
        buffer.Dispose();

        Assert.IsTrue(extractor.BytesPerSecond > 10_000);
        Assert.IsTrue(extractor.BytesPerSecondInclPause > 100);
    }

    [TestMethod]
    public void Entropy()
    {
        TrulyRandom.Modules.Buffer buffer1;
        buffer1 = new TrulyRandom.Modules.Buffer();
        buffer1.CalculateEntropy = true;
        buffer1.Start();
        buffer1.AddData(Enumerable.Repeat((byte)0, 10_000).ToArray());

        TrulyRandom.Modules.Buffer buffer2;
        buffer2 = new TrulyRandom.Modules.Buffer();
        buffer2.CalculateEntropy = true;
        buffer2.Start();
        buffer2.AddData(Enumerable.Range(0, 256).Select(x => (byte)(x % 256)).ToArray());

        Thread.Sleep(1500);

        Assert.AreEqual(0, buffer1.Entropy);
        Assert.AreEqual(1, buffer2.Entropy);

        buffer1.Dispose();
        buffer2.Dispose();
    }

    [TestMethod]
    public void CircularBuffer()
    {
        TrulyRandom.CircularBuffer<byte> buffer = new(3);
        buffer.Write(new byte[] { 1, 2, 3 });
        byte b = buffer.Read();
        Assert.IsTrue(b == 1);
        buffer.Write(new byte[] { 4 });
        buffer.Write(5);
        Assert.AreEqual(3, buffer.Count);
        byte[] arr = buffer.Read(3);
        Assert.AreEqual(3, arr[0]);
        Assert.AreEqual(4, arr[1]);
        Assert.AreEqual(5, arr[2]);
        Assert.AreEqual(0, buffer.Count);
    }
}
