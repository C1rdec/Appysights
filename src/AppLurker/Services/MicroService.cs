using AppLurker.Models;

namespace AppLurker.Services
{
    public class MicroService
    {
        #region Fields

        private string _name;
        private AppInsightsService _devService;
        private AppInsightsService _prodSerivce;

        #endregion

        #region Constructors

        public MicroService(MicroServiceConfiguration configuration)
        {
            _name = configuration.Name;
            _devService = new AppInsightsService(configuration.Dev);
            _prodSerivce = new AppInsightsService(configuration.Prod);
        }

        #endregion

        #region Properties

        public string Name => _name;

        public AppInsightsService Prod => _prodSerivce;

        public AppInsightsService Dev => _devService;

        #endregion

        #region Methods

        public void Watch()
        {
            _devService.Watch();
            _prodSerivce.Watch();
        }

        public void Stop()
        {
            _devService.Stop();
            _prodSerivce.Stop();
        }

        public void Clear()
        {
            _devService.Clear();
            _prodSerivce.Clear();
        }

        public void GetLastDay()
        {
            _devService.GetLastDay();
            _prodSerivce.GetLastDay();
        }

        public void GetLastHour()
        {
            _devService.GetLastHour();
            _prodSerivce.GetLastHour();
        }

        #endregion
    }
}
