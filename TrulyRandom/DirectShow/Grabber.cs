using System;
using System.Runtime.InteropServices;

// Source: AForge library
namespace TrulyRandom.DirectShow
{
    class Grabber : ISampleGrabberCB
    {
        public event Action<byte[]> NewData;

        // Constructor
        public Grabber()
        {
        }

        // Callback to receive samples
        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        // Callback method that receives a pointer to the sample buffer
        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            byte[] result = new byte[bufferLen];
            Marshal.Copy(buffer, result, 0, bufferLen);
            if (NewData != null)
            {
                NewData(result);
            }
            return 0;
        }
    }
}
