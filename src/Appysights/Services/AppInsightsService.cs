using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Appysights.Models;

namespace Appysights.Services
{
    public class AppInsightsService
    {
        #region Fields

        private static int Top => 5000;
        private CancellationTokenSource _tokenSource;
        private bool _watching;
        private bool _isBusy;
        private double _cpuPourcentage;
        private List<AppInsightEvent> _events;
        private AppInsightsConfiguration _configuration;
        private DebounceService _debounceService;

        #endregion

        #region Constructors

        public AppInsightsService(AppInsightsConfiguration configuration)
        {
            _debounceService = new DebounceService();
            _configuration = configuration;
            _events = new List<AppInsightEvent>();
        }

        #endregion

        #region Properties

        public string Name => _configuration.Name;

        public double CpuPourcentage => _cpuPourcentage;

        public bool MonitoringCpu => _configuration.MonitorCpu;

        public bool IsBusy => _isBusy;

        public IEnumerable<AppInsightEvent> Events => _events.OrderByDescending(e => e.Timestamp);

        private string ApplicationId => _configuration.ApplicationId;

        private string ApiKey => _configuration.ApiKey;

        private string BaseUrl => $"https://api.applicationinsights.io/v1/apps/{ApplicationId}";

        private string MetricsUrl => $"{BaseUrl}/metrics";

        private string CpuPercentageUrl => $"{MetricsUrl}/performanceCounters/processCpuPercentage";

        private string EventsUrl => $"{BaseUrl}/events";

        private string TracesUrl => $"{EventsUrl}/traces?$top={Top}";

        private string RequestsUrl => $"{EventsUrl}/requests";

        private string ExceptionsUrl => $"{EventsUrl}/exceptions?$top={Top}";

        private string FailedRequestUrl => $"{RequestsUrl}?$filter=(request%2Fsuccess%20eq%20'False')&$top={Top}";

        #endregion

        #region Events

        public event EventHandler<int> CountChanged;

        public event EventHandler<AppInsightEvent> NewEvent;

        public event EventHandler<AppInsightEvent> DeleteEvent;

        public event EventHandler Cleared;

        public event EventHandler<bool> BusyChanged;

        public event EventHandler<double> CpuPourcentageChanged;

        #endregion

        #region Methods

        public async void WatchExceptions(bool needInit = true)
        {
            if (_watching)
            {
                return;
            }

            _watching = true;
            using var requestCountPipeline = new MetricPipeline(CpuPercentageUrl, NewCpuPercentage, ApiKey);
            using var exceptionPipeline = new EventPipeline<ExceptionEvent>(ExceptionsUrl, NewEventAction, ApiKey);

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            var initialize = !needInit;
            var tasks = new List<Task>();
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                tasks.Add(Task.Delay(5000));
                if (_configuration.MonitorCpu)
                {
                    tasks.Add(requestCountPipeline.GetPourcentage());
                }

                if (!initialize)
                {
                    initialize = true;
                    InvokeBusyChanged(true);
                    tasks.Add(exceptionPipeline.GetLastDay().ContinueWith((t) => InvokeBusyChanged(false)));
                }
                else
                {
                    tasks.Add(exceptionPipeline.GetLastHour());
                }

                try
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    tasks.Clear();
                }
                catch (Exception)
                {
                    // Todo: Add Logging
                }
            }
        }

        public void Stop()
        {
            if (_tokenSource == null)
            {
                return;
            }

            _tokenSource.Cancel();
            _watching = false;
            _tokenSource = null;
        }

        public Task<IEnumerable<ExceptionEvent>> GetExceptionsFromToday() => GetTodayAsync<ExceptionEvent>();

        public Task<IEnumerable<RequestEvent>> GetLastHourFailedRequests() => GetLastHourAsync<RequestEvent>();

        public void WatchLast24Hour()
        {
            Clear();
            Stop();
            WatchExceptions();
        }

        public void WatchLastHour()
        {
            Clear();
            Stop();
            WatchExceptions(false);
        }

        public void Clear()
        {
            _events.Clear();
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        public void Remove(AppInsightEvent eventToRemove)
        {
            _events.Remove(eventToRemove);
            CountChanged?.Invoke(this, _events.Count);
        }

        private void InvokeBusyChanged(bool isBusy)
        {
            _isBusy = isBusy;
            BusyChanged?.Invoke(this, isBusy);
        }

        private void NewEventAction(AppInsightEvent appInsightEvent)
        {
            if (_events.Any(e => e.Id == appInsightEvent.Id))
            {
                return;
            }

            if (_events.Count >= Top)
            {
                var firstEvent = _events.First();
                _events.Remove(firstEvent);
                DeleteEvent?.Invoke(this, firstEvent);
            }

            _events.Add(appInsightEvent);
            NewEvent?.Invoke(this, appInsightEvent);
            _debounceService.Debounce(300, () =>
            {
                CountChanged?.Invoke(this, _events.Count());
            });
        }

        private void NewCpuPercentage(CpuUsageMetric metric)
        {
            if (metric == null)
            {
                return;
            }

            _cpuPourcentage = metric.CpuPercentage.Average;
            CpuPourcentageChanged?.Invoke(this, _cpuPourcentage);
        }

        private async Task<IEnumerable<T>> GetTodayAsync<T>()
            where T : AppInsightEvent
        {
            var events = new List<T>();

            using var exceptionPipeline = new EventPipeline<T>(GetUrl<T>(), (e) => events.Add(e), ApiKey);
            await exceptionPipeline.GetLastDay();
            var todayEvents = events.Where(e => e.Timestamp.ToLocalTime().Day == DateTime.Now.Day).OrderBy(e => e.Timestamp);
            foreach (var todayEvent in todayEvents)
            {
                _events.Add(todayEvent);
            }

            return todayEvents;
        }

        private async Task<IEnumerable<T>> GetLastHourAsync<T>()
            where T : AppInsightEvent
        {
            var events = new List<T>();

            using var exceptionPipeline = new EventPipeline<T>(GetUrl<T>(), (e) => events.Add(e), ApiKey);
            await exceptionPipeline.GetLastHour();

            return events;
        }

        private string GetUrl<T>()
            where T : AppInsightEvent
        {
            if (typeof(T) == typeof(ExceptionEvent))
            {
                return ExceptionsUrl;
            }
            else if (typeof(T) == typeof(RequestEvent))
            {
                return FailedRequestUrl;
            }
            else if (typeof(T) == typeof(TraceEvent))
            {
                return TracesUrl;
            }

            throw new NotSupportedException();
        }

        #endregion
    }
}
