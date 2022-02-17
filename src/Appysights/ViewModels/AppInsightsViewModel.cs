using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appysights.Models;
using Appysights.Services;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Appysights.ViewModels
{
    public class AppInsightsViewModel : PropertyChangedBase, System.IDisposable
    {
        #region Fields

        private AppInsightsService _service;
        private IEnumerable<AppInsightEvent> _currentEvents;
        private Position _position;
        private int _pageIndex = 0;
        private int _pageSize = 20;
        private readonly object pageLock = new();
        private bool _listenScroll = true;
        private bool _requestMode;
        private bool _isBusy;

        #endregion

        #region Constructors

        public AppInsightsViewModel(AppInsightsService service, Position position)
        {
            _position = position;
            _service = service;
            Events = new ObservableCollection<EventTileViewModel>();

            if (_service.IsBusy)
            {
                IsBusy = true;
                _service.BusyChanged += Service_BusyChanged;
                
            }
            else
            {
                _service.NewEvent += Service_NewEvent;
                _service.DeleteEvent += Service_DeleteEvent;
                _currentEvents = _service.Events;
                DisplayNextPage();
            }
        }

        private void Service_BusyChanged(object sender, bool e)
        {
            if (!e)
            {
                IsBusy = false;
                _service.NewEvent += Service_NewEvent;
                _service.DeleteEvent += Service_DeleteEvent;
                _currentEvents = _service.Events;
                Execute.OnUIThread(() => DisplayNextPage());
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<EventTileViewModel> Events { get; set; }

        public string ServiceName => _service.Name;

        public bool Selected => Events.Any(e => e.Selected);

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                NotifyOfPropertyChange();
            }
        }

        public bool RequestMode
        {
            get
            {
                return _requestMode;
            }

            private set
            {
                _requestMode = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        public async void ToggleEvents()
        {
            Clear();
            if (!RequestMode)
            {
                RequestMode = true;
                IsBusy = true;
                _currentEvents = await GetLastHourFailedRequests();
                IsBusy = false;
            }
            else
            {
                RequestMode = false;
                _currentEvents = _service.Events;
            }


            DisplayNextPage();
        }

        public async void RefreshRequests()
        {
            Clear();
            _currentEvents = await GetLastHourFailedRequests();
            DisplayNextPage();
        }

        public void OnScroll(System.Windows.Controls.ScrollChangedEventArgs scrollEvent)
        {
            if (!_listenScroll)
            {
                _listenScroll = true;
                return;
            }

            var position = scrollEvent.VerticalOffset + scrollEvent.ViewportHeight;
            var heightThreshold = scrollEvent.ExtentHeight / 1.1;

            if (position >= heightThreshold)
            {
                if (Monitor.TryEnter(pageLock))
                {
                    try
                    {
                        DisplayNextPage();
                    }
                    finally
                    {
                        Monitor.Exit(pageLock);
                    }
                }
            }
        }

        public void Next()
        {
            var selectedEvent = Events.FirstOrDefault(e => e.Selected);
            if (selectedEvent == null)
            {
                var first = Events.FirstOrDefault();
                if (first != null)
                {
                    first.Select();
                }
            }
            else
            {
                var index = Events.IndexOf(selectedEvent);
                if (index == -1 || index + 1 >= Events.Count)
                {
                    return;
                }

                Events[index + 1].Select();
            }
        }

        public void Previous()
        {
            var selectedEvent = Events.FirstOrDefault(e => e.Selected);
            if (selectedEvent == null)
            {
                return;
            }

            var index = Events.IndexOf(selectedEvent);
            if (index == -1 || index - 1 < 0)
            {
                return;
            }

            Events[index - 1].Select();
        }

        public void Dispose()
        {
            _service.NewEvent -= Service_NewEvent;
            _service.DeleteEvent -= Service_DeleteEvent;
            _service.BusyChanged -= Service_BusyChanged;
        }

        private async Task<IEnumerable<RequestEvent>> GetLastHourFailedRequests()
        {
            IsBusy = true;
            var requests = await _service.GetLastHourFailedRequests();
            IsBusy = false;
            return requests;
        }

        private void Service_NewEvent(object sender, AppInsightEvent e)
        {
            if (RequestMode)
            {
                return;
            }

            Events.Insert(0, new EventTileViewModel(e, _position, RemoveEvent));
        }

        private void Service_DeleteEvent(object sender, AppInsightEvent e)
        {
            if (RequestMode)
            {
                return;
            }

            var viewModel = Events.FirstOrDefault(e => e.Id == e.Id);
            if (viewModel != null)
            {
                Events.Remove(viewModel);
            }
        }

        private void DisplayNextPage()
        {
            if (Events.Count >= _currentEvents.Count())
            {
                return;
            }

            var eventToDisplay = Enumerable.Empty<AppInsightEvent>();

            if (Events.Count < _pageSize)
            {
                // This is to handle when event are removed from the list
                eventToDisplay =  _currentEvents.Skip(Events.Count).Take(_pageSize);
            }
            else
            {
                eventToDisplay = _currentEvents.Skip(_pageIndex * _pageSize).Take(_pageSize);
            }

            foreach (var appInsightEvent in eventToDisplay)
            {
                Events.Add(new EventTileViewModel(appInsightEvent, _position, RemoveEvent));
            }

            _pageIndex++;
        }

        private void Clear()
        {
            _listenScroll = false;
            Events.Clear();

            _pageIndex = 0;
        }

        private void RemoveEvent(EventTileViewModel viewModel)
        {
            _service.Remove(viewModel.Event);
            Events.Remove(viewModel);
        }

        #endregion
    }
}
