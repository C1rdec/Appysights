using System.Linq;
using Appysights.Models;
using Appysights.Views;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Appysights.ViewModels
{
    public class EventTileViewModel : EventViewModelBase, IViewAware
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
                var items = appEvent.Cloud.RoleName.Split("-");
                if (items.Length > 2)
                {
                    _environment = appEvent.Cloud.RoleName.Split("-").Last().ToUpper();
                }
                else
                {
                    _environment = appEvent.Cloud.RoleName.ToUpper();
                }
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

            Select();
        }

        public void Close()
        {
            Selected = false;
        }

        public void Select()
        {
            Selected = true;

            // For Keyboard navigation
            Execute.OnUIThread(() => 
            {
                var view = GetView() as EventTileView;
                view.MainBorder.BringIntoView();
            });

            IoC.Get<IEventAggregator>().PublishOnUIThreadAsync(new DashboardMessage(Close)
            {
                Position = _position,
                AppInsightEvent = AppEvent
            });
        }
    }
}
