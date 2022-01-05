using System.Collections.Generic;

namespace AppLurker.Models
{
    public class MicroServiceConfiguration
    {
        #region Constructors

        public MicroServiceConfiguration()
        {
            Applications = new List<AppInsightsConfiguration>();
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public List<AppInsightsConfiguration> Applications { get; init; }

        #endregion
    }
}
