using System;

namespace Appysights.Models
{
    public class AppInsightEvent
    {
        #region Properties

        public Guid Id { get; set; }

        public string Type { get; set; }

        public DateTime Timestamp { get; set; }

        public Cloud Cloud { get; set; }

        public CustomDimensions CustomDimensions { get; set; }

        #endregion

        #region Methods

        public string GetLocalDate()
        {
            return Timestamp.ToLocalTime().ToString("f");
        }

        #endregion
    }
}
