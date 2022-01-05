using System;
using System.Collections.Generic;
using System.Linq;
using AppLurker.Services;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace AppLurker.ViewModels
{
    public class MicroServiceDetailsViewModel: PropertyChangedBase, IDisposable
    {
        public MicroServiceDetailsViewModel(MicroService microService)
        {
            var count = microService.Applications.Count();
            var applications = new List<AppInsightsViewModel>();
            for (int i = 0; i < count; i++)
            {
                applications.Add(new AppInsightsViewModel(microService.Applications.ElementAt(i), GetPosition(i, count)));
            }

            Applications = applications;
        }

        public IEnumerable<AppInsightsViewModel> Applications { get; init; }

        public void Dispose()
        {
            foreach (var application in Applications)
            {
                application.Dispose();
            }
        }

        private Position GetPosition(int index, int count)
        {
            var half = (int)Math.Ceiling((double)count / (double)2);
            if ((index + 1) > half)
            {
                return Position.Left;
            }
            else
            {
                return Position.Right;
            }
        }
    }
}
