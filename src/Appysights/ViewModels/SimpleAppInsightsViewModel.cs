using System;
using System.Linq;
using Appysights.Services;
using Caliburn.Micro;

namespace Appysights.ViewModels
{
    public class SimpleAppInsightsViewModel : PropertyChangedBase, IDisposable
    {
        #region Fields

        private AppInsightsService _service;
        private DebounceService _debounceService;
        private bool _isBusy;

        #endregion

        #region Constructors

        public SimpleAppInsightsViewModel(AppInsightsService service)
        {
            _service = service;
            _debounceService = new DebounceService();
            ApplicationName = service.Name;

            _service.BusyChanged += Service_BusyChanged;
            _service.Cleared += Service_Cleared;
            _service.CountChanged += Service_CountChanged;
        }

        #endregion

        #region Properties

        public string ApplicationName { get; set; }

        public int ErrorCount => _service.Events.Count();

        public string ErrorCountValue => ErrorCount == 0 ? "-" : ErrorCount.ToString();

        public bool IsReady => !IsBusy;

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            private set
            {
                _isBusy = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => IsReady);
            }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            _service.Cleared -= this.Service_Cleared;
            _service.BusyChanged -= this.Service_BusyChanged;
            _service.CountChanged -= Service_CountChanged;
        }

        private void Service_NewEvent(object sender, Models.AppInsightEvent e)
        {
            _debounceService.Debounce(800, () =>
            {
                NotifyOfPropertyChange(() => ErrorCountValue);
            });
        }

        private void Service_Cleared(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => ErrorCountValue);
        }

        private void Service_BusyChanged(object sender, bool e)
        {
            IsBusy = e;
        }

        private void Service_CountChanged(object sender, int e)
        {
            NotifyOfPropertyChange(() => ErrorCountValue);
        }

        #endregion
    }
}
