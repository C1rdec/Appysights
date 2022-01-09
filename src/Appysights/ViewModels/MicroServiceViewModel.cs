using System;
using System.Collections.Generic;
using System.Linq;
using Appysights.Services;
using Caliburn.Micro;

namespace Appysights.ViewModels
{
    public class MicroServiceViewModel : PropertyChangedBase, IDisposable
    {
        #region Fields

        private bool _selected;
        private bool _hasEvents;
        private MicroService _service;
        private Action<MicroServiceViewModel> _callback;

        #endregion

        #region Constructors

        public MicroServiceViewModel(MicroService microService, Action<MicroServiceViewModel> callback)
        {
            _callback = callback;
            _service = microService;
            ServiceName = microService.Name;

            var viewModels = new List<SimpleAppInsightsViewModel>();
            foreach (var application in microService.Applications)
            {
                var viewModel = new SimpleAppInsightsViewModel(application);
                viewModel.PropertyChanged += this.ViewModel_PropertyChanged;
                viewModels.Add(viewModel);
            }

            Applications = viewModels;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ErrorCountValue")
            {
                this.HasEvents = Applications.Any(a => a.ErrorCount > 0);
            }
        }

        #endregion

        #region Properties

        public string ServiceName { get; set; }

        public IEnumerable<SimpleAppInsightsViewModel> Applications { get; set; }

        public MicroService Service => _service;

        public bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;
                NotifyOfPropertyChange(() => Selected);
            }
        }

        public bool HasEvents
        {
            get
            {
                return _hasEvents;
            }

            set
            {
                _hasEvents = value;
                NotifyOfPropertyChange(() => HasEvents);
            }
        }

        #endregion

        #region Methods

        public void GetLastHour()
        {
            _service.GetLastHour();
        }

        public void GetLast24Hour()
        {
            _service.GetLast24Hour();
        }

        public void Clear()
        {
            _service.Clear();
        }

        public void OnClick()
        {
            _callback(this);
        }

        public void Dispose()
        {
            foreach (var application in Applications)
            {
                application.PropertyChanged -= this.ViewModel_PropertyChanged;
                application.Dispose();
            }
        }

        #endregion
    }
}
