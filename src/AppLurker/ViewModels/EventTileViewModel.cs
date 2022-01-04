using System.Linq;
using Caliburn.Micro;
using AppLurker.Enums;
using AppLurker.Models;

namespace AppLurker.ViewModels
{
    public class EventTileViewModel : EventViewModelBase
    {
        private EnvironmentType _type;
        private string _environment;
        private bool _selected;

        public EventTileViewModel(AppInsightEvent appEvent, EnvironmentType type)
            : base(appEvent)
        {
            _type = type;
            if (appEvent.Cloud != null)
            {
                _environment = appEvent.Cloud.RoleName.Split("-").Last().ToUpper();
            }
        }

        public string Environment => _environment;

        public bool Selected
        {
            get
            {
                return _selected;
            }

            private set
            {
                _selected = value;
                NotifyOfPropertyChange(() => Selected);
            }
        }

        public void OnClick()
        {
            if (Selected)
            {
                return;
            }

            Selected = true;
            IoC.Get<IEventAggregator>().PublishOnUIThreadAsync(new DashboardMessage(Close)
            {
                Type = _type,
                AppInsightEvent = AppEvent
            });
        }

        public void Close()
        {
            Selected = false;
        }
    }
}
