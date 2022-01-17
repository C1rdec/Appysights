using System;
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
        private bool _skipMainAction;
        private Action<EventTileViewModel> _removeCallback;
        private AppInsightEvent _event;

        #endregion

        #region Constructors

        public EventTileViewModel(AppInsightEvent appEvent, Position position, Action<EventTileViewModel> removeCallback)
            : base(appEvent)
        {
            _removeCallback = removeCallback;
            _position = position;
            _event = appEvent;
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

        public AppInsightEvent Event => _event;

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
            if (Selected || _skipMainAction)
            {
                _skipMainAction = false;

                if (Selected)
                {
                    IoC.Get<IEventAggregator>().PublishOnUIThreadAsync(new DashboardMessage(Close)
                    {
                        RequestClose = true,
                    }); 
                }

                return;
            }

            Select();
        }

        public void Remove()
        {
            _skipMainAction = true;
            _removeCallback(this);
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
                if (view == null)
                {
                    return;
                }

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
