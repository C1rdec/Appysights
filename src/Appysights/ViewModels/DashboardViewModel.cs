using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appysights.Models;
using Appysights.Services;
using Caliburn.Micro;
using MahApps.Metro.IconPacks;

namespace Appysights.ViewModels
{
    public class DashboardViewModel : PropertyChangedBase, IHandle<DashboardMessage>, IHandle<ConfigChangedMessage>, IMenuItem, IResponsive
    {
        #region Fields

        private List<MicroService> _services;
        private StatusbarService _statusbarService;
        private MicroServiceViewModel _selectedMicro;
        private DashboardMessage _lastMessage;
        private FlyoutService _flyoutService;
        private Configuration _configuration;
        private KeyboardService _keyboardService;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        public DashboardViewModel(Configuration configuration)
        {
            _configuration = configuration;
            _flyoutService = IoC.Get<FlyoutService>();
            _keyboardService = IoC.Get<KeyboardService>(); ;
            _services = new List<MicroService>();
            _eventAggregator = IoC.Get<IEventAggregator>();

            _flyoutService.FlyoutClosed += FlyoutService_FlyoutClosed;
            _eventAggregator.SubscribeOnPublishedThread(this);

            if (configuration.Statusbar != null)
            {
                _statusbarService = new StatusbarService(new AppInsightsService(configuration.Statusbar));
            }

            foreach (var config in _configuration.Services)
            {
                var micro = new MicroService(config);
                _services.Add(micro);
                micro.Watch();
            }

            MicroServices = new ObservableCollection<MicroServiceViewModel>();
        }


        #endregion

        #region Properties

        public string DashboardName => _configuration.Name;

        public ObservableCollection<MicroServiceViewModel> MicroServices { get; set; }

        public MicroServiceDetailsViewModel SelectedMicroService { get; set; }

        public StatusbarViewModel Statusbar { get; set; }

        public bool HasConfiguration
        {
            get
            {
                var configuration = _configuration;
                return configuration.Services.Any() || configuration.Statusbar != null;
            }
        }

        public bool HasNoConfiguration => !HasConfiguration;

        public bool HasStatusbarConfiguration => HasConfiguration && _configuration.Statusbar != null;

        public string Title => _configuration.Name;

        public object Icon => string.IsNullOrEmpty(_configuration.Icon) ? BuildDefaultIcon() : BuildIcon(_configuration.Icon);

        #endregion

        #region Methods

        public void Clear()
        {
            ClearSelection();

            foreach (var micro in MicroServices)
            {
                micro.Clear();
            }
        }

        public void GetLastHour()
        {
            ClearSelection();
            foreach (var micro in MicroServices)
            {
                micro.Clear();
                micro.GetLastHour();
            }
        }

        public void GetLast24Hour()
        {
            ClearSelection();
            foreach (var micro in MicroServices)
            {
                micro.Clear();
                micro.GetLast24Hour();
            }
        }

        public Task HandleAsync(DashboardMessage message, CancellationToken cancellationToken)
        {
            if (_lastMessage != null)
            {
                _lastMessage.CloseCallback?.Invoke();
            }

            if (message.RequestClose)
            {
                _flyoutService.Close();
                message.CloseCallback();
                return Task.CompletedTask;
            }

            _lastMessage = message;
            if (message.MicroService != null)
            {
                this.HandleMicroService(message.MicroService);
            }
            else if (message.AppInsightEvent != null)
            {
                var header = message.AppInsightEvent.GetLocalDate();
                _flyoutService.Show(header, new EventViewModel(message.AppInsightEvent), message.Position);
            }

            return Task.CompletedTask;
        }

        public void Initialize()
        {
            InitializeKeyboard();
            InitializeServices();
            InitializeStatusbar();

            NotifyOfPropertyChange(() => HasConfiguration);
            NotifyOfPropertyChange(() => HasNoConfiguration);
            NotifyOfPropertyChange(() => HasStatusbarConfiguration);
        }

        public void Deactivate()
        {
            ClearSelection();

            foreach(var micro in MicroServices)
            {
                micro.Dispose();
            }

            MicroServices.Clear();
            _keyboardService.LeftPressed -= this.KeyboardService_LeftPressed;
            _keyboardService.RightPressed -= this.KeyboardService_RightPressed;
            _keyboardService.OnePressed -= this.KeyboardService_OnePressed;
            _keyboardService.TwoPressed -= this.KeyboardService_TwoPressed;
        }

