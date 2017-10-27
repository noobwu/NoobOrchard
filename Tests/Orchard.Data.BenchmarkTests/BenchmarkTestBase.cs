using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.BenchmarkTests
{
    /// <summary>
    /// 
    /// </summary>
    public class BenchmarkTestBase
    {
        protected const long DefaultIterations = 1;
        //protected readonly List<long> BatchIterations = new List<long> { 100, 1000, 5000, 20000, /*100000, 250000, 1000000, 5000000*/ };
        //protected readonly List<long> BatchIterations = new List<long> { 1, 10, 100 };
        protected readonly List<long> BatchIterations = new List<long> { 1 };
    }
}
