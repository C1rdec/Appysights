using System.Linq;
using Appysights.Models;
using Appysights.Views;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Appysights.ViewModels
{
    public class EventTileViewModel : EventViewModelBase, IViewAware
    {
        #region Fields

        private Position _position;
        private string _environment;
        private bool _selected;

        #endregion

        #region Constructors

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

        #endregion

        #region Properties

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

        #endregion

        #region Methods

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

        #endregion
    }
}
