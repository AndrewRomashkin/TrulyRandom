using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// Source: AForge library
namespace TrulyRandom.DirectShow;

internal class AudioCapabilities
{
    public readonly int Channels;
    public readonly int SamplesPerSec;
    public readonly int AvgBytesPerSec;
    public readonly int BlockAlign;
    public readonly int BitsPerSample;

    internal AudioCapabilities() { }

    /// <summary>
    /// Retrieves capabilities of an audio device.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    internal static AudioCapabilities[] FromStreamConfig(IAMStreamConfig_Audio audioStreamConfig)
    {
        if (audioStreamConfig == null)
        {
            throw new ArgumentNullException(nameof(audioStreamConfig));
        }

        //Ensure this device reports capabilities
        int hr = audioStreamConfig.GetNumberOfCapabilities(out int count, out int size);

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

        //Group capabilities with similar parameters
        Dictionary<int, AudioCapabilities> audiocapsList = new();

        for (int i = 0; i < count; i++)
        {
            AudioCapabilities ac = new(audioStreamConfig, i);

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

        AudioCapabilities[] audiocaps = new AudioCapabilities[audiocapsList.Count];
        audiocapsList.Values.CopyTo(audiocaps, 0);

        return audiocaps;
    }

    /// <summary>
    /// Retrieves capabilities of an audio device.
    /// </summary>
    /// <exception cref="ApplicationException"></exception>
    internal AudioCapabilities(IAMStreamConfig_Audio audioStreamConfig, int index)
    {
        AMMediaType mediaType = null;
        AudioStreamConfigCaps caps = new();

        try
        {
            //Retrieve capabilities struct at the specified index
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
    /// <param name="obj">Object to compare with</param>
    /// <returns>Returns true if both are equal are equal or false otherwise</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as AudioCapabilities);
    }

    /// <summary>
    /// Check if two audio capabilities are equal.
    /// </summary>
    /// <param name="ac2">Second audio capability to compare with.</param>
    /// <returns>Returns true if both audio capabilities are equal or false otherwise.</returns>
    public bool Equals(AudioCapabilities ac2)
    {
        if (ac2 == null)
        {
            return false;
        }

        return (AvgBytesPerSec == ac2.AvgBytesPerSec) && (BitsPerSample == ac2.BitsPerSample) && (BlockAlign == ac2.BlockAlign) && (Channels == ac2.Channels) && (SamplesPerSec == ac2.SamplesPerSec);
    }

    /// <summary>
    /// Get hash code of the object.
    /// </summary>
    /// <returns>Returns hash code ot the object.</returns>
    public override int GetHashCode()
    {
        return AvgBytesPerSec.GetHashCode() ^ BitsPerSample.GetHashCode() ^ BlockAlign.GetHashCode() ^ Channels.GetHashCode() ^ SamplesPerSec.GetHashCode();
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="a">First object to check.</param>
    /// <param name="b">Seconds object to check.</param>
    /// <returns>Return true if both objects are equal or false otherwise.</returns>
    public static bool operator ==(AudioCapabilities a, AudioCapabilities b)
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
    /// Inequality operator.
    /// </summary>
    /// <param name="a">First object to check.</param>
    /// <param name="b">Seconds object to check.</param>
    /// <returns>Return true if both objects are not equal or false otherwise.</returns>
    public static bool operator !=(AudioCapabilities a, AudioCapabilities b)
    {
        return !(a == b);
    }
}
