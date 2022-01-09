using System;
using System.Linq;
using System.Text.RegularExpressions;
using Appysights.Models;
using Caliburn.Micro;

namespace Appysights.ViewModels
{
    public abstract class EventViewModelBase : Screen
    {
        private static readonly Regex VersionRegex = new Regex("v[0-9]");

        public EventViewModelBase(AppInsightEvent appEvent)
        {
            AppEvent = appEvent;

            if (appEvent is RequestEvent requestEvent)
            {
                var request = requestEvent.Request;
                Title = $"[{request.ResultCode}] {request.Name}";
                DescriptionOverview = appEvent.GetSimpleLocalDate();
                Description = requestEvent.Request.Url;
                ActionName = $"Duration: {Math.Round(requestEvent.Request.Duration, 2)}s";
            }
            else if (appEvent is ExceptionEvent exceptionEvent)
            {
                var exception = exceptionEvent.Exception;
                Title = exception.Type;

                var message = string.IsNullOrEmpty(exception.Message) ? exception.OuterMessage : exception.Message;
                DescriptionOverview = message;
                Description = message;
                ActionName = FormatActionName(AppEvent.CustomDimensions);
            }
        }

        public string Title { get; set; }

        public string DescriptionOverview { get; set; }

        public string Description { get; set; }

        public bool IsRequest => AppEvent is RequestEvent;

        public bool IsException => AppEvent is ExceptionEvent;

        public string DateTime => AppEvent.GetLocalDate();

        public string Id => AppEvent.Id.ToString();

        public string ActionName { get; set; }

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
            return string.Join(separator, paths.Skip(Math.Max(0, paths.Count - elementToTaKe)));
        }
    }
}
