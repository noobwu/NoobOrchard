using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Orchard.Data.Storage.BenchmarkTests
{
    /// <summary>
    /// 对应的Storage类型
    /// </summary>
    public class StorageColum : IColumn
    {
        /// <summary>
        ///  An unique identificator of the column. If there are several columns with the
        ///  same Id, only one of them will be shown in the summary.
        /// </summary>
        public string Id => nameof(StorageColum);
        /// <summary>
        ///  Display column title in the summary.
        /// </summary>
        public string ColumnName { get; } = "Storage";
        /// <summary>
        ///   Column description.
        /// </summary>
        public string Legend => "The object/relational mapper being tested";

        public bool IsDefault(Summary summary, Benchmark benchmark) => false;
        /// <summary>
        /// Value in this column formatted using the default style.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="benchmark"></param>
        /// <returns></returns>
        public string GetValue(Summary summary, Benchmark benchmark) => benchmark.Target.Method.DeclaringType.Name.Replace("Benchmarks", string.Empty);
        /// <summary>
        /// Value in this column formatted using the specified style.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="benchmark"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public string GetValue(Summary summary, Benchmark benchmark, ISummaryStyle style) => benchmark.Target.Method.DeclaringType.Name.Replace("Benchmarks", string.Empty);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public bool IsAvailable(Summary summary) => true;
        public bool AlwaysShow => true;
        /// <summary>
        /// 
        /// </summary>
        public ColumnCategory Category => ColumnCategory.Job;
        /// <summary>
        /// Defines order of column in the same category.
        /// </summary>
        public int PriorityInCategory => -10;
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
