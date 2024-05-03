using System;
using System.Drawing;
using System.Runtime.InteropServices;

// Source: AForge library
namespace TrulyRandom.DirectShow;

[StructLayout(LayoutKind.Sequential)]
internal class AMMediaType : IDisposable
{
    /// <summary>
    /// Globally unique identifier (GUID) that specifies the major type of the media sample.
    /// </summary>
    public Guid MajorType;

    /// <summary>
    /// GUID that specifies the subtype of the media sample.
    /// </summary>
    public Guid SubType;

    /// <summary>
    /// If <b>true</b>, samples are of a fixed size.
    /// </summary>
    [MarshalAs(UnmanagedType.Bool)]
    public bool FixedSizeSamples = true;

    /// <summary>
    /// If <b>true</b>, samples are compressed using temporal (interframe) compression.
    /// </summary>
    [MarshalAs(UnmanagedType.Bool)]
    public bool TemporalCompression;

    /// <summary>
    /// Size of the sample in bytes. For compressed data, the value can be zero.
    /// </summary>
    public int SampleSize = 1;

    /// <summary>
    /// GUID that specifies the structure used for the format block.
    /// </summary>
    public Guid FormatType;

    /// <summary>
    /// Not used.
    /// </summary>
    public IntPtr unkPtr;

    /// <summary>
    /// Size of the format block, in bytes.
    /// </summary>
    public int FormatSize;

    /// <summary>
    /// Pointer to the format block.
    /// </summary>
    public IntPtr FormatPtr;

    /// <summary>
    /// Destroys the instance of the <see cref="AMMediaType"/> class.
    /// </summary>
    ~AMMediaType()
    {
        Dispose(false);
    }

