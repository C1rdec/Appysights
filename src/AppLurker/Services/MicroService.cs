using System;
using System.Collections.Generic;
using System.Linq;
using AppLurker.Models;

namespace AppLurker.Services
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

        #endregion

        #region Properties

        public string Name => _name;

        public IEnumerable<AppInsightsService> Applications { get; init; }

        #endregion

        #region Methods

        public void Watch() => ForeachApplication((a) => a.Watch());

        public void Stop() => ForeachApplication((a) => a.Stop());

        public void Clear() => ForeachApplication((a) => a.Clear());

        public void GetLast24Hour() => ForeachApplication((a) => a.GetLast24Hour());

        public void GetLastHour() => ForeachApplication((a) => a.GetLastHour());

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
