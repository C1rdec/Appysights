using System.Threading.Tasks;
using Appysights.Helper;
using Appysights.Models;

namespace Appysights.ViewModels
{
    public class EventViewModel : EventViewModelBase
    {
        private bool _isCopiedOpen;

        public EventViewModel(AppInsightEvent appEvent) 
            : base(appEvent)
        {
        }

        public bool IsCopiedOpen
        {
            get
            {
                return _isCopiedOpen;
            }

            set
            {
                _isCopiedOpen = value;
                NotifyOfPropertyChange();
            }
        }

        public void OnActionClick()
        {
            IsCopiedOpen = true;
            Task.Delay(1200).ContinueWith(t => IsCopiedOpen = false);
            ClipboardHelper.SetText(ActionName);
        }
    }
}
