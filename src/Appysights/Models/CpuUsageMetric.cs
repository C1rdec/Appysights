using System.Text.Json.Serialization;

namespace Appysights.Models
{
    public class CpuUsageMetric
    {
        public string Start { get; set; }

        public string End { get; set; }

        [JsonPropertyName("performanceCounters/processCpuPercentage")]
        public AverageResult CpuPercentage { get; set; }
    }
}
