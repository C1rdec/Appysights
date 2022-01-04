using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using AppLurker.Enums;
using AppLurker.Models;
using AppLurker.Services;

namespace AppLurker.ViewModels
{
    public class AppInsightsViewModel : Caliburn.Micro.PropertyChangedBase, System.IDisposable
    {
        #region Fields

        private AppInsightsService _service;
        private EnvironmentType _type;
        private int _pageIndex = 0;
        private int _pageSize = 10;
        private readonly object pageLock = new object();

        #endregion

        #region Constructors

        public AppInsightsViewModel(AppInsightsService service, EnvironmentType type)
        {
            _type = type;
            _service = service;
            _service.NewEvent += Service_NewEvent;
            Events = new ObservableCollection<EventTileViewModel>();

            DisplayNextPage();
        }

        #endregion

        #region Properties

        public ObservableCollection<EventTileViewModel> Events { get; set; }

        public string Type => _type.ToString();

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

        private void Service_NewEvent(object sender, AppInsightEvent e)
        {
            Events.Add(new EventTileViewModel(e, _type));
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
                Events.Add(new EventTileViewModel(appInsightEvent, _type));
            }

            _pageIndex++;
        }

        #endregion
    }
}
