using System;

namespace TrulyRandom;

/// <summary>
/// The exception that is thrown when the buffer doesn't contain sufficient random data to fulfill the request.
/// </summary>
[Serializable]
public class OutOfRandomnessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRandomnessException"/> class.
    /// </summary>
    public OutOfRandomnessException() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRandomnessException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public OutOfRandomnessException(string message) : base(message) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRandomnessException"/> class with a specified error message 
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">Inner exception.</param>
    public OutOfRandomnessException(string message, Exception inner) : base(message, inner) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRandomnessException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The object that holds the serialized object data.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    protected OutOfRandomnessException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
