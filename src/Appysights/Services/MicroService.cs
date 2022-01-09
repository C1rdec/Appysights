using System;
using System.Collections.Generic;
using System.Linq;
using Appysights.Models;

namespace Appysights.Services
{
    public class MicroService
    {
        #region Fields

        private string _name;

        #endregion

        #region Constructors

        public MicroService(MicroServiceConfiguration configuration)
        {
            _name = configuration.Name;
            Applications = new List<AppInsightsService>(configuration.Applications.Take(3).Select(a => new AppInsightsService(a)));
        }

        public MicroService(AppInsightsService appService)
        {
            _name = appService.Name;
            Applications = new List<AppInsightsService>() { appService };
        }

        #endregion

        #region Properties

        public string Name => _name;

        public IEnumerable<AppInsightsService> Applications { get; init; }

        #endregion

        #region Methods

        public void Watch() => ForeachApplication((a) => a.WatchExceptions());

        public void Stop() => ForeachApplication((a) => a.Stop());

        public void Clear() => ForeachApplication((a) => a.Clear());

        public void GetLast24Hour() => ForeachApplication((a) => a.WatchLast24Hour());

        public void GetLastHour() => ForeachApplication((a) => a.WatchLastHour());

        public void ForeachApplication(Action<AppInsightsService> callback)
        {
            foreach (var application in Applications)
            {
                callback(application);
            }
        }

        #endregion
    }
}
