using System.Collections.Generic;

namespace Appysights.Models
{
    public class AzureResponse<T>
    {
        #region Properties

        public List<T> Value { get; set; }

        #endregion
    }
}
