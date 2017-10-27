using System;

namespace Orchard.Web.Models
{
    /// <inheritdoc/>
    public class ErrorInfoBuilder : IErrorInfoBuilder, ISingletonDependency
    {
        private IExceptionToErrorInfoConverter Converter { get; set; }

        /// <inheritdoc/>
        public ErrorInfoBuilder()
        {
            Converter = new DefaultErrorInfoConverter();
        }

        /// <inheritdoc/>
        public ErrorInfo BuildForException(Exception exception)
        {
            return Converter.Convert(exception);
        }

        /// <summary>
        /// Adds an exception converter that is used by <see cref="BuildForException"/> method.
        /// </summary>
        /// <param name="converter">Converter object</param>
        public void AddExceptionConverter(IExceptionToErrorInfoConverter converter)
        {
            converter.Next = Converter;
            Converter = converter;
        }
    }
}
