using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using AppLurker.Models;
using AppLurker.Services;
using MahApps.Metro.Controls;

namespace AppLurker.ViewModels
{
    public class AppInsightsViewModel : Caliburn.Micro.PropertyChangedBase, System.IDisposable
    {
        #region Fields

        private AppInsightsService _service;
        private Position _position;
        private int _pageIndex = 0;
        private int _pageSize = 20;
        private readonly object pageLock = new();

        #endregion

        #region Constructors

        public AppInsightsViewModel(AppInsightsService service, Position position)
        {
            _position = position;
            _service = service;
            _service.NewEvent += Service_NewEvent;
            Events = new ObservableCollection<EventTileViewModel>();

            DisplayNextPage();
        }

        #endregion

        #region Properties

        public ObservableCollection<EventTileViewModel> Events { get; set; }

        public string ServiceName => _service.Name;

        public bool Selected => Events.Any(e => e.Selected);

        #endregion

        #region Methods

        public void OnScroll(System.Windows.Controls.ScrollChangedEventArgs scrollEvent)
        {
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

        public void Dispose()
        {
            _service.NewEvent -= Service_NewEvent;
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

        private void Service_NewEvent(object sender, AppInsightEvent e)
        {
            Events.Add(new EventTileViewModel(e, _position));
        }

        private void DisplayNextPage()
        {
            if (Events.Count >= _service.Events.Count())
            {
                return;
            }

            var eventToDisplay = _service.Events.Skip(_pageIndex * _pageSize).Take(_pageSize);
            foreach (var appInsightEvent in eventToDisplay)
            {
                Events.Add(new EventTileViewModel(appInsightEvent, _position));
            }

            _pageIndex++;
        }

        #endregion
    }
}
