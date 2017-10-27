using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Utility;
namespace OrchardNorthwind.Services.OrleansClient.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class OrleansTestBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected const int PageIndex = 1;
        /// <summary>
        /// 
        /// </summary>
        protected const int PageSize = 20;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected static int GetIntGrainId()
        {
            return RandomData.GetInt();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected static long GetLongGrainId()
        {
            return RandomData.GetLong();
        }
    }
}
