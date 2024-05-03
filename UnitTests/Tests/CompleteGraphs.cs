using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using TrulyRandom.Modules;
using TrulyRandom.Modules.Extractors;
using TrulyRandom.Modules.Sources;
using TrulyRandom.Modules.Sources.Prng;

namespace UnitTests;

/// <summary>
/// Evaluates the functionality of the complete graphs with the different kinds of sources. May take a long time to run
/// </summary>
[TestClass]
public class CompleteGraphs
{
    [TestMethod, TestCategory("RequiresHardware")]
    public void Video()
    {
        VideoSource source = new();
        DeflateExtractor deflateExtractor = new();
        deflateExtractor.AddSource(source);
        ShuffleExtractor shuffleExtractor = new();
        shuffleExtractor.AddSource(deflateExtractor);
        HashExtractor hashExtractor = new();
        hashExtractor.AddSource(shuffleExtractor);
        Tester tester = new();
        tester.AddSource(hashExtractor);
        Buffer buffer = new();
        buffer.AddSource(tester);

        deflateExtractor.DynamicCoefficientSource = buffer;
        shuffleExtractor.DynamicCoefficientSource = buffer;
        hashExtractor.DynamicCoefficientSource = buffer;

        shuffleExtractor.SeedSource = buffer;
        hashExtractor.SeedSource = buffer;

        source.Start();
        deflateExtractor.Start();
        shuffleExtractor.Start();
        hashExtractor.Start();
        tester.Start();
        buffer.Start();

        while (!buffer.Overflow)
        {
            Thread.Sleep(100);
        }

        source.Dispose();
        deflateExtractor.Dispose();
        shuffleExtractor.Dispose();
        hashExtractor.Dispose();
        tester.Dispose();
        buffer.Dispose();

        Assert.IsTrue(tester.SuccessRate > 0.3);
    }

    [TestMethod, TestCategory("RequiresHardware")]
    public void Audio()
    {
        AudioSource source = new();
        DeflateExtractor deflateExtractor = new();
        deflateExtractor.AddSource(source);
        ShuffleExtractor shuffleExtractor = new();
        shuffleExtractor.AddSource(deflateExtractor);
        HashExtractor hashExtractor = new();
        hashExtractor.AddSource(shuffleExtractor);
        Tester tester = new();
        tester.AddSource(hashExtractor);
        TrulyRandom.Modules.Buffer buffer = new();
        buffer.AddSource(tester);

        deflateExtractor.DynamicCoefficientSource = buffer;
        shuffleExtractor.DynamicCoefficientSource = buffer;
        hashExtractor.DynamicCoefficientSource = buffer;

        shuffleExtractor.SeedSource = buffer;
        hashExtractor.SeedSource = buffer;

        source.Start();
        deflateExtractor.Start();
        shuffleExtractor.Start();
        hashExtractor.Start();
        tester.Start();
        buffer.Start();

        while (!buffer.Overflow)
        {
            Thread.Sleep(100);
        }

        source.Dispose();
        deflateExtractor.Dispose();
        shuffleExtractor.Dispose();
        hashExtractor.Dispose();
        tester.Dispose();
        buffer.Dispose();

        Assert.IsTrue(tester.SuccessRate > 0.3);
    }

    [TestMethod, TestCategory("RequiresHardware")]
    public void Biological()
    {
        BiologicalSource source = new();
        DeflateExtractor deflateExtractor = new();
        deflateExtractor.AddSource(source);
        ShuffleExtractor shuffleExtractor = new();
        shuffleExtractor.BatchSize = 10000;
        shuffleExtractor.BlockSize = 100;
        shuffleExtractor.AddSource(deflateExtractor);
        HashExtractor hashExtractor = new();
        hashExtractor.AddSource(shuffleExtractor);
        Tester tester = new();
        tester.BatchSize = 1000;
        tester.AddSource(hashExtractor);
        Buffer buffer = new();
        buffer.BufferSize = 100000;
        buffer.AddSource(tester);

        deflateExtractor.DynamicCoefficientSource = buffer;
        shuffleExtractor.DynamicCoefficientSource = buffer;
        hashExtractor.DynamicCoefficientSource = buffer;

        shuffleExtractor.SeedSource = buffer;
        hashExtractor.SeedSource = buffer;
        shuffleExtractor.SeedRotationInterval = 10000;
        hashExtractor.SeedRotationInterval = 10000;

        source.Start();
        deflateExtractor.Start();
        shuffleExtractor.Start();
        hashExtractor.Start();
        tester.Start();
        buffer.Start();

        while (!buffer.Overflow)
        {
            Thread.Sleep(100);
        }

        source.Dispose();
        deflateExtractor.Dispose();
        shuffleExtractor.Dispose();
        hashExtractor.Dispose();
        tester.Dispose();
        buffer.Dispose();

        Assert.IsTrue(tester.SuccessRate > 0.3);
    }

