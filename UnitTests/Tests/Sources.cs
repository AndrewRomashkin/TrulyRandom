using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using TrulyRandom.Models;
using TrulyRandom.Modules.Sources;

namespace UnitTests
{
    [TestClass]
    public class Sources
    {
        DateTime testStart;
        [TestInitialize()]
        public void TestInitialize()
        {
            testStart = DateTime.Now;
        }

        [TestMethod]
        public void Video()
        {
            VideoSource source = new VideoSource();
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
            Assert.IsTrue(!source.Still);
        }

        [TestMethod]
        public void Audio()
        {
            AudioSource source = new AudioSource();
            source.Start();
            bool success = false;
            while (!testStart.WasAgo(TimeSpan.FromSeconds(10)))
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
            Assert.IsTrue(!source.Still);
        }

        [TestMethod]
        [Description("You need to wiggle your mouse and/or press keyboard keys while this test is running!!!")]
        public void Biological()
        {
            BiologicalSource source = new BiologicalSource();
            source.Start();
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

        [TestMethod]
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
            Assert.IsTrue(!modules.Where(x => !x.Disposed).Any());
        }

        [TestMethod]
        public void DeviceLocking()
        {
            bool success = false;
            TrulyRandom.Devices.DeviceDescriptor deviceDescriptor = VideoSource.AvailableDevices[0];
            VideoSource source = new VideoSource(deviceDescriptor);
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
