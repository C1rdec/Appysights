using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Appysights.Models;
using Appysights.Services;
using Appysights.Views;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Appysights.ViewModels
{
    public class ShellViewModel : Screen
    {
        #region Fields

        private ShellView _view;
        private bool _flyoutOpen;
        private bool _needUpdate;
        private string _flyoutHeader;
        private Position _flyoutPosition;
        private PropertyChangedBase _flyoutContent;
        private FlyoutService _flyoutService;
        private KeyboardService _keyboardService;
        private UpdateManagerService _updateManagerService;
        private DialogService _dialogService;
        private ConfigurationManager _manager;

        #endregion

        #region Constructors

        public ShellViewModel(
            ConfigurationManager manager,
            FlyoutService flyoutService, 
            ThemeService themeService, 
            KeyboardService keyboardService, 
            UpdateManagerService updateManagerSerivce,
            DialogService dialogService)
        {
            _manager = manager;
            _updateManagerService = updateManagerSerivce;
            _flyoutService = flyoutService;
            _keyboardService = keyboardService;
            _dialogService = dialogService;
            _dialogService.Register(this);

            _flyoutService.ShowFlyoutRequested += FlyoutService_ShowFlyout;
            _flyoutService.CloseFlyoutRequested += FlyoutService_CloseFlyout;
            _manager.NewConfiguration += Manager_NewConfiguration;

            var viewModels = manager.Configurations.Select(c => new DashboardViewModel(c.Entity));
            var options = new List<IMenuItem>()
            {
                new MenuItem("New", "ShapeSquareRoundedPlus", AddNewConfiguration),
                new MenuItem("Settings", "CogOutline", () => ShowSettings()),
            };
            Menu = new HamburgerSelectorViewModel(viewModels, options, OnMenuClick);
            Menu.PropertyChanged += Menu_PropertyChanged;
            if (viewModels.Any())
            {
                var first = viewModels.FirstOrDefault();
                first.Initialize();
                CurrentView = first;
            }
            else
            {
                CurrentView = new ConfigurationSelectorViewModel(manager, OnNewConfiguration);
            }

            themeService.Apply();
            DisplayName = string.Empty;
        }

        private void Manager_NewConfiguration(object sender, ConfigurationService e)
        {
            Menu.AddItem(new DashboardViewModel(e.Entity));
        }

        private void AddNewConfiguration()
        {
            _manager.Add(null);
        }

        private void Menu_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
            {
                NotifyOfPropertyChange(() => MenuVisible);
            }
        }

        #endregion

        #region Properties

        public HamburgerSelectorViewModel Menu { get; set; }

        public object CurrentView { get; set; }

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

        public bool MenuVisible => Menu.IsVisible;

        #endregion

        #region Methods

        public void ShowReleasePage()
        {
            var targetURL = $"https://github.com/C1rdec/Appysights/releases/tag/v{Version}";
            var psi = new ProcessStartInfo
            {
                FileName = targetURL,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

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
            _view = view as ShellView;
            NeedUpdate = await _updateManagerService.CheckForUpdate();
            if (!NeedUpdate)
            {
                _updateManagerService.UpdateRequested += UpdateManagerService_UpdateRequested;
                _updateManagerService.Watch();
            }

            await Task.Delay(200);
            await _keyboardService.InstallAsync();
        }

        private void UpdateManagerService_UpdateRequested(object sender, EventArgs e)
        {
            NeedUpdate = true;
        }

        private void FlyoutService_ShowFlyout(object sender, FlyoutRequest e)
        {
            if (Menu != null)
            {
                // need to remove the width of the menu
                _view.SetFlyoutWidth(e.Position);
            }

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
        }

        private void OnMenuClick(IMenuItem t)
        {
            if (t is IResponsive responsive)
            {
                responsive.Initialize();
            }

            if (CurrentView is IResponsive r)
            {
                r.Deactivate();
            }

            CurrentView = t;
            

            NotifyOfPropertyChange(() => CurrentView);
        }

        private void OnNewConfiguration()
        {
            var firstConfig = _manager.Configurations.FirstOrDefault();
            if (firstConfig != null)
            {
                CurrentView = new DashboardViewModel(firstConfig.Entity);
                NotifyOfPropertyChange(() => CurrentView);
            }
        }

        #endregion
    }
}
