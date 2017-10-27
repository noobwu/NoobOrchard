using System;
using Orchard.Metrics;

namespace Orchard.Tests.Queue {
    public class SimpleWorkItem : IHaveSubMetricName {
        public string Data { get; set; }
        public int Id { get; set; }

        public string GetSubMetricName() {
            if (string.IsNullOrEmpty(Data)) return null;
            return Data.Trim();
        }
    }
}
