using Appysights.Models;

namespace Appysights.ViewModels
{
    public class EventViewModel : EventViewModelBase
    {
        public EventViewModel(AppInsightEvent appEvent) 
            : base(appEvent)
        {
        }
    }
}