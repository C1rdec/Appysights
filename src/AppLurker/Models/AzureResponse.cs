using System.Collections.Generic;

namespace AppLurker.Models
{
    public class AzureResponse<T>
    {
        public IEnumerable<T> Value { get; set; }
    }
}
