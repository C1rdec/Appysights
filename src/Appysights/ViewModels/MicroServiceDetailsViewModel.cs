using System;
using System.Collections.Generic;
using System.Linq;
using Appysights.Services;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Appysights.ViewModels
{
    public class MicroServiceDetailsViewModel: PropertyChangedBase, IDisposable
    {
        private KeyboardService _keyboardService;

        public MicroServiceDetailsViewModel(MicroService microService, KeyboardService keyboardService)
        {
            _keyboardService = keyboardService;
            var count = microService.Applications.Count();
            var applications = new List<AppInsightsViewModel>();
            for (int i = 0; i < count; i++)
            {
                applications.Add(new AppInsightsViewModel(microService.Applications.ElementAt(i), GetPosition(i, count)));
            }

            Applications = applications;
            InitializeKeyboard();
        }

        public List<AppInsightsViewModel> Applications { get; init; }

        public void InitializeKeyboard()
        {
            _keyboardService.UpPressed += this.KeyboardService_UpPressed;
            _keyboardService.DownPressed += this.KeyboardService_DownPressed;
            _keyboardService.NextTabPressed += this.KeyboardService_NextTabPressed;
            _keyboardService.PreviousTabPressed += this.KeyboardService_PreviousTabPressed;
        }

        public void Dispose()
        {
            foreach (var application in Applications)
            {
                application.Dispose();
            }

            _keyboardService.UpPressed -= this.KeyboardService_UpPressed;
            _keyboardService.DownPressed -= this.KeyboardService_DownPressed;
            _keyboardService.NextTabPressed -= this.KeyboardService_NextTabPressed;
            _keyboardService.PreviousTabPressed -= this.KeyboardService_PreviousTabPressed;
        }

        private void KeyboardService_PreviousTabPressed(object sender, EventArgs e)
        {
            var selectedApplication = Applications.FirstOrDefault(a => a.Selected);
            if (selectedApplication == null)
            {
                return;
            }

            var index = Applications.IndexOf(selectedApplication);
            if (index == -1 || index - 1 < 0)
            {
                return;
            }

            Applications[index - 1].Next();
        }

        private void KeyboardService_NextTabPressed(object sender, EventArgs e)
        {
            var selectedApplication = Applications.FirstOrDefault(a => a.Selected);
            if (selectedApplication == null)
            {
                var first = Applications.FirstOrDefault(a => a.Events.Any());
                if (first != null)
                {
                    first.Next();
                }
            }
            else
            {
                var index = Applications.IndexOf(selectedApplication);
                if (index == -1 || index + 1 >= Applications.Count())
                {
                    return;
                }

                Applications[index + 1].Next();
            }
        }

        private void KeyboardService_DownPressed(object sender, EventArgs e)
        {
            var selectedApplication = Applications.FirstOrDefault(a => a.Selected);
            if (selectedApplication == null)
            {
                var first = Applications.FirstOrDefault(a => a.Events.Any());
                if (first != null)
                {
                    first.Next();
                }
            }
            else
            {
                selectedApplication.Next();
            }
        }

        private void KeyboardService_UpPressed(object sender, EventArgs e)
        {
            var selectedApplication = Applications.FirstOrDefault(a => a.Selected);
            if (selectedApplication == null)
            {
                return;
            }

            selectedApplication.Previous();
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
