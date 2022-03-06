using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

// Source: AForge library
namespace TrulyRandom.DirectShow
{
    class VideoCapabilities
    {
        /// <summary>
        /// Frame size supported by video device
        /// </summary>
        public readonly Size FrameSize;

        /// <summary>
        /// Average frame rate of video device for corresponding <see cref="FrameSize">frame size</see>
        /// </summary>
        public readonly int AverageFrameRate;

        /// <summary>
        /// Maximum frame rate of video device for corresponding <see cref="FrameSize">frame size</see>
        /// </summary>
        public readonly int MaximumFrameRate;

        /// <summary>
        /// Number of bits per pixel provided by the camera
        /// </summary>
        public readonly int BitCount;

        internal VideoCapabilities() { }

        /// <summary>
        /// Retrieves capabilities of a video device
        /// </summary>
        /// <param name="videoStreamConfig"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        internal static VideoCapabilities[] FromStreamConfig(IAMStreamConfig_Video videoStreamConfig)
        {
            if (videoStreamConfig == null)
            {
                throw new ArgumentNullException(nameof(videoStreamConfig));
            }

            //Ensure this device reports capabilities
            int hr = videoStreamConfig.GetNumberOfCapabilities(out int count, out int size);

            if (hr != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            if (count <= 0)
            {
                throw new NotSupportedException("This video device does not report capabilities.");
            }

            if (size > Marshal.SizeOf(typeof(VideoStreamConfigCaps)))
            {
                throw new NotSupportedException("Unable to retrieve video device capabilities. This video device requires a larger VideoStreamConfigCaps structure.");
            }

            //Group capabilities with similar parameters
            Dictionary<uint, VideoCapabilities> videocapsList = new();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    VideoCapabilities vc = new(videoStreamConfig, i);

                    uint key = (((uint)vc.FrameSize.Height) << 32) |
                               (((uint)vc.FrameSize.Width) << 16);

                    if (!videocapsList.ContainsKey(key))
                    {
                        videocapsList.Add(key, vc);
                    }
                    else
                    {
                        if (vc.BitCount > videocapsList[key].BitCount)
                        {
                            videocapsList[key] = vc;
                        }
                    }
                }
                catch
                {
                }
            }

            VideoCapabilities[] videocaps = new VideoCapabilities[videocapsList.Count];
            videocapsList.Values.CopyTo(videocaps, 0);

            return videocaps;
        }

        /// <summary>
        /// Retrieves capabilities of a video device
        /// </summary>
        /// <param name="videoStreamConfig"></param>
        /// <param name="index"></param>
        /// <exception cref="ApplicationException"></exception>
        internal VideoCapabilities(IAMStreamConfig_Video videoStreamConfig, int index)
        {
            AMMediaType mediaType = null;
            VideoStreamConfigCaps caps = new();

            try
            {
                //Retrieve capabilities struct at the specified index
                int hr = videoStreamConfig.GetStreamCaps(index, out mediaType, caps);

                if (hr != 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                if (mediaType.FormatType == FormatType.VideoInfo)
                {
                    VideoInfoHeader videoInfo = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));

                    FrameSize = new Size(videoInfo.BmiHeader.Width, videoInfo.BmiHeader.Height);
                    BitCount = videoInfo.BmiHeader.BitCount;
                    AverageFrameRate = (int)(10000000 / videoInfo.AverageTimePerFrame);
                    MaximumFrameRate = (int)(10000000 / caps.MinFrameInterval);
                }
                else if (mediaType.FormatType == FormatType.VideoInfo2)
                {
                    VideoInfoHeader2 videoInfo = (VideoInfoHeader2)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader2));

                    FrameSize = new Size(videoInfo.BmiHeader.Width, videoInfo.BmiHeader.Height);
                    BitCount = videoInfo.BmiHeader.BitCount;
                    AverageFrameRate = (int)(10000000 / videoInfo.AverageTimePerFrame);
                    MaximumFrameRate = (int)(10000000 / caps.MinFrameInterval);
                }
                else
                {
                    throw new ApplicationException("Unsupported format found.");
                }

                // Ignore 12 bpp formats, since it was noticed they cause issues on Windows 8
                if (BitCount <= 12)
                {
                    throw new ApplicationException("Unsupported format found.");
                }
            }
            finally
            {
                if (mediaType != null)
                {
                    mediaType.Dispose();
                }
            }
        }

        /// <summary>
        /// Check if the video capability equals to the specified object
        /// </summary>
        /// <param name="obj">Object to compare with</param>
        /// <returns>Returns true if both are equal are equal or false otherwise</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as VideoCapabilities);
        }

        /// <summary>
        /// Check if two video capabilities are equal
        /// </summary>
        /// <param name="vc2">Second video capability to compare with</param>
        /// <returns>Returns true if both video capabilities are equal or false otherwise</returns>
        public bool Equals(VideoCapabilities vc2)
        {
            if (vc2 is null)
            {
                return false;
            }

            return ((FrameSize == vc2.FrameSize) && (BitCount == vc2.BitCount));
        }

        /// <summary>
        /// Get hash code of the object
        /// </summary>
        /// <returns>Returns hash code ot the object</returns>
        public override int GetHashCode()
        {
            return FrameSize.GetHashCode() ^ BitCount;
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="a">First object to check</param>
        /// <param name="b">Seconds object to check</param>
        /// <returns>Return true if both objects are equal or false otherwise</returns>
        public static bool operator ==(VideoCapabilities a, VideoCapabilities b)
        {
            //If both are null, or both are same instance, return true
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            //If one is null, but not both, return false
            if ((a is null) || (b is null))
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="a">First object to check</param>
        /// <param name="b">Seconds object to check</param>
        /// <returns>Return true if both objects are not equal or false otherwise</returns>
        public static bool operator !=(VideoCapabilities a, VideoCapabilities b)
        {
            return !(a == b);
        }
    }
}
