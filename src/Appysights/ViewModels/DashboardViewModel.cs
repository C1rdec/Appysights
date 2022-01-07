﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appysights.Models;
using Appysights.Services;
using Caliburn.Micro;

namespace Appysights.ViewModels
{
    public class DashboardViewModel : PropertyChangedBase, IHandle<DashboardMessage>, IHandle<ConfigChangedMessage>
    {
        #region Fields

        private List<MicroService> _services;
        private StatusbarService _statusbarService;
        private MicroServiceViewModel _selectedMicro;
        private DashboardMessage _lastMessage;
        private FlyoutService _flyoutService;
        private ConfigurationService _configurationService;
        private KeyboardService _keyboardService;

        #endregion

        #region Constructors

        public DashboardViewModel(ConfigurationService configurationService, IEventAggregator eventAggregator, FlyoutService flyoutService, KeyboardService keyboardService)
        {
            _configurationService = configurationService;
            _flyoutService = flyoutService;
            _keyboardService = keyboardService;
            _services = new List<MicroService>();
            _flyoutService.FlyoutClosed += FlyoutService_FlyoutClosed;
            eventAggregator.SubscribeOnPublishedThread(this);

            Initialize();
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
                return configuration.Services.Any() || configuration.Statusbar != null;
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

        private void Initialize()
        {
            InitializeKeyboard();
            InitializeServices();
            InitializeStatusbar();

            NotifyOfPropertyChange(() => HasConfiguration);
            NotifyOfPropertyChange(() => HasNoConfiguration);
        }

        private void InitializeKeyboard()
        {
            _keyboardService.LeftPressed += this.KeyboardService_LeftPressed;
            _keyboardService.RightPressed += this.KeyboardService_RightPressed;
            _keyboardService.OnePressed += this.KeyboardService_OnePressed;
            _keyboardService.TwoPressed += this.KeyboardService_TwoPressed;
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
            var statusbarConfiguration = _configurationService.Entity.Statusbar;
            if (statusbarConfiguration == null)
            {
                Statusbar = null;
            }
            else
            {
                _statusbarService = new StatusbarService(new AppInsightsService(statusbarConfiguration));
                Statusbar = new StatusbarViewModel(_statusbarService, OnStatusBarClick);
            }

            this.NotifyOfPropertyChange(() => Statusbar);
        }

        public void OnStatusBarClick()
        {
            _flyoutService.Close();
            if (_selectedMicro != null)
            {
                _selectedMicro.Selected = false;
            }

            HandleMicroService(new MicroService(_statusbarService.AppService));
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

            SelectedMicroService?.Dispose();
            SelectedMicroService = null;
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
            Initialize();
            return Task.CompletedTask;
        }

        #endregion
    }
}