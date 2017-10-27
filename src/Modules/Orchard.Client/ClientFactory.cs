using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Client
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientFactory
    {
        public static IOneWayClient Create(string endpointUrl)
        {
            if (string.IsNullOrEmpty(endpointUrl) || !endpointUrl.StartsWith("http"))
                return null;
            return null;
        }
    }
}
