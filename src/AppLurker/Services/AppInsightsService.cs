using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppLurker.Models;

namespace AppLurker.Services
{
    public class AppInsightsService
    {
        #region Fields

        private CancellationTokenSource _tokenSource;
        private bool _watching;
        private List<AppInsightEvent> _events;
        private AppInsightsConfiguration _configuration;

        #endregion

        #region Constructors

        public AppInsightsService(AppInsightsConfiguration configuration)
        {
            _configuration = configuration;
            _events = new List<AppInsightEvent>();
        }

        #endregion

        #region Properties

        public string Name => _configuration.Name;

        public IEnumerable<AppInsightEvent> Events => _events;

        private string ApplicationId => _configuration.ApplicationId;

        private string ApiKey => _configuration.ApiKey;

        private static int Top => 250;

        private string BaseUrl => $"https://api.applicationinsights.io/v1/apps/{ApplicationId}";

        private string MetricsUrl => $"{BaseUrl}/metrics";

        private string EventsUrl => $"{BaseUrl}/events";

        private string TracesUrl => $"{EventsUrl}/traces?$top=5000";

        private string RequestsUrl => $"{EventsUrl}/requests";

        private string ExceptionsUrl => $"{EventsUrl}/exceptions?$top=5000";

        private string FailedRequestUrl => $"{RequestsUrl}?$filter=(request%2Fsuccess%20eq%20'False')&$top={Top}";

        #endregion

        #region Events

        public event EventHandler<AppInsightEvent> NewEvent;

        public event EventHandler Cleared;

        #endregion

        #region Methods

        public async void Watch(bool needInit = true)
        {
            if (_watching)
            {
                return;
            }

            _watching = true;
            //using var requestPipeline = new EventPipeline<RequestEvent>(FailedRequestUrl, NewFailedRequest, _apiKey);
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

                if (!initialize)
                {
                    initialize = true;
                    tasks.Add(exceptionPipeline.GetLastDay());
                }
                else
                {
                    tasks.Add(exceptionPipeline.GetLastHour());
                }

                try
                {
                    await Task.WhenAll(tasks);
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

        public async Task<IEnumerable<T>> GetTodayAsync<T>()
            where T : AppInsightEvent
        {
            var events = new List<T>();

            using var exceptionPipeline = new EventPipeline<T>(GetUrl<T>(), (e) => events.Add(e), ApiKey);
            await exceptionPipeline.GetLastDay();
            return events.Where(e => e.Timestamp.ToLocalTime().Day == DateTime.Now.Day).OrderBy(e => e.Timestamp);
        }

        public void GetLast24Hour()
        {
            Clear();
            Stop();
            Watch();
        }

        public void GetLastHour()
        {
            Clear();
            Stop();
            Watch(false);
        }

        public void Clear()
        {
            _events.Clear();
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        private void NewEventAction(AppInsightEvent appInsightEvent)
        {
            if (_events.Any(e => e.Id == appInsightEvent.Id))
            {
                return;
            }

            this._events.Add(appInsightEvent);
            NewEvent?.Invoke(this, appInsightEvent);
        }

        private string GetUrl<T>()
            where T : AppInsightEvent
        {
            if (typeof(T) == typeof(ExceptionEvent))
            {
                return ExceptionsUrl;
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
