using System;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using AppLurker.Models;
using AppLurker.Services;
using System.Threading.Tasks;
using System.Threading;

namespace AppLurker.ViewModels
{
    public class ShellViewModel : Screen
    {
        private bool _flyoutOpen;
        private string _flyoutHeader;
        private Position _flyoutPosition;
        private PropertyChangedBase _flyoutContent;
        private FlyoutService _flyoutService;
        private KeyboardService _keyboardService;

        public ShellViewModel(DashboardViewModel dashboard, FlyoutService flyoutService, ThemeService themeService, KeyboardService keyboardService)
        {
            _flyoutService = flyoutService;
            _keyboardService = keyboardService;

            _flyoutService.ShowFlyoutRequested += FlyoutService_ShowFlyout;
            _flyoutService.CloseFlyoutRequested += FlyoutService_CloseFlyout;

            CurrentView = dashboard;
            themeService.Apply();
        }

        #region Properties

        public PropertyChangedBase CurrentView { get; set; }

        public StatusbarViewModel Statusbar { get; set; }

        public bool FlyoutOpen
        {
            get
            {
                return _flyoutOpen;
            }

            set
            {
                if (!value)
                {
                    _flyoutService.NotifyFlyoutClosed();
                }

                _flyoutOpen = value;
                NotifyOfPropertyChange(() => FlyoutOpen);
            }
        }

        public Position FlyoutPosition
        {
            get
            {
                return _flyoutPosition;
            }

            set
            {
                _flyoutPosition = value;
                NotifyOfPropertyChange(() => FlyoutPosition);
            }
        }

        public PropertyChangedBase FlyoutContent
        {
            get
            {
                return _flyoutContent;
            }

            set
            {
                _flyoutContent = value;
                NotifyOfPropertyChange(() => FlyoutContent);
            }
        }

        public string FlyoutHeader
        {
            get
            {
                return _flyoutHeader;
            }

            set
            {
                _flyoutHeader = value;
                NotifyOfPropertyChange(() => FlyoutHeader);
            }
        }

        #endregion

        #region Methods
        protected override async void OnViewLoaded(object view)
        {
            await Task.Delay(200);
            await _keyboardService.InstallAsync();
        }

        public void ChangeTheme()
        {
            this.ShowFlyout("Settings", IoC.Get<SettingsViewModel>(), Position.Right);
        }

        public void SetCurrentView(PropertyChangedBase viewModel)
        {
            CurrentView = viewModel;
            NotifyOfPropertyChange(() => CurrentView);
        }

        public void CloseFlyout()
        {
            FlyoutOpen = false;

            // We dont want to notify the UI
            // since the content will disapear
            _flyoutContent = null;
        }

        public void ShowFlyout(string header, PropertyChangedBase content, Position position)
        {
            FlyoutHeader = header;
            FlyoutPosition = position;
            FlyoutContent = content;
            FlyoutOpen = true;
        }

        private void FlyoutService_ShowFlyout(object sender, FlyoutRequest e)
        {
            ShowFlyout(e.Header, e.Content, e.Position);
        }

        private void FlyoutService_CloseFlyout(object sender, EventArgs e)
        {
            CloseFlyout();
        }

        #endregion
    }
}