    [TestMethod, TestCategory("RequiresHardware")]
    public void All()
    {
        AudioSource audioSource = new();
        VideoSource videoSource = new();
        BiologicalSource biologicalSource = new();

        DeflateExtractor deflateExtractor = new();
        deflateExtractor.AddSource(audioSource);
        deflateExtractor.AddSource(videoSource);
        deflateExtractor.AddSource(biologicalSource);
        ShuffleExtractor shuffleExtractor = new();
        shuffleExtractor.AddSource(deflateExtractor);
        HashExtractor hashExtractor = new();
        hashExtractor.AddSource(shuffleExtractor);
        Tester tester = new();
        tester.AddSource(hashExtractor);
        Buffer buffer = new();
        buffer.AddSource(tester);

        deflateExtractor.DynamicCoefficientSource = buffer;
        shuffleExtractor.DynamicCoefficientSource = buffer;
        hashExtractor.DynamicCoefficientSource = buffer;

        shuffleExtractor.SeedSource = buffer;
        hashExtractor.SeedSource = buffer;

        audioSource.Start();
        videoSource.Start();
        biologicalSource.Start();
        deflateExtractor.Start();
        shuffleExtractor.Start();
        hashExtractor.Start();
        tester.Start();
        buffer.Start();

        while (!buffer.Overflow)
        {
            Thread.Sleep(100);
        }

        audioSource.Dispose();
        videoSource.Dispose();
        biologicalSource.Dispose();
        deflateExtractor.Dispose();
        shuffleExtractor.Dispose();
        hashExtractor.Dispose();
        tester.Dispose();
        buffer.Dispose();

        Assert.IsTrue(tester.SuccessRate > 0.3);
    }

#pragma warning disable CS0618 // Type or member is obsolete
    [TestMethod, TestCategory("RequiresHardware")]
    public void Prng()
    {
        LcgPrngSource lcgSource = new();
        XorshiftPrngSource xorshiftSource = new();
        MtPrngSource mtSource = new();

        DeflateExtractor deflateExtractor = new();
        deflateExtractor.AddSource(lcgSource);
        deflateExtractor.AddSource(xorshiftSource);
        deflateExtractor.AddSource(mtSource);
        ShuffleExtractor shuffleExtractor = new();
        shuffleExtractor.AddSource(deflateExtractor);
        HashExtractor hashExtractor = new();
        hashExtractor.AddSource(shuffleExtractor);
        Tester tester = new();
        tester.AddSource(hashExtractor);
        Buffer buffer = new();
        buffer.AddSource(tester);

        deflateExtractor.DynamicCoefficientSource = buffer;
        shuffleExtractor.DynamicCoefficientSource = buffer;
        hashExtractor.DynamicCoefficientSource = buffer;

        shuffleExtractor.SeedSource = buffer;
        hashExtractor.SeedSource = buffer;

        lcgSource.Start();
        xorshiftSource.Start();
        mtSource.Start();
        deflateExtractor.Start();
        shuffleExtractor.Start();
        hashExtractor.Start();
        tester.Start();
        buffer.Start();

        while (!buffer.Overflow)
        {
            Thread.Sleep(100);
        }

        lcgSource.Dispose();
        xorshiftSource.Dispose();
        mtSource.Dispose();
        deflateExtractor.Dispose();
        shuffleExtractor.Dispose();
        hashExtractor.Dispose();
        tester.Dispose();
        buffer.Dispose();

        Assert.IsTrue(tester.SuccessRate > 0.3);
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
