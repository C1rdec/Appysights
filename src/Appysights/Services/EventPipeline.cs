using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appysights.Helper;
using Appysights.Models;

namespace Appysights.Services
{
    public class EventPipeline<T> : HttpServiceBase
        where T : AppInsightEvent
    {
        #region Fields

        private string _apiKey;
        private string _urlBase;
        private Action<T> _callback;
        private T _lastEvent;
        private Dictionary<string, string> _header;
        private bool _initialize;
        private readonly string _variableCharacter;

        #endregion

        #region Constructors

        public EventPipeline(string url, Action<T> callback, string apiKey)
        {
            _urlBase = url;
            _variableCharacter = _urlBase.Contains('?') ? "&" : "?";
            _callback = callback;
            _apiKey = apiKey;

            _header = new Dictionary<string, string>
            {
                { "X-Api-Key", _apiKey }
            };
        }

        #endregion

        #region Methods

        public Task Next()
        {
            if (!_initialize)
            {
                _initialize = true;
                return GetLastDay();
            }
            else
            {
                return GetLastHour();
            }
        }

        public Task GetLastHour() => GetEvents(LastHourUrl(1));

        public Task GetLastDay() => GetEvents(LastDayUrl(1));

        private async Task GetEvents(string url)
        {
            var text = await GetText(url, _header);

            var response = JsonHelper.Deserialize<AzureResponse<T>>(text);
            HandleEvents(response.Value);
        }

        private void HandleEvents(IEnumerable<T> appInsightEvents)
        {
            var lastEventSet = false;
            if (_lastEvent == null)
            {
                _lastEvent = appInsightEvents.FirstOrDefault();
                foreach (var appInsightEvent in appInsightEvents)
                {
                    _callback?.Invoke(appInsightEvent);
                }

                return;
            }

            foreach (var appInsightEvent in appInsightEvents)
            {
                if (appInsightEvent.Id == _lastEvent.Id)
                {
                    return;
                }

                if (!lastEventSet)
                {
                    lastEventSet = true;
                    _lastEvent = appInsightEvent;
                }

                _callback?.Invoke(appInsightEvent);
            }
        }

        private string LastDayUrl(int count) => $"{_urlBase}{_variableCharacter}timespan=P{count}D";

        private string LastHourUrl(int count) => $"{_urlBase}{_variableCharacter}timespan=PT{count}H";

        #endregion
    }
}