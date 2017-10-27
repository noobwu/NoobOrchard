using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard
{
    /// <summary>
    /// 
    /// </summary>
    public class PclExport
    {
        public static PclExport Instance = new PclExport();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  string GetStackTrace()
        {
            return System.Environment.StackTrace;
        }
    }
}
