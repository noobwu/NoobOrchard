using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net;
namespace Orchard.Client
{
    public static class DtoUtils
    {
        /// <summary>
        /// Naming convention for the ResponseStatus property name on the response DTO
        /// </summary>
        public const string ResponseStatusPropertyName = "ResponseStatus";

        public static ResponseStatus ToResponseStatus(this Exception exception)
        {
            var customStatus = exception as IResponseStatusConvertible;
            return customStatus != null
                ? customStatus.ToResponseStatus()
                : CreateResponseStatus(exception.GetType().Name, exception.Message);
        }

    
        public static ResponseStatus CreateSuccessResponse(string message)
        {
            return new ResponseStatus { Message = message };
        }

        public static ResponseStatus CreateResponseStatus(string errorCode)
        {
            var errorMessage = errorCode.SplitCamelCase();
            return ResponseStatusUtils.CreateResponseStatus(errorCode, errorMessage);
        }
        public static ResponseStatus CreateResponseStatus(string errorCode, string errorMessage)
        {
            return ResponseStatusUtils.CreateResponseStatus(errorCode, errorMessage);
        }
    }
}