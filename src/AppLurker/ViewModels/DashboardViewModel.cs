using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppLurker.Enums;
using AppLurker.Models;
using AppLurker.Services;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace AppLurker.ViewModels
{
    public class DashboardViewModel : PropertyChangedBase, IHandle<DashboardMessage>
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

            InitializeServices();
            InitializeFooter();
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
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Lurk files (*.lurk)|*.lurk",
            };

            try
            {
                var result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    var text = System.IO.File.ReadAllText(dialog.FileName);
                    _configurationService.Save(text);

                    InitializeServices();
                    InitializeFooter();

                    NotifyOfPropertyChange(() => HasConfiguration);
                    NotifyOfPropertyChange(() => HasNoConfiguration);
                }
            }
            catch
            {
                // Notify the user
            }
        }

        public void Clear()
        {
            ClearSelection();

            foreach (var micro in MicroServices)
            {
                micro.Clear();
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

        public void GetLastHour()
        {
            ClearSelection();
            foreach (var micro in MicroServices)
            {
                micro.Clear();
                micro.GetLastHour();
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
                var position = message.Type == EnvironmentType.Dev ? Position.Left : Position.Right;
                _flyoutService.Show(header, new EventViewModel(message.AppInsightEvent), position);
            }

            return Task.CompletedTask;
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

        private void InitializeFooter()
        {
            var footerConfiguration = _configurationService.Entity.Footer;
            if (footerConfiguration == null)
            {
                return;
            }

            Statusbar = new StatusbarViewModel(new FooterService(new AppInsightsService(footerConfiguration)), OnStatusBarClick);
            this.NotifyOfPropertyChange(() => Statusbar);
        }

        public void OnStatusBarClick()
        {
        }

        private void OnMicroServiceClick(MicroServiceViewModel viewModel)
        {
            _flyoutService.Close();
            if (viewModel.Selected)
            {
                ClearSelection();
                return;
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
            if (_selectedMicro != null)
            {
                _selectedMicro.Selected = false;
                _selectedMicro = null;
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

        #endregion
    }
}
