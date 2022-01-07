﻿using System;
using Appysights.Services;
using MahApps.Metro.Controls;

namespace Appysights.Models
{
    public class DashboardMessage
    {
        public DashboardMessage(Action closeCallback)
        {
            CloseCallback = closeCallback;
        }

        public Position Position { get; set; }

        public MicroService MicroService { get; set; }

        public AppInsightEvent AppInsightEvent { get; set; }

        public Action CloseCallback  { get; set; }
    }
}