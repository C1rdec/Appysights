using System;
using System.Threading.Tasks;
using Appysights.Helper;
using Appysights.Models;

namespace Appysights.Services
{
    public class MetricPipeline : PipelineBase
    {
        #region Fields

        private string _url;
        private Action<CpuUsageMetric> _callback;

        #endregion

        #region Constructors

        public MetricPipeline(string url, Action<CpuUsageMetric> callback, string apiKey) 
            : base(apiKey)
        {
            _url = url;
            _callback = callback;
        }

        #endregion

        #region Methods

        public async Task GetPourcentage()
        {
            var text = await GetText(_url, Headers);
            var response = JsonHelper.Deserialize<SingleAzureReponse<CpuUsageMetric>>(text);
            _callback(response.Value);
        }

        #endregion
    }
}
