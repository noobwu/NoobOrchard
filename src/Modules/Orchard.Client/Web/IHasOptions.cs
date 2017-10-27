using System.Collections.Generic;

namespace Orchard.Client.Web
{
    public interface IHasOptions
    {
        IDictionary<string, string> Options { get; }
    }
}