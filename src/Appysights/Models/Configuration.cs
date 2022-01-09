using System.Collections.Generic;

namespace Appysights.Models
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

        public AppInsightsConfiguration Statusbar { get; set; }

        #endregion
    }
}