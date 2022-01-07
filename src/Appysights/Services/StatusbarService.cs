using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appysights.Models;

namespace Appysights.Services
{
    public class StatusbarService
    {
        #region Fields

        private AppInsightsService _appInsightsService;
        private List<ExceptionEvent> _exceptions;
        private Task _initializeTask;

        #endregion

        public StatusbarService(AppInsightsService appInsightsService)
        {
            _exceptions = new List<ExceptionEvent>();
            _appInsightsService = appInsightsService;
            _initializeTask = InitializeCore();
        }

        public AppInsightsService AppService => _appInsightsService;

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
