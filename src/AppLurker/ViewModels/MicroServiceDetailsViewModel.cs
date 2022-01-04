using System;
using Caliburn.Micro;
using AppLurker.Services;

namespace AppLurker.ViewModels
{
    public class MicroServiceDetailsViewModel: PropertyChangedBase, IDisposable
    {
        private MicroService _microService;

        public MicroServiceDetailsViewModel(MicroService microService)
        {
            _microService = microService;
            Production = new AppInsightsViewModel(_microService.Prod, Enums.EnvironmentType.Prod);
            Development = new AppInsightsViewModel(_microService.Dev, Enums.EnvironmentType.Dev);
        }

        public AppInsightsViewModel Production { get; init; }

        public AppInsightsViewModel Development { get; init; }

        public void Dispose()
        {
            Production?.Dispose();
            Development?.Dispose();
        }
    }
}
