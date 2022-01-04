using System.Collections.Generic;

namespace AppLurker.Models
{
    public class Configuration
    {
        #region Constructors

        public Configuration()
        {
            Services = new List<MicroServiceConfiguration>();
        }

        #endregion

        #region Properties

        public IEnumerable<MicroServiceConfiguration> Services { get; set; }

        public AppInsightsConfiguration Footer { get; set; }

        #endregion
    }
}
