using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using TrulyRandom.Models;
using TrulyRandom.Modules.Sources;

namespace UnitTests
{
    /// <summary>
    /// Evaluates the functionality of different sources
    /// </summary>
    [TestClass]
    public class Sources
    {
        DateTime testStart;
        [TestInitialize()]
        public void TestInitialize()
        {
            testStart = DateTime.Now;
        }

        [TestMethod,TestCategory("RequiresHardware")]
        public void Video()
        {
            VideoSource source = new();
            source.CalculateEntropy = true;
            source.Start();
            source.BufferSize = 100_000_000;  //To ensure buffer won't overflow after 2 frames
            bool success = false;
            while (!testStart.WasAgo(TimeSpan.FromSeconds(100)))
            {
                if (source.BytesInBuffer > 1_000_000 && source.Device.SamplesRecieved >= 3 && source.Entropy > 0.3)
                {
                    success = true;
                    break;
                }
                Thread.Sleep(100);
            }
            source.Dispose();
            Assert.IsTrue(success);
            Assert.IsFalse(source.Still);
        }

        [TestMethod, TestCategory("RequiresHardware")]
        public void Audio()
        {
            AudioSource source = new();
            source.CalculateEntropy = true;
            source.Start();
            bool success = false;
            while (!testStart.WasAgo(TimeSpan.FromSeconds(10)))
            {
                if (source.BytesInBuffer > 1_000_000 && source.Device.SamplesRecieved >= 3 && source.Entropy > 0.08)
                {
                    success = true;
                    break;
                }
                Thread.Sleep(100);
            }
            source.Dispose();
            Assert.IsTrue(success);
            Assert.IsFalse(source.Still);
        }

        [TestMethod, TestCategory("RequiresHardware")]
        [Description("You need to wiggle your mouse and/or press keyboard keys while this test is running!!!")]
        public void Biological()
        {
            BiologicalSource source = new();
            source.Start();
            source.CalculateEntropy = true;
            bool success = false;
            while (!testStart.WasAgo(TimeSpan.FromSeconds(30)))
            {
                if (source.BytesInBuffer > 1_000 && source.Entropy > 0.3)
                {
                    success = true;
                    break;
                }
                Thread.Sleep(100);
            }
            source.Dispose();
            Assert.IsTrue(success);
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
            TrulyRandom.Devices.DeviceDescriptor deviceDescriptor = VideoSource.AvailableDevices[0];
            VideoSource source = new(deviceDescriptor);
            VideoSource source1;
            try
            {
                source1 = new VideoSource(deviceDescriptor);
            }
            catch
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
    }
}
