using AppLurker.Models;

namespace AppLurker.ViewModels
{
    public class EventViewModel : EventViewModelBase
    {
        public EventViewModel(AppInsightEvent appEvent) 
            : base(appEvent)
        {
        }
    }
}