    /// <summary>
    /// Dispose the object.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        // remove me from the Finalization queue 
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose the object.
    /// </summary>
    /// <param name="disposing">Indicates if disposing was initiated manually.</param>
    protected virtual void Dispose(bool disposing)
    {
        if ((FormatSize != 0) && (FormatPtr != IntPtr.Zero))
        {
            Marshal.FreeCoTaskMem(FormatPtr);
            FormatSize = 0;
        }

        if (unkPtr != IntPtr.Zero)
        {
            Marshal.Release(unkPtr);
            unkPtr = IntPtr.Zero;
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
internal struct PinInfo
{
    /// <summary>
    /// Owning filter.
    /// </summary>
    public IBaseFilter Filter;

    /// <summary>
    /// Direction of the pin.
    /// </summary>
    public PinDirection Direction;

    /// <summary>
    /// Name of the pin.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string Name;
}

[StructLayout(LayoutKind.Sequential)]
internal class VideoStreamConfigCaps		// VIDEO_STREAM_CONFIG_CAPS
{
    public Guid Guid;
    public AnalogVideoStandard VideoStandard;
    public Size InputSize;
    public Size MinCroppingSize;
    public Size MaxCroppingSize;
    public int CropGranularityX;
    public int CropGranularityY;
    public int CropAlignX;
    public int CropAlignY;
    public Size MinOutputSize;
    public Size MaxOutputSize;
    public int OutputGranularityX;
    public int OutputGranularityY;
    public int StretchTapsX;
    public int StretchTapsY;
    public int ShrinkTapsX;
    public int ShrinkTapsY;
    public long MinFrameInterval;
    public long MaxFrameInterval;
    public int MinBitsPerSecond;
    public int MaxBitsPerSecond;
}

[StructLayout(LayoutKind.Sequential)]
internal class AudioStreamConfigCaps		// AUDIO_STREAM_CONFIG_CAPS
{
    public Guid Guid;
    public ulong MinimumChannels;
    public ulong MaximumChannels;
    public ulong ChannelsGranularity;
    public ulong MinimumBitsPerSample;
    public ulong MaximumBitsPerSample;
    public ulong BitsPerSampleGranularity;
    public ulong MinimumSampleFrequency;
    public ulong MaximumSampleFrequency;
    public ulong SampleFrequencyGranularity;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    /// <summary>
    /// Specifies the x-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int Left;

    /// <summary>
    /// Specifies the y-coordinate of the upper-left corner of the rectangle. 
    /// </summary>
    public int Top;

    /// <summary>
    /// Specifies the x-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int Right;

    /// <summary>
    /// Specifies the y-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int Bottom;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct BitmapInfoHeader
{
    /// <summary>
    /// Specifies the number of bytes required by the structure.
    /// </summary>
    public int Size;

    /// <summary>
    /// Specifies the width of the bitmap.
    /// </summary>
    public int Width;

    /// <summary>
    /// Specifies the height of the bitmap, in pixels.
    /// </summary>
    public int Height;

    /// <summary>
    /// Specifies the number of planes for the target device. This value must be set to 1.
    /// </summary>
    public short Planes;

    /// <summary>
    /// Specifies the number of bits per pixel.
    /// </summary>
    public short BitCount;

    /// <summary>
    /// If the bitmap is compressed, this member is a <b>FOURCC</b> the specifies the compression.
    /// </summary>
    public int Compression;

    /// <summary>
    /// Specifies the size, in bytes, of the image.
    /// </summary>
    public int ImageSize;

    /// <summary>
    /// Specifies the horizontal resolution, in pixels per meter, of the target device for the bitmap.
    /// </summary>
    public int XPelsPerMeter;

    /// <summary>
    /// Specifies the vertical resolution, in pixels per meter, of the target device for the bitmap.
    /// </summary>
    public int YPelsPerMeter;

    /// <summary>
    /// Specifies the number of color indices in the color table that are actually used by the bitmap.
    /// </summary>
    public int ColorsUsed;

    /// <summary>
    /// Specifies the number of color indices that are considered important for displaying the bitmap.
    /// </summary>
    public int ColorsImportant;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VideoInfoHeader
{
    /// <summary>
    /// <see cref="RECT"/> structure that specifies the source video window.
    /// </summary>
    public RECT SrcRect;

    /// <summary>
    /// <see cref="RECT"/> structure that specifies the destination video window.
    /// </summary>
    public RECT TargetRect;

    /// <summary>
    /// Approximate data rate of the video stream, in bits per second.
    /// </summary>
    public int BitRate;

    /// <summary>
    /// Data error rate, in bit errors per second.
    /// </summary>
    public int BitErrorRate;

    /// <summary>
    /// The desired average display time of the video frames, in 100-nanosecond units.
    /// </summary>
    public long AverageTimePerFrame;

    /// <summary>
    /// <see cref="BitmapInfoHeader"/> structure that contains color and dimension information for the video image bitmap.
    /// </summary>
    public BitmapInfoHeader BmiHeader;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VideoInfoHeader2
{
    /// <summary>
    /// <see cref="RECT"/> structure that specifies the source video window.
    /// </summary>
    public RECT SrcRect;

    /// <summary>
    /// <see cref="RECT"/> structure that specifies the destination video window.
    /// </summary>
    public RECT TargetRect;

    /// <summary>
    /// Approximate data rate of the video stream, in bits per second.
    /// </summary>
    public int BitRate;

    /// <summary>
    /// Data error rate, in bit errors per second.
    /// </summary>
    public int BitErrorRate;

    /// <summary>
    /// The desired average display time of the video frames, in 100-nanosecond units.
    /// </summary>
    public long AverageTimePerFrame;

    /// <summary>
    /// Flags that specify how the video is interlaced.
    /// </summary>
    public int InterlaceFlags;

    /// <summary>
    /// Flag set to indicate that the duplication of the stream should be restricted.
    /// </summary>
    public int CopyProtectFlags;

    /// <summary>
    /// The X dimension of picture aspect ratio.
    /// </summary>
    public int PictAspectRatioX;

    /// <summary>
    /// The Y dimension of picture aspect ratio.
    /// </summary>
    public int PictAspectRatioY;

    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public int Reserved1;

    /// <summary>
    /// Reserved for future use. 
    /// </summary>
    public int Reserved2;

    /// <summary>
    /// <see cref="BitmapInfoHeader"/> structure that contains color and dimension information for the video image bitmap.
    /// </summary>
    public BitmapInfoHeader BmiHeader;
}

[StructLayout(LayoutKind.Sequential)]
internal struct WaveFormatEx
{
    public short wFormatTag;
    public short nChannels;
    public int nSamplesPerSec;
    public int nAvgBytesPerSec;
    public short nBlockAlign;
    public short wBitsPerSample;
    public short cbSize;
}
