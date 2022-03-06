using System;
using System.Drawing;
using System.Linq;
using TrulyRandom.DirectShow;

namespace TrulyRandom.Devices
{
    /// <summary>
    /// Represents a video Direct Show device
    /// </summary>
    public class VideoDevice : DirectShowDevice
    {
        VideoCapabilities capabilities = null;

        /// <summary>
        /// Resolution reported by device
        /// </summary>
        public Size? Resolution => capabilities?.FrameSize;

        /// <summary>
        /// FPS reported by device
        /// </summary>
        public int? FPS => capabilities?.AverageFrameRate;

        /// <summary>
        /// Bit depth reported by device
        /// </summary>
        public int? BitDepth => capabilities?.BitCount;

        /// <summary>
        /// Creates new <see cref="VideoDevice"/> object by searching through available video devices and attempting to get alternating samples from those
        /// </summary>
        /// <exception cref="DeviceNotFoundException">Thrown when no suitable devices found. If device is found and then goes offline object will keep trying to establish connection and no exception will be thrown</exception>
        /// <param name="autoSearchTimeout">Timeout for waiting for data from the device</param>
        public VideoDevice(int autoSearchTimeout = 5000) : base(autoSearchTimeout) { }
        /// <summary>
        /// Creates new <see cref="VideoDevice"/> object from given <c>DeviceDescriptor</c>. If device is unavailable object will keep trying to establish connection and no exception will be thrown
        /// </summary>
        public VideoDevice(DeviceDescriptor deviceDescriptor) : base(deviceDescriptor) { }
        /// <summary>
        /// Creates new <see cref="VideoDevice"/> object from given name and moniker string. If device is unavailable object will keep trying to establish connection and no exception will be thrown
        /// </summary>
        public VideoDevice(string name, string monikerString) : base(name, monikerString) { }

        /// <summary>
        /// Returns all audio device descriptors currently available in the system
        /// </summary>
        public static DeviceDescriptor[] AvailableDevices => GetAvailableDevices(Clsid.VideoInputDevice);

        static readonly object autoSearchLock = new();

        /// <summary>
        /// Returns a static lock object for all instances of this type
        /// </summary>
        protected override object GetTypeSync()
        {
            return autoSearchLock;
        }

        /// <summary>
        /// Finds first operational video device. If none found returns <c>null</c>
        /// </summary>
        /// <param name="timeout">Timeout for waiting for data from the device</param>
        /// <returns>Descriptor of the first operational device found</returns>
        public static DeviceDescriptor FindFirstOperationalDevice(int timeout = 5000)
        {
            lock (autoSearchLock)
            {
                return FindFirstOperationalDevice(timeout, typeof(VideoDevice), MediaType.Video);
            }
        }

        /// <summary>
        /// Returns unique thread name
        /// </summary>
        /// <returns>Unique thread name</returns>
        private protected override string GetThreadName()
        {
            return "Video device thread: " + Name;
        }

        //Members specific for video devices:

        private protected override AMMediaType GetAMMediaType()
        {
            AMMediaType mediaType = new();
            mediaType.MajorType = MediaType.Video;
            mediaType.SubType = MediaSubType.RGB24;

            return mediaType;
        }

        private protected override Guid GetMediaType()
        {
            return MediaType.Video;
        }

        private protected override Guid GetDeviceType()
        {
            return Clsid.VideoInputDevice;
        }

        private protected override void GetAndConfigurePinCapabilities(ICaptureGraphBuilder2 graphBuilder, IBaseFilter baseFilter)
        {
            graphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, baseFilter, typeof(IAMStreamConfig_Video).GUID, out object streamConfigObject);

            if (streamConfigObject != null)
            {
                IAMStreamConfig_Video streamConfig = null;

                try
                {
                    streamConfig = (IAMStreamConfig_Video)streamConfigObject;
                }
                catch (InvalidCastException)
                {
                }

                VideoCapabilities[] videoCapabilities = Array.Empty<VideoCapabilities>();
                if (streamConfig != null)
                {
                    try
                    {
                        // get all capabilities
                        videoCapabilities = VideoCapabilities.FromStreamConfig(streamConfig);
                    }
                    catch
                    {
                    }
                }
                if (videoCapabilities.Length > 0)
                {
                    VideoCapabilities videoCapability = videoCapabilities.OrderByDescending(x => x.AverageFrameRate * x.BitCount * x.FrameSize.Width * x.FrameSize.Height).First();
                    SetCapabilities(streamConfig, videoCapability);
                    capabilities = videoCapability;
                }
            }
            else
            {
                capabilities = null;
            }
        }

        private static void SetCapabilities(IAMStreamConfig_Video streamConfig, VideoCapabilities capabilities)
        {
            if (capabilities == null)
            {
                return;
            }

            AMMediaType newMediaType = null;
            VideoStreamConfigCaps caps = new();

            // iterate through device's capabilities to find mediaType for desired resolution
            streamConfig.GetNumberOfCapabilities(out int capabilitiesCount, out _);

            for (int i = 0; i < capabilitiesCount; i++)
            {
                try
                {
                    VideoCapabilities vc = new(streamConfig, i);

                    if (capabilities == vc)
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