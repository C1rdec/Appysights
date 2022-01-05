using System.Linq;
using AppLurker.Models;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace AppLurker.ViewModels
{
    public class EventTileViewModel : EventViewModelBase
    {
        private Position _position;
        private string _environment;
        private bool _selected;

        public EventTileViewModel(AppInsightEvent appEvent, Position position)
            : base(appEvent)
        {
            _position = position;
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
                Position = _position,
                AppInsightEvent = AppEvent
            });
        }

        public void Close()
        {
            Selected = false;
        }
    }
}
