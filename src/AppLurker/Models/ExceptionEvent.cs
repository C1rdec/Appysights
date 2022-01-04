namespace AppLurker.Models
{
    public class ExceptionEvent : AppInsightEvent
    {
        public AppException Exception { get; set; }
    }
}
