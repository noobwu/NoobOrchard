using System;
using System.Runtime.Serialization;

namespace Orchard.Exceptions
{
    /// <summary>
    /// This exception is thrown if a problem on ABP initialization progress.
    /// </summary>
    [Serializable]
    public class InitializationException : DefaultException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InitializationException()
        {

        }

#if NET46
        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public InitializationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
#endif

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public InitializationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public InitializationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
