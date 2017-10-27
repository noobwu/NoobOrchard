// Copyright (c) Orchard, Inc. All Rights Reserved.
// License: https://raw.github.com/Orchard/Orchard/master/license.txt


using System.Collections.Generic;

namespace Orchard.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeta
    {
        Dictionary<string, string> Meta { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IHasSessionId
    {
        string SessionId { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IHasVersion
    {
        int Version { get; set; }
    }
}