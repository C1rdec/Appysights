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

        private DateTime LocalDate => Timestamp.ToLocalTime();


        #endregion

        #region Methods

        public string GetLocalDate()
        {
            return LocalDate.ToString("f");
        }

        public string GetSimpleLocalDate()
        {
            return LocalDate.ToString("h:mm tt");
        }

        #endregion
    }
}