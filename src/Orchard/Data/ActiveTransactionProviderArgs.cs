using System.Collections.Generic;

namespace Orchard.Data
{
    public class ActiveTransactionProviderArgs : Dictionary<string, object>
    {
        public static ActiveTransactionProviderArgs Empty { get { return new ActiveTransactionProviderArgs(); } } 
    }
}
