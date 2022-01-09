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

        #endregion

        #region Constructors

        public SimpleAppInsightsViewModel(AppInsightsService service)
        {
            _service = service;
            _debounceService = new DebounceService();
            ApplicationName = service.Name;

            _service.NewEvent += this.Service_NewEvent;
            _service.Cleared += this.Service_Cleared;
        }

        #endregion

        #region Properties

        public string ApplicationName { get; set; }

        public int ErrorCount => _service.Events.Count();

        public string ErrorCountValue => ErrorCount == 0 ? "-" : ErrorCount.ToString();

        #endregion

        #region Methods

        public void Dispose()
        {
            _service.Cleared -= this.Service_Cleared;
            _service.NewEvent -= this.Service_NewEvent;
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

        #endregion
    }
}
