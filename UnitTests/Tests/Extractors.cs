using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using TrulyRandom.Models;
using TrulyRandom.Modules.Extractors;

namespace UnitTests;

/// <summary>
/// Evaluates the functionality of different extractors
/// </summary>
[TestClass]
public class Extractors
{
    private DateTime testStart;
    private TrulyRandom.Modules.Buffer buffer;

    [TestInitialize()]
    public void TestInitialize()
    {
        testStart = DateTime.Now;
        buffer = new TrulyRandom.Modules.Buffer();
        buffer.Start();
    }

    [TestCleanup()]
    public void TestCleanup()
    {
        buffer.Dispose();
    }

    private static byte[] Generate1MbBadSequence()
    {
        return Enumerable.Repeat(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 12500).SelectMany(x => x).ToArray();
    }

    [TestMethod]
    public void DeflateE()
    {
        buffer.AddData(Utils.First1mDigitsOfE.ToByteArray());
        DeflateExtractor extractor = new();
        extractor.BatchSize = 5000;
        extractor.CalculateEntropy = true;
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        extractor.ForceRefreshEntropy();

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.IsTrue(extractor.Entropy > 0.9);
        Assert.IsTrue(extractor.BytesInBuffer > 125000 * 0.9);
    }

    [TestMethod]
    public void DeflateBadSequence()
    {
        buffer.AddData(Generate1MbBadSequence());

        DeflateExtractor extractor = new();
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.IsTrue(extractor.BytesInBuffer < 1000);
    }

    [TestMethod]
    public void VonNeumannE()
    {
        buffer.AddData(Utils.First1mDigitsOfE.ToByteArray());

        VonNeumannExtractor extractor = new();
        extractor.CalculateEntropy = true;
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        extractor.ForceRefreshEntropy();

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.IsTrue(extractor.Entropy > 0.9);
        Assert.IsTrue(extractor.BytesInBuffer > 125000 / 4 * 0.9 && extractor.BytesInBuffer < 125000 / 4 * 1.1);
    }

    [TestMethod]
    public void VonNeumannBadSequence()
    {
        buffer.AddData(Generate1MbBadSequence());

        VonNeumannExtractor extractor = new();
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        extractor.ForceRefreshEntropy();

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.IsTrue(extractor.Entropy < 0.7);
        Assert.IsTrue(extractor.BytesInBuffer < 125000 / 4 / 1.5);
    }

    [TestMethod]
    public void HashE()
    {
        buffer.AddData(Utils.First1mDigitsOfE.ToByteArray());

        HashExtractor extractor = new Sha512Extractor();
        extractor.BatchSize = 5000;
        extractor.InputBlockSize = 125; //Exactly 1000 blocks, 1000 bits each
        extractor.CalculateEntropy = true;
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        extractor.ForceRefreshEntropy();

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.IsTrue(extractor.Entropy > 0.9);
        Assert.AreEqual(64 * 1000, extractor.BytesInBuffer);
    }

    [TestMethod]
    public void HashBadSequence()
    {
        buffer.AddData(Generate1MbBadSequence());

        HashExtractor extractor = new Sha512Extractor();
        extractor.BatchSize = 5000;
        extractor.Chaining = false;
        extractor.InputBlockSize = 10; //All blocks are the same
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        extractor.ForceRefreshEntropy();

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.IsTrue(extractor.Entropy < 0.9);
        Assert.AreEqual(64 * 12500, extractor.BytesInBuffer);
    }

    [TestMethod]
    public void ShuffleELargeBlock()
    {
        buffer.AddData(Utils.First1mDigitsOfE.ToByteArray());

        ShuffleExtractor extractor = new();
        extractor.BlockSize = 1000;
        extractor.BatchSize = 125000;
        extractor.CalculateEntropy = true;
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        extractor.ForceRefreshEntropy();

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.IsTrue(extractor.Entropy > 0.9);
        Assert.IsTrue(extractor.BytesInBuffer > 120000);
    }

    [TestMethod]
    public void ShuffleESmallBlock()
    {
        buffer.AddData(Utils.First1mDigitsOfE.ToByteArray());

        ShuffleExtractor extractor = new();
        extractor.BlockSize = 1;
        extractor.BatchSize = 125000;
        extractor.CalculateEntropy = true;
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }
        extractor.ForceRefreshEntropy();

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.IsTrue(extractor.Entropy > 0.9);
        Assert.IsTrue(extractor.BytesInBuffer < 50000);
    }

    [TestMethod]
    public void ShuffleArrayOfOnes()
    {
        //buffer of only ones guarantees that at some time random number should be repicked (because it's larger than a constraint) over and over until data depletes
        buffer.AddData(Enumerable.Repeat((byte)0xFF, 125000).ToArray());

        ShuffleExtractor extractor = new();
        extractor.SetSeed(new byte[] { 0 });
        extractor.BlockSize = 1;
        extractor.BatchSize = 125000;
        extractor.AddSource(buffer);
        extractor.Start();

        while (!extractor.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }

        extractor.Dispose();

        Assert.IsTrue(extractor.NoDataToProcess);
        Assert.AreEqual(0, extractor.BytesInBuffer);
    }

    [TestMethod]
    public void XorExtractor()
    {
        byte[] data = Enumerable.Range(0, 10).Select(x => (byte)x).ToArray();
        for (int i = 0; i < 6; i++)
        {
            buffer.AddData(data);
        }

        XorExtractor extractor1 = new();
        extractor1.SetSeed(new byte[] { 0 });
        extractor1.Compression = 3;
        extractor1.BatchSize = data.Length * extractor1.Compression;
        extractor1.CalculateEntropy = true;
        extractor1.AddSource(buffer);
        extractor1.Start();

        while (!extractor1.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }

        XorExtractor extractor2 = new();
        extractor2.SetSeed(new byte[] { 0 });
        extractor2.Compression = 2;
        extractor2.BatchSize = data.Length * extractor2.Compression;
        extractor2.CalculateEntropy = true;
        extractor2.AddSource(extractor1);
        extractor2.Start();

        while (!extractor2.NoDataToProcess && !testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            Thread.Sleep(10);
        }

        Assert.IsTrue(extractor1.NoDataToProcess);
        Assert.IsTrue(extractor2.NoDataToProcess);
        byte[] result = extractor2.ReadAll();
        Assert.IsTrue(result.Length == data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            Assert.IsTrue(result[i] == 0);
        }

        extractor1.Dispose();
        extractor2.Dispose();
    }

    [TestMethod]
    public void Dispose()
    {
        Extractor[] modules = { new DeflateExtractor(), new Sha512Extractor(), new ShuffleExtractor(), new VonNeumannExtractor() };
        foreach (Extractor module in modules)
        {
            module.Start();
        }
        Thread.Sleep(100);
        foreach (Extractor module in modules)
        {
            module.Dispose();
        }
        Thread.Sleep(500);
        Assert.IsTrue(!modules.Where(x => !x.Disposed).Any());
    }
}
