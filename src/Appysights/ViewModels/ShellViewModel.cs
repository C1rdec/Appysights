using System;
using System.Reflection;
using System.Threading.Tasks;
using Appysights.Models;
using Appysights.Services;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Appysights.ViewModels
{
    public class ShellViewModel : Screen
    {
        #region Fields

        private bool _flyoutOpen;
        private bool _needUpdate;
        private string _flyoutHeader;
        private Position _flyoutPosition;
        private PropertyChangedBase _flyoutContent;
        private FlyoutService _flyoutService;
        private KeyboardService _keyboardService;
        private UpdateManagerService _updateManagerService;
        private DialogService _dialogService;

        #endregion

        #region Constructors

        public ShellViewModel(
            DashboardViewModel dashboard, 
            FlyoutService flyoutService, 
            ThemeService themeService, 
            KeyboardService keyboardService, 
            UpdateManagerService updateManagerSerivce,
            DialogService dialogService)
        {
            _updateManagerService = updateManagerSerivce;
            _flyoutService = flyoutService;
            _keyboardService = keyboardService;
            _dialogService = dialogService;
            _dialogService.Register(this);

            _flyoutService.ShowFlyoutRequested += FlyoutService_ShowFlyout;
            _flyoutService.CloseFlyoutRequested += FlyoutService_CloseFlyout;

            CurrentView = dashboard;
            themeService.Apply();
        }

        #endregion

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

        public bool NeedUpdate 
        {
            get 
            {
                return _needUpdate;
            } 

            private set 
            { 
                _needUpdate = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => UpToDate);
            } 
        }

        public bool UpToDate => !NeedUpdate;

        #endregion

        #region Methods

        public void ShowSettings()
        {
            this.ShowFlyout("Settings", IoC.Get<SettingsViewModel>(), Position.Right);
        }

        public async void UpdateApplication()
        {
            CurrentView = IoC.Get<SplashScreenViewModel>();
            NotifyOfPropertyChange(() => CurrentView);
            await _dialogService.ShowProgressAsync("Updating...", "Appysights will restart", UpdateCore());
        }

        public string Version
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        public void CloseFlyout()
        {
            FlyoutOpen = false;

            // We use the field since we dont want to notify the UI
            _flyoutContent = null;
        }

        public void ShowFlyout(string header, PropertyChangedBase content, Position position)
        {
            FlyoutHeader = header;
            FlyoutPosition = position;
            FlyoutContent = content;
            FlyoutOpen = true;
        }

        protected override async void OnViewLoaded(object view)
        {
            NeedUpdate = await _updateManagerService.CheckForUpdate();
            await Task.Delay(200);
            await _keyboardService.InstallAsync();
        }
        private void FlyoutService_ShowFlyout(object sender, FlyoutRequest e)
        {
            ShowFlyout(e.Header, e.Content, e.Position);
        }

        private void FlyoutService_CloseFlyout(object sender, EventArgs e)
        {
            CloseFlyout();
        }

        private async Task UpdateCore()
        {
            var needUpdate = await _updateManagerService.CheckForUpdate();
            if (needUpdate)
            {
                await _updateManagerService.Update();
            }
            else
            {
                needUpdate = false;
            }
        }

        #endregion
    }
}
