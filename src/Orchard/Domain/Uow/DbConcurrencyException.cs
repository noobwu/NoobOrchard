using Orchard.Exceptions;
using System;
using System.Runtime.Serialization;

namespace Orchard.Domain.Uow
{
    [Serializable]
    public class DbConcurrencyException : DefaultException
    {
        /// <summary>
        /// Creates a new <see cref="AbpDbConcurrencyException"/> object.
        /// </summary>
        public DbConcurrencyException()
        {

        }

#if NET46
        /// <summary>
        /// Creates a new <see cref="AbpException"/> object.
        /// </summary>
        public DbConcurrencyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
#endif

        /// <summary>
        /// Creates a new <see cref="AbpDbConcurrencyException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public DbConcurrencyException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="AbpDbConcurrencyException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public DbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
