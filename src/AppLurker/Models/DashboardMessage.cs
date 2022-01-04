using System;
using AppLurker.Enums;
using AppLurker.Services;

namespace AppLurker.Models
{
    public class DashboardMessage
    {
        public DashboardMessage(Action closeCallback)
        {
            CloseCallback = closeCallback;
        }

        public EnvironmentType Type { get; set; }

        public MicroService MicroService { get; set; }

        public AppInsightEvent AppInsightEvent { get; set; }

        public Action CloseCallback  { get; set; }
    }
}
