using System;
using System.Linq;
using System.Text.RegularExpressions;
using Appysights.Models;
using Caliburn.Micro;

namespace Appysights.ViewModels
{
    public abstract class EventViewModelBase : Screen
    {
        #region Fields

        private static readonly Regex VersionRegex = new Regex("v[0-9]");

        #endregion

        #region Constructors

        public EventViewModelBase(AppInsightEvent appEvent)
        {
            AppEvent = appEvent;

            if (appEvent is RequestEvent requestEvent)
            {
                var request = requestEvent.Request;
                Title = $"[{request.ResultCode}] {request.Name}";
                Description = requestEvent.Request.Url;
                ActionName = $"Duration: {Math.Round(requestEvent.Request.Duration, 2)}s";
            }
            else if (appEvent is ExceptionEvent exceptionEvent)
            {
                var exception = exceptionEvent.Exception;
                Title = exception.Type;

                var message = string.IsNullOrEmpty(exception.Message) ? exception.OuterMessage : exception.Message;
                Description = message;
                ActionName = FormatActionName(AppEvent.CustomDimensions);
            }
        }

        #endregion

        #region Properties

        public string Title { get; set; }

        public DateTime LocalDate => AppEvent.LocalDate;

        public string Description { get; set; }

        public bool IsRequest => AppEvent is RequestEvent;

        public bool IsException => AppEvent is ExceptionEvent;

        public string DateTime => AppEvent.GetLocalDate();

        public string SimpleDateTime => AppEvent.GetSimpleLocalDate();

        public string Id => AppEvent.Id.ToString();

        public string ActionName { get; set; }

        public bool IsLessThen3Hours
        {
            get
            {
                var maximumDate = System.DateTime.Now.AddHours(-3);
                var result = System.DateTime.Compare(maximumDate, AppEvent.LocalDate);
                return result > 0 ? false : true;
            }
        }

        protected AppInsightEvent AppEvent { get; private set; }

        private string FormatActionName(CustomDimensions customDimensions)
        {
            if (customDimensions == null)
            {
                return string.Empty;
            }

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

        #endregion
    }
}
