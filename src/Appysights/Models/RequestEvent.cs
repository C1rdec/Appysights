namespace Appysights.Models
{
    public class RequestEvent : AppInsightEvent
    {
        #region Properties

        public AppRequest Request { get; set; }

        #endregion
    }
}