using System;
using System.Linq;
using TrulyRandom.DirectShow;

namespace TrulyRandom.Devices
{
    /// <summary>
    /// Represents an audio Direct Show device
    /// </summary>
    public class AudioDevice : DirectShowDevice
    {
        AudioCapabilities capabilities = null;

        /// <summary>
        /// Bitrate reported by device
        /// </summary>
        public int? AvgBytesPerSecond => capabilities?.AvgBytesPerSec;

        /// <summary>
        /// Bits per sample reported by device
        /// </summary>
        public int? BitsPerSample => capabilities?.BitsPerSample;


        /// <summary>
        /// Amount of channels on this device
        /// </summary>
        public int? Channels => capabilities?.Channels;

        /// <summary>
        /// Samples per second reported by device
        /// </summary>
        public int? SamplesPerSecond => capabilities?.SamplesPerSec;

        /// <summary>
        /// Creates new <see cref="AudioDevice"/> object by searching through available audio devices and attempting to get alternating samples from those
        /// </summary>
        /// <exception cref="DeviceNotFoundException">Thrown when no suitable devices found. If device is found and then goes offline object will keep trying to establish connection and no exception will be thrown</exception>
        /// <param name="autoSearchTimeout">Timeout for waiting for data from the device</param>
        public AudioDevice(int autoSearchTimeout = 5000) : base(autoSearchTimeout) { }
        /// <summary>
        /// Creates new <see cref="AudioDevice"/> object from given <c>DeviceDescriptor</c>. If device is unavailable object will keep trying to establish connection and no exception will be thrown
        /// </summary>
        public AudioDevice(DeviceDescriptor deviceDescriptor) : base(deviceDescriptor) { }
        /// <summary>
        /// Creates new <see cref="AudioDevice"/> object from given name and moniker string. If device is unavailable object will keep trying to establish connection and no exception will be thrown
        /// </summary>
        public AudioDevice(string name, string monikerString) : base(name, monikerString) { }

        /// <summary>
        /// Returns all audio device descriptors currently available in the system
        /// </summary>
        public static DeviceDescriptor[] AvailableDevices => GetAvailableDevices(Clsid.AudioInputDevice);

        static readonly object autoSearchLock = new();

        /// <summary>
        /// Returns a static lock object for all instances of this type
        /// </summary>
        protected override object GetTypeSync()
        {
            return autoSearchLock;
        }

        /// <summary>
        /// Finds first operational audio device. If none found returns <c>null</c>
        /// </summary>
        /// <param name="timeout">Timeout for waiting for data from the device</param>
        /// <returns>Descriptor of the first operational device found</returns>
        public static DeviceDescriptor FindFirstOperationalDevice(int timeout = 5000)
        {
            lock (autoSearchLock)
            {
                return FindFirstOperationalDevice(timeout, typeof(AudioDevice), MediaType.Audio);
            }
        }

        /// <summary>
        /// Returns unique thread name
        /// </summary>
        /// <returns>Unique thread name</returns>
        private protected override string GetThreadName()
        {
            return "Audio device thread: " + Name;
        }

        //Members specific for audio devices:

        private protected override AMMediaType GetAMMediaType()
        {
            AMMediaType mediaType = new();
            mediaType.MajorType = MediaType.Audio;

            return mediaType;
        }

        private protected override Guid GetMediaType()
        {
            return MediaType.Audio;
        }

        private protected override Guid GetDeviceType()
        {
            return Clsid.AudioInputDevice;
        }

        private protected override void GetAndConfigurePinCapabilities(ICaptureGraphBuilder2 graphBuilder, IBaseFilter baseFilter)
        {
            graphBuilder.FindInterface(PinCategory.Capture, MediaType.Audio, baseFilter, typeof(IAMStreamConfig_Audio).GUID, out object streamConfigObject);

            if (streamConfigObject != null)
            {
                IAMStreamConfig_Audio streamConfig = null;

                try
                {
                    streamConfig = (IAMStreamConfig_Audio)streamConfigObject;
                }
                catch (InvalidCastException)
                {
                }

                AudioCapabilities[] audioCapabilities = Array.Empty<AudioCapabilities>();
                if (streamConfig != null)
                {
                    try
                    {
                        // get all capabilities
                        audioCapabilities = AudioCapabilities.FromStreamConfig(streamConfig);
                    }
                    catch
                    {
                    }
                }
                if (audioCapabilities.Length > 0)
                {
                    AudioCapabilities audioCapability = audioCapabilities.OrderByDescending(x => x.Channels * x.AvgBytesPerSec).First();
                    SetCapabilities(streamConfig, audioCapability);
                    capabilities = audioCapability;
                }
            }
            else
            {
                capabilities = null;
            }
        }

        private static void SetCapabilities(IAMStreamConfig_Audio streamConfig, AudioCapabilities capabilities)
        {
            if (capabilities == null)
            {
                return;
            }

            AMMediaType newMediaType = null;
            AudioStreamConfigCaps caps = new();

            // iterate through device's capabilities to find mediaType for desired capabilities
            streamConfig.GetNumberOfCapabilities(out int capabilitiesCount, out _);

            for (int i = 0; i < capabilitiesCount; i++)
            {
                try
                {
                    AudioCapabilities ac = new(streamConfig, i);

                    if (capabilities == ac)
                    {
                        if (streamConfig.GetStreamCaps(i, out newMediaType, caps) == 0)
                        {
                            break;
                        }
                    }
                }
                catch
                {
                }
            }

            // set the new format
            if (newMediaType != null)
            {
                streamConfig.SetFormat(newMediaType);
                newMediaType.Dispose();
            }
        }
    }
}