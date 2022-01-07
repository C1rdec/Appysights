using System;

namespace Appysights.Models
{
    public class RequestEvent : AppInsightEvent
    {
        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        public AppRequest Request { get; set; }
    }
}
