namespace Appysights.Models
{
    public class ExceptionEvent : AppInsightEvent
    {
        #region Properties

        public AppException Exception { get; set; }

        #endregion
    }
}
