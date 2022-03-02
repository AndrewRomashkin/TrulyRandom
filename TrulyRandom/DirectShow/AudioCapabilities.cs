using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// Source: AForge library
namespace TrulyRandom.DirectShow
{
    class AudioCapabilities
    {
        /// <summary>
        /// Average frame rate of video device for corresponding <see cref="FrameSize">frame size</see>.
        /// </summary>
        public readonly int Channels;
        public readonly int SamplesPerSec;
        public readonly int AvgBytesPerSec;
        public readonly int BlockAlign;
        public readonly int BitsPerSample;

        internal AudioCapabilities() { }

        // Retrieve capabilities of a audio device
        internal static AudioCapabilities[] FromStreamConfig(IAMStreamConfig_Audio audioStreamConfig)
        {
            if (audioStreamConfig == null)
            {
                throw new ArgumentNullException("audioStreamConfig");
            }

            // ensure this device reports capabilities
            int count, size;
            int hr = audioStreamConfig.GetNumberOfCapabilities(out count, out size);

            if (hr != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            if (count <= 0)
            {
                throw new NotSupportedException("This audio device does not report capabilities.");
            }

            if (size > Marshal.SizeOf(typeof(AudioStreamConfigCaps)))
            {
                throw new NotSupportedException("Unable to retrieve audio device capabilities. This audio device requires a larger AudioStreamConfigCaps structure.");
            }

            // group capabilities with similar parameters
            Dictionary<int, AudioCapabilities> audiocapsList = new Dictionary<int, AudioCapabilities>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    AudioCapabilities ac = new AudioCapabilities(audioStreamConfig, i);

                    int key = ac.Channels + ac.BlockAlign + ac.BitsPerSample + ac.AvgBytesPerSec + ac.SamplesPerSec;

                    if (!audiocapsList.ContainsKey(key))
                    {
                        audiocapsList.Add(key, ac);
                    }
                    else
                    {
                        if (ac.BitsPerSample > audiocapsList[key].BitsPerSample)
                        {
                            audiocapsList[key] = ac;
                        }
                    }
                }
                catch
                {
                }
            }

            AudioCapabilities[] audiocaps = new AudioCapabilities[audiocapsList.Count];
            audiocapsList.Values.CopyTo(audiocaps, 0);

            return audiocaps;
        }

        // Retrieve capabilities of a audio device
        internal AudioCapabilities(IAMStreamConfig_Audio audioStreamConfig, int index)
        {
            AMMediaType mediaType = null;
            AudioStreamConfigCaps caps = new AudioStreamConfigCaps();

            try
            {
                // retrieve capabilities struct at the specified index
                int hr = audioStreamConfig.GetStreamCaps(index, out mediaType, caps);

                if (hr != 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                if (mediaType.FormatType == FormatType.WaveFormatEx)
                {
                    WaveFormatEx audioInfo = (WaveFormatEx)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(WaveFormatEx));

                    Channels = audioInfo.nChannels;
                    SamplesPerSec = audioInfo.nSamplesPerSec;
                    AvgBytesPerSec = audioInfo.nAvgBytesPerSec;
                    BlockAlign = audioInfo.nBlockAlign;
                    BitsPerSample = audioInfo.wBitsPerSample;
                }
                else
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
        /// Check if the audio capability equals to the specified object.
        /// </summary>
        /// 
        /// <param name="obj">Object to compare with.</param>
        /// 
        /// <returns>Returns true if both are equal are equal or false otherwise.</returns>
        /// 
        public override bool Equals(object obj)
        {
            return Equals(obj as AudioCapabilities);
        }

        /// <summary>
        /// Check if two audio capabilities are equal.
        /// </summary>
        /// 
        /// <param name="vc2">Second audio capability to compare with.</param>
        /// 
        /// <returns>Returns true if both audio capabilities are equal or false otherwise.</returns>
        /// 
        public bool Equals(AudioCapabilities vc2)
        {
            if (vc2 == null)
            {
                return false;
            }

            return (AvgBytesPerSec == vc2.AvgBytesPerSec) && (BitsPerSample == vc2.BitsPerSample) && (BlockAlign == vc2.BlockAlign) && (Channels == vc2.Channels) && (SamplesPerSec == vc2.SamplesPerSec);
        }

        /// <summary>
        /// Get hash code of the object.
        /// </summary>
        /// 
        /// <returns>Returns hash code ot the object </returns>
        public override int GetHashCode()
        {
            return AvgBytesPerSec.GetHashCode() ^ BitsPerSample.GetHashCode() ^ BlockAlign.GetHashCode() ^ Channels.GetHashCode() ^ SamplesPerSec.GetHashCode();
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// 
        /// <param name="a">First object to check.</param>
        /// <param name="b">Seconds object to check.</param>
        /// 
        /// <returns>Return true if both objects are equal or false otherwise.</returns>
        public static bool operator ==(AudioCapabilities a, AudioCapabilities b)
        {
            // if both are null, or both are same instance, return true.
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            // if one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// 
        /// <param name="a">First object to check.</param>
        /// <param name="b">Seconds object to check.</param>
        /// 
        /// <returns>Return true if both objects are not equal or false otherwise.</returns>
        public static bool operator !=(AudioCapabilities a, AudioCapabilities b)
        {
            return !(a == b);
        }
    }
}
