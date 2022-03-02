using System;

namespace TrulyRandom
{

    [Serializable]
    public class OutOfRandomnessException : Exception
    {
        public OutOfRandomnessException() { }
        public OutOfRandomnessException(string message) : base(message) { }
        public OutOfRandomnessException(string message, Exception inner) : base(message, inner) { }
        protected OutOfRandomnessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
