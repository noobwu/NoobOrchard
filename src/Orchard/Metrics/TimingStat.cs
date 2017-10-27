using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Orchard.Metrics {
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Time: {Time} Count: {Count} Min: {MinDuration} Max: {MaxDuration} Total: {TotalDuration} Avg: {AverageDuration}")]
    public class TimingStat {
        public DateTime Time { get; set; }
        public int Count { get; set; }
        public long TotalDuration { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }
        public double AverageDuration => Count > 0 ? (double)TotalDuration / Count : 0;
    }
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Time: {StartTime}-{EndTime} Count: {Count} Min: {MinDuration} Max: {MaxDuration} Total: {TotalDuration} Avg: {AverageDuration}")]
    public class TimingStatSummary {
        public TimingStatSummary(string name, ICollection<TimingStat> stats, DateTime start, DateTime end) {
            Name = name;
            Stats = stats;
            Count = stats.Count > 0 ? Stats.Sum(s => s.Count) : 0;
            MinDuration = stats.Count > 0 ? Stats.Min(s => s.MinDuration) : 0;
            MaxDuration = stats.Count > 0 ? Stats.Max(s => s.MaxDuration) : 0;
            TotalDuration = stats.Count > 0 ? Stats.Sum(s => s.TotalDuration) : 0;
            AverageDuration = Count > 0 ? (double)TotalDuration / Count : 0;
            StartTime = start;
            EndTime = end;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime { get; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime EndTime { get; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<TimingStat> Stats { get; }
        public int Count { get; }
        public int MinDuration { get; }
        public int MaxDuration { get; }
        public long TotalDuration { get; }
        public double AverageDuration { get; }

        public override string ToString() {
            return $"Timing: {Name} Time: {StartTime}-{EndTime} Count: {Count} Min: {MinDuration} Max: {MaxDuration} Total: {TotalDuration} Avg: {AverageDuration}";
        }
    }
}