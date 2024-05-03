using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using TrulyRandom.Models;
using TrulyRandom.Modules.Sources;
using TrulyRandom.Modules.Sources.Prng;

namespace UnitTests;

/// <summary>
/// Evaluates the functionality of different sources
/// </summary>
[TestClass]
public class Sources
{
    private DateTime testStart;
    [TestInitialize()]
    public void TestInitialize()
    {
        testStart = DateTime.Now;
    }

    [TestMethod, TestCategory("RequiresHardware")]
    public void Video()
    {
        VideoSource source = new();
        source.CalculateEntropy = true;
        source.Start();
        source.BufferSize = 100_000_000;  //To ensure buffer won't overflow after 2 frames
        bool enoughBytes = false;
        while (!testStart.WasAgo(TimeSpan.FromSeconds(100)))
        {
            if (source.BytesInBuffer > 1_000_000 && source.Device.SamplesRecieved >= 3)
            {
                enoughBytes = true;
                source.ForceRefreshEntropy();
                break;
            }
            Thread.Sleep(100);
        }
        Assert.IsTrue(enoughBytes);
        Assert.IsTrue(source.Entropy > 0.3);
        Assert.IsFalse(source.Still);
        source.Dispose();
    }

    [TestMethod, TestCategory("RequiresHardware")]
    public void Audio()
    {
        AudioSource source = new();
        source.CalculateEntropy = true;
        source.Start();
        bool enoughBytes = false;
        while (!testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            if (source.BytesInBuffer > 1_000_000 && source.Device.SamplesRecieved >= 3)
            {
                enoughBytes = true;
                source.ForceRefreshEntropy();
                break;
            }
            Thread.Sleep(100);
        }
        Assert.IsTrue(enoughBytes);
        Assert.IsTrue(source.Entropy > 0.08);
        Assert.IsFalse(source.Still);
        source.Dispose();
    }

    [TestMethod, TestCategory("RequiresHardware")]
    [Description("You need to wiggle your mouse and/or press keyboard keys while this test is running!!!")]
    public void Biological()
    {
        BiologicalSource source = new();
        source.Start();
        source.CalculateEntropy = true;
        bool enoughBytes = false;
        while (!testStart.WasAgo(TimeSpan.FromSeconds(30)))
        {
            if (source.BytesInBuffer > 1_000)
            {
                enoughBytes = true;
                source.ForceRefreshEntropy();
                break;
            }
            Thread.Sleep(100);
        }
        Assert.IsTrue(enoughBytes);
        Assert.IsTrue(source.Entropy > 0.3);
        source.Dispose();
    }

    [TestMethod, TestCategory("RequiresHardware")]
    public void Dispose()
    {
        Source[] modules = { new VideoSource(), new AudioSource(), new BiologicalSource() };
        foreach (Source module in modules)
        {
            module.Start();
        }
        Thread.Sleep(1000);
        foreach (Source module in modules)
        {
            module.Dispose();
        }
        Thread.Sleep(500);
        Assert.IsFalse(modules.Where(x => !x.Disposed).Any());
    }

    [TestMethod, TestCategory("RequiresHardware")]
    public void DeviceLocking()
    {
        bool success = false;
        TrulyRandom.Devices.DeviceDescriptor deviceDescriptor = VideoSource.AvailableDevices.First();
        VideoSource source = new(deviceDescriptor);
        VideoSource source1;
        try
        {
            source1 = new VideoSource(deviceDescriptor);
            source1.Dispose();
        }
        catch (InvalidOperationException)
        {
            success = true;
        }

        source.Dispose();
        while (!source.Disposed)
        {
            Thread.Sleep(10);
        }
        source1 = new VideoSource(deviceDescriptor);
        source1.Dispose();
        Assert.IsTrue(success);
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private void TestPrngSource(PrngSource source)
    {
        source.CalculateEntropy = true;
        source.Start();
        bool enoughBytes = false;
        while (!testStart.WasAgo(TimeSpan.FromSeconds(10)))
        {
            if (source.BytesInBuffer > 1_000_000)
            {
                enoughBytes = true;
                source.ForceRefreshEntropy();
                break;
            }
            Thread.Sleep(10);
        }
        Assert.IsTrue(enoughBytes);
        Assert.IsTrue(source.Entropy > 0.99);
        source.Dispose();
    }

    [TestMethod]
    public void Lcg()
    {
        TestPrngSource(new LcgPrngSource());
    }

    [TestMethod]
    public void Xorshift()
    {
        TestPrngSource(new XorshiftPrngSource());
    }

    [TestMethod]
    public void Mt()
    {
        TestPrngSource(new MtPrngSource());
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
