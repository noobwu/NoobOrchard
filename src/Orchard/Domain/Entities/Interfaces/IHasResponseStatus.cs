//Copyright (c) Orchard, Inc. All Rights Reserved.
//License: https://raw.github.com/Orchard/Orchard/master/license.txt

namespace Orchard.Domain.Entities
{
    /// <summary>
    /// Contract indication that the Response DTO has a ResponseStatus
    /// </summary>
    public interface IHasResponseStatus
    {
        ResponseStatus ResponseStatus { get; set; }
    }
}