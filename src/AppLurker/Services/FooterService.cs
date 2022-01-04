using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppLurker.Models;

namespace AppLurker.Services
{
    public class FooterService
    {
        #region Fields

        private AppInsightsService _appInsightsService;
        private List<ExceptionEvent> _exceptions;
        private Task _initializeTask;

        #endregion

        public FooterService(AppInsightsService appInsightsService)
        {
            _exceptions = new List<ExceptionEvent>();
            _appInsightsService = appInsightsService;
            _initializeTask = InitializeCore();
        }

        public string Name => _appInsightsService.Name;

        public bool HasException => _exceptions.Any();

        public Task InitializeAsync() => _initializeTask;

        private async Task InitializeCore()
        {
            var exceptions = await _appInsightsService.GetTodayAsync<ExceptionEvent>();
            _exceptions.AddRange(exceptions);
        }
    }
}
