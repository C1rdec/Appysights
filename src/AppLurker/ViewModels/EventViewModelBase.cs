using System;
using System.Linq;
using System.Text.RegularExpressions;
using AppLurker.Models;
using Caliburn.Micro;

namespace AppLurker.ViewModels
{
    public abstract class EventViewModelBase : PropertyChangedBase
    {
        private static readonly Regex VersionRegex = new Regex("v[0-9]");

        public EventViewModelBase(AppInsightEvent appEvent)
        {
            AppEvent = appEvent;

            if (appEvent is RequestEvent requestEvent)
            {
                var request = requestEvent.Request;
                Title = $"[{request.ResultCode}] {request.Name}";
                Description = request.Url;
            }
            else if (appEvent is ExceptionEvent exceptionEvent)
            {
                var exception = exceptionEvent.Exception;
                Title = exception.Type;
                Description = string.IsNullOrEmpty(exception.Message) ? exception.OuterMessage : exception.Message;
            }
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsRequest => AppEvent is RequestEvent;

        public bool IsException => AppEvent is ExceptionEvent;

        public string DateTime => AppEvent.GetLocalDate();

        public string Id => AppEvent.Id.ToString();

        public string ActionName => FormatActionName(AppEvent.CustomDimensions);

        protected AppInsightEvent AppEvent { get; private set; }

        private string FormatActionName(CustomDimensions customDimensions)
        {
            var actionName = customDimensions.ActionName;
            if (!string.IsNullOrEmpty(actionName))
            {
                // Remove namespace
                var startIndex = actionName.IndexOf(" (");
                if (startIndex >= 0)
                {
                    actionName = actionName.Substring(0, startIndex);
                }

                return GetLastElements(actionName, '.');
            }

            var requestPath = customDimensions.RequestPath;
            if (!string.IsNullOrEmpty(requestPath))
            {
                return GetLastElements(requestPath, '/');
            }

            return string.Empty;
        }

        private static string GetLastElements(string value, char separator)
        {
            var paths = value.Split(separator).ToList();
            var versionIndex = paths.FindIndex(s => VersionRegex.Match(s.ToLower()).Success);
            var elementToTaKe = paths.Count - versionIndex;
            return string.Join(separator, paths.Skip(Math.Max(0, paths.Count() - elementToTaKe)));
        }
    }
}
