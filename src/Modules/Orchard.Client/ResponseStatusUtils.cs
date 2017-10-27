// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using Orchard.Domain.Entities;
using System;
using System.Collections.Generic;
using static System.String;

namespace Orchard.Client
{
    public static class ResponseStatusUtils
    {
        /// <summary>
        /// Creates the error response from the values provided.
        /// 
        /// If the errorCode is empty it will use the first validation error code, 
        /// if there is none it will throw an error.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public static ResponseStatus CreateResponseStatus(string errorCode, string errorMessage)
        {
            var to = new ResponseStatus {
                ErrorCode = errorCode,
                Message = errorMessage,
                Errors = new List<ResponseError>(),
            };
            if (IsNullOrEmpty(errorCode) && IsNullOrEmpty(to.ErrorCode))
                throw new ArgumentException("Cannot create a valid error response with a en empty errorCode and an empty validationError list");

            return to;
        }
    }
}