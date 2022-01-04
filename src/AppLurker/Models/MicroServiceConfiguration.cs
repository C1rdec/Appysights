namespace AppLurker.Models
{
    public class MicroServiceConfiguration
    {
        #region Properties

        public string Name { get; set; }

        public AppInsightsConfiguration Dev { get; set; }

        public AppInsightsConfiguration Prod { get; set; }

        #endregion
    }
}
