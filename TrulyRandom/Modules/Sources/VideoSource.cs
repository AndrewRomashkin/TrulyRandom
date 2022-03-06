using System;
using TrulyRandom.Devices;
using TrulyRandom.Models;

namespace TrulyRandom.Modules.Sources
{
    /// <summary>
    /// Recieves data from the video device
    /// </summary>
    public class VideoSource : Source
    {
        /// <summary>
        /// Underlying DirectShow device
        /// </summary>
        public VideoDevice Device { get; protected set; } = null;
        /// <summary>
        /// Shows whether device is running
        /// </summary>
        public bool Running => Device.Running;
        /// <summary>
        /// Shows whether device is disposed
        /// </summary>
        public override bool Disposed => Device.Disposed;
        /// <summary>
        /// Shows whether device is silent (provides the same sample over and over)
        /// </summary>
        public bool Still => Device.Still;
        /// <summary>
        /// System name of the device
        /// </summary>
        public string DeviceName => Device.Name;

        void Setup()
        {
            overflowHysteresis = 0.5;
            BufferSize = 100 * 1024 * 1024;
        }

        /// <summary>
        /// Creates new <see cref="VideoSource"/> object by searching through available devices and attempting to get alternating samples from those
        /// </summary>
        /// <exception cref="DeviceNotFoundException">Thrown when no suitable devices found. If device is found and then goes offline object will keep trying to establish connection and no exception will be thrown</exception>
        /// <param name="autoSearchTimeout">Timeout for waiting for data from the device</param>
        public VideoSource(int autoSearchTimeout = 5000)
        {
            Setup();
            Device = new VideoDevice(autoSearchTimeout);
            Device.NewData += Device_NewData;
        }

        /// <summary>
        /// Creates new <see cref="VideoSource"/> object from given <see cref="DeviceDescriptor"/>. If device is unavailable object will keep trying to establish connection and no exception will be thrown
        /// </summary>
        public VideoSource(DeviceDescriptor deviceDescriptor)
        {
            Setup();
            Device = new VideoDevice(deviceDescriptor);
            Device.NewData += Device_NewData;
        }

        /// <summary>
        /// Creates new <see cref="VideoSource"/> object from given name and moniker string. If device is unavailable object will keep trying to establish connection and no exception will be thrown
        /// </summary>
        public VideoSource(string name, string monikerString)
        {
            Setup();
            Device = new VideoDevice(name, monikerString);
            Device.NewData += Device_NewData;
        }

        /// <summary>
        /// Returns all video device descriptors currently available in the system
        /// </summary>
        public static DeviceDescriptor[] AvailableDevices => VideoDevice.AvailableDevices;

        ///<inheritdoc/>
        protected override void BPSCalculation()
        {
            BytesPerSecond = Device == null ? 0 : Device.ActualBytesPerSecond;
        }

        void Device_NewData(byte[] data)
        {
            AddData(data);
        }

        ///<inheritdoc/>
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            base.Dispose();
            Device.Dispose();
        }

        ///<inheritdoc/>
        protected override void StartInternal()
        {
            base.StartInternal();
            Device.Start();
        }

        ///<inheritdoc/>
        protected override void StopInternal()
        {
            base.StopInternal();
            Device.Stop();
        }
    }
}
