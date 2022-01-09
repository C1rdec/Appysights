using System.Collections.Generic;

namespace Appysights.Models
{
    public class AzureResponse<T>
    {
        #region Properties

        public IEnumerable<T> Value { get; set; }

        #endregion
    }
}