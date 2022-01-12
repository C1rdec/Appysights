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

        public Task GetLastHour() => GetEvents(LastHourUrl(1));

        public Task GetLastDay() => GetEvents(LastDayUrl(1));

        private async Task GetEvents(string url)
        {
            var text = await GetText(url, _header);
            var response = JsonHelper.Deserialize<AzureResponse<T>>(text);
            HandleEvents(response.Value);
        }

        private void HandleEvents(List<T> appInsightEvents)
        {
            var newEvents = new List<T>();
            if (_lastEvent == null)
            {
                _lastEvent = appInsightEvents.FirstOrDefault();
                newEvents.AddRange(appInsightEvents);
            }
            else
            {
                var currentLastEvent = appInsightEvents.FirstOrDefault(a => a.Id == _lastEvent.Id);
                if (currentLastEvent == null)
                {
                    return;
                }

                var index = appInsightEvents.IndexOf(currentLastEvent);
                if (index == 0 || index == -1)
                {
                    return;
                }

                _lastEvent = appInsightEvents.FirstOrDefault();
                newEvents.AddRange(appInsightEvents.Take(index));
                newEvents.Reverse();
            }

            foreach (var newEvent in newEvents)
            {
                _callback?.Invoke(newEvent);
            }
        }

        private string LastDayUrl(int count) => $"{_urlBase}{_variableCharacter}timespan=P{count}D";

        private string LastHourUrl(int count) => $"{_urlBase}{_variableCharacter}timespan=PT{count}H";

        #endregion
    }
}
