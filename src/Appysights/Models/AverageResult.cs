using System.Text.Json.Serialization;

namespace Appysights.Models
{
    public class AverageResult
    {
        [JsonPropertyName("avg")]
        public double Average { get; set; }
    }
}
