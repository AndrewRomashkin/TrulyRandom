using System;
using System.Runtime.InteropServices;

// Source: AForge library
namespace TrulyRandom.DirectShow;

internal class Grabber : ISampleGrabberCB
{
    public event Action<byte[]> NewData;

    public Grabber()
    {
    }

    /// <summary>
    /// Callback to receive samples.
    /// </summary>
    public int SampleCB(double sampleTime, IntPtr sample)
    {
        return 0;
    }

    /// <summary>
    /// Callback method that receives a pointer to the sample buffer.
    /// </summary>
    public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
    {
        byte[] result = new byte[bufferLen];
        Marshal.Copy(buffer, result, 0, bufferLen);
        NewData?.Invoke(result);
        return 0;
    }
}
