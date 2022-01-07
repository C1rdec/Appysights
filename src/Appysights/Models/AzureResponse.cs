using System.Collections.Generic;

namespace Appysights.Models
{
    public class AzureResponse<T>
    {
        public IEnumerable<T> Value { get; set; }
    }
}