        private void InitializeKeyboard()
        {
            _keyboardService.LeftPressed += this.KeyboardService_LeftPressed;
            _keyboardService.RightPressed += this.KeyboardService_RightPressed;
            _keyboardService.OnePressed += this.KeyboardService_OnePressed;
            _keyboardService.TwoPressed += this.KeyboardService_TwoPressed;
        }

        private static object BuildDefaultIcon() => BuildIcon(PackIconMaterialKind.MicrosoftAzure);

        private static object BuildIcon(string kindValue)
        {
            if (Enum.TryParse<PackIconMaterialKind>(kindValue, out var icon))
            {
                return BuildIcon(icon);
            }
            else
            {
                return BuildDefaultIcon();
            }
        }

        private static object BuildIcon(PackIconMaterialKind kind)
        {
            return new PackIconMaterial() { Kind = kind, Width = 30, Height = 30 };
        }

        private void OnStatusBarClick()
        {
            _flyoutService.Close();
            if (Statusbar.Selected)
            {
                ClearSelectedMicroService();

                return;
            }

            if (_selectedMicro != null)
            {
                _selectedMicro.Selected = false;
            }

            HandleMicroService(new MicroService(_statusbarService.AppService));
        }

        private void KeyboardService_LeftPressed(object sender, System.EventArgs e)
        {
            var index = MicroServices.IndexOf(_selectedMicro);
            var previous = MicroServices.Take(index).Reverse().FirstOrDefault(m => m.HasEvents);
            if (previous != null)
            {
                OnMicroServiceClick(previous);
            }
        }

        private void KeyboardService_RightPressed(object sender, System.EventArgs e)
        {
            if (_selectedMicro == null)
            {
                var first = MicroServices.FirstOrDefault(m => m.HasEvents);
                if (first != null)
                {
                    OnMicroServiceClick(first);
                }
            }
            else
            {
                var index = MicroServices.IndexOf(_selectedMicro);
                var next = MicroServices.Skip(index + 1).FirstOrDefault(m => m.HasEvents);
                if (next != null)
                {
                    OnMicroServiceClick(next);
                }
            }
        }

        private void FlyoutService_FlyoutClosed(object sender, System.EventArgs e)
        {
            if (_lastMessage != null)
            {
                _lastMessage.CloseCallback?.Invoke();
                _lastMessage = null;
            }
        }

        private void KeyboardService_TwoPressed(object sender, System.EventArgs e)
        {
            Execute.OnUIThread(() => GetLast24Hour());
        }

        private void KeyboardService_OnePressed(object sender, System.EventArgs e)
        {
            Execute.OnUIThread(() => GetLastHour());
        }

        private void InitializeServices()
        {
            foreach (var service in _services)
            {
                MicroServices.Add(new MicroServiceViewModel(service, OnMicroServiceClick));
            }

            NotifyOfPropertyChange(() => MicroServices);
        }

        private void InitializeStatusbar()
        {
            if (_statusbarService == null)
            {
                Statusbar = null;
            }
            else
            {
                Statusbar = new StatusbarViewModel(_statusbarService, OnStatusBarClick);
            }

            this.NotifyOfPropertyChange(() => Statusbar);
        }

        private void OnMicroServiceClick(MicroServiceViewModel viewModel)
        {
            _flyoutService.Close();
            if (viewModel.Selected)
            {
                ClearSelection();
                return;
            }

            if (Statusbar != null)
            {
                Statusbar.Selected = false;
            }

            if (_selectedMicro != null)
            {
                _selectedMicro.Selected = false;
            }

            _selectedMicro = viewModel;
            _selectedMicro.Selected = true;
            HandleMicroService(viewModel.Service);
        }

        private void ClearSelection()
        {
            _flyoutService.Close();

            if (_selectedMicro != null)
            {
                _selectedMicro.Selected = false;
                _selectedMicro = null;
            }

            if (Statusbar != null)
            {
                Statusbar.Selected = false;
            }

            ClearSelectedMicroService();
        }

        private void ClearSelectedMicroService()
        {
            if (SelectedMicroService != null)
            {
                SelectedMicroService.Dispose();
                SelectedMicroService = null;
            }

            NotifyOfPropertyChange(() => SelectedMicroService);
        }

        private void HandleMicroService(MicroService microService)
        {
            SelectedMicroService?.Dispose();

            SelectedMicroService = new MicroServiceDetailsViewModel(microService, _keyboardService);
            NotifyOfPropertyChange(() => SelectedMicroService);
        }

        public Task HandleAsync(ConfigChangedMessage message, CancellationToken cancellationToken)
        {
            //Initialize();
            return Task.CompletedTask;
        }

        #endregion
    }
}
