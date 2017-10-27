using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Orchard.Repositories.BenchmarkTests
{
    /// <summary>
    /// 方法返回值对应的列信息
    /// </summary>
    public class ReturnColum : IColumn
    {
        /// <summary>
        ///  An unique identificator of the column. If there are several columns with the
        ///  same Id, only one of them will be shown in the summary.
        /// </summary>
        public string Id => nameof(ReturnColum);
        /// <summary>
        ///  Display column title in the summary.
        /// </summary>
        public string ColumnName { get; } = "Return";
        /// <summary>
        ///   Column description.
        /// </summary>
        public string Legend => "The return type of the method";

        public bool IsDefault(Summary summary, Benchmark benchmark) => false;
        /// <summary>
        /// Value in this column formatted using the default style.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="benchmark"></param>
        /// <returns>方法返回值</returns>
        public string GetValue(Summary summary, Benchmark benchmark) => benchmark.Target.Method.ReturnType.Name;
        /// <summary>
        /// Value in this column formatted using the specified style.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="benchmark"></param>
        /// <param name="style"></param>
        /// <returns>方法返回值</returns>
        public string GetValue(Summary summary, Benchmark benchmark, ISummaryStyle style) => benchmark.Target.Method.ReturnType.Name;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public bool IsAvailable(Summary summary) => true;
        public bool AlwaysShow => true;
        /// <summary>
        ///  Defines order of column in the same category.
        /// </summary>
        public ColumnCategory Category => ColumnCategory.Job;
        public int PriorityInCategory => 1;
        /// <summary>
        ///   Defines if the column's value represents a number
        /// </summary>
        public bool IsNumeric => false;
        /// <summary>
        ///    Defines how to format column's value
        /// </summary>
        public UnitType UnitType => UnitType.Dimensionless;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ColumnName;
    }
}
