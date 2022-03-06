using System;
using System.Runtime.InteropServices;

// Source: AForge library
namespace TrulyRandom.DirectShow
{
    class Grabber : ISampleGrabberCB
    {
        public event Action<byte[]> NewData;

        public Grabber()
        {
        }

        /// <summary>
        /// Callback to receive samples
        /// </summary>
        /// <param name="sampleTime"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        /// <summary>
        /// Callback method that receives a pointer to the sample buffer
        /// </summary>
        /// <param name="sampleTime"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferLen"></param>
        /// <returns></returns>
        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            byte[] result = new byte[bufferLen];
            Marshal.Copy(buffer, result, 0, bufferLen);
            NewData?.Invoke(result);
            return 0;
        }
    }
}
