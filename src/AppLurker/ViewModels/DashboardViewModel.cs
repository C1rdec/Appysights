using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppLurker.Models;
using AppLurker.Services;
using Caliburn.Micro;

namespace AppLurker.ViewModels
{
    public class DashboardViewModel : PropertyChangedBase, IHandle<DashboardMessage>, IHandle<ConfigChangedMessage>
    {
        #region Fields

        private List<MicroService> _services;
        private MicroServiceViewModel _selectedMicro;
        private DashboardMessage _lastMessage;
        private FlyoutService _flyoutService;
        private ConfigurationService _configurationService;

        #endregion

        #region Constructors

        public DashboardViewModel(ConfigurationService configurationService, IEventAggregator eventAggregator, FlyoutService flyoutService)
        {
            _configurationService = configurationService;
            _flyoutService = flyoutService;
            _services = new List<MicroService>();
            _flyoutService.FlyoutClosed += FlyoutService_FlyoutClosed;
            eventAggregator.SubscribeOnPublishedThread(this);

            Initialize();
        }

        private void FlyoutService_FlyoutClosed(object sender, System.EventArgs e)
        {
            if (_lastMessage != null)
            {
                _lastMessage.CloseCallback?.Invoke();
                _lastMessage = null;
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<MicroServiceViewModel> MicroServices { get; set; }

        public MicroServiceDetailsViewModel SelectedMicroService { get; set; }

        public StatusbarViewModel Statusbar { get; set; }

        public bool HasConfiguration
        {
            get
            {
                var configuration = _configurationService.Entity;
                return configuration.Services.Any() || configuration.Footer != null;
            }
        }

        public bool HasNoConfiguration => !HasConfiguration;

        #endregion

        #region Methods

        public void ImportConfiguration()
        {
            _configurationService.Import(() => 
            {
                Initialize();
            });
        }

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

        public void GetLastDay()
        {
            ClearSelection();
            foreach (var micro in MicroServices)
            {
                micro.Clear();
                micro.GetLastDay();
            }
        }

        public Task HandleAsync(DashboardMessage message, CancellationToken cancellationToken)
        {
            if (_lastMessage != null)
            {
                _lastMessage.CloseCallback?.Invoke();
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

        private void Initialize()
        {
            InitializeServices();
            InitializeStatusbar();

            NotifyOfPropertyChange(() => HasConfiguration);
            NotifyOfPropertyChange(() => HasNoConfiguration);
        }

        private void InitializeServices()
        {
            MicroServices = new ObservableCollection<MicroServiceViewModel>();
            foreach (var config in _configurationService.Entity.Services)
            {
                var micro = new MicroService(config);
                _services.Add(micro);

                MicroServices.Add(new MicroServiceViewModel(micro, OnMicroServiceClick));

                micro.Watch();
            }

            NotifyOfPropertyChange(() => MicroServices);
        }

        private void InitializeStatusbar()
        {
            var footerConfiguration = _configurationService.Entity.Footer;
            if (footerConfiguration == null)
            {
                Statusbar = null;
            }
            else
            {
                Statusbar = new StatusbarViewModel(new StatusBatService(new AppInsightsService(footerConfiguration)), OnStatusBarClick);
            }

            this.NotifyOfPropertyChange(() => Statusbar);
        }

        public void OnStatusBarClick()
        {
            _flyoutService.Close();

            var configuration = new MicroServiceConfiguration();
            configuration.Applications.Add(_configurationService.Entity.Footer);
            HandleMicroService(new MicroService(configuration));
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

            SelectedMicroService = null;
            NotifyOfPropertyChange(() => SelectedMicroService);
        }

        private void HandleMicroService(MicroService microService)
        {
            SelectedMicroService?.Dispose();

            SelectedMicroService = new MicroServiceDetailsViewModel(microService);
            NotifyOfPropertyChange(() => SelectedMicroService);
        }

        public Task HandleAsync(ConfigChangedMessage message, CancellationToken cancellationToken)
        {
            Initialize();
            return Task.CompletedTask;
        }

        #endregion
    }
}
