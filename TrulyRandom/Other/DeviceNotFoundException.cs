using System;

namespace TrulyRandom
{

    [Serializable]
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException() { }
        public DeviceNotFoundException(string message) : base(message) { }
        public DeviceNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected DeviceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
