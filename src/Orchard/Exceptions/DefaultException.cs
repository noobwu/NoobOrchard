using System;
using System.Runtime.Serialization;

namespace Orchard.Exceptions
{
    /// <summary>
    /// Base exception type for those are thrown by Abp system for Abp specific exceptions.
    /// </summary>
    [Serializable]
    public class DefaultException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="DefaultException"/> object.
        /// </summary>
        public DefaultException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="DefaultException"/> object.
        /// </summary>
        public DefaultException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="DefaultException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public DefaultException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="DefaultException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public DefaultException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
