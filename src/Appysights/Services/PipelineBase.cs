using System.Collections.Generic;

namespace Appysights.Services
{
    public abstract class PipelineBase : HttpServiceBase
    {
        private string _apiKey;
        private Dictionary<string, string> _header;

        #region Constructors

        public PipelineBase(string apiKey)
        {
            _apiKey = apiKey;
            _header = new Dictionary<string, string>
            {
                { "X-Api-Key", _apiKey }
            };
        }

        #endregion

        #region Properties

        protected Dictionary<string, string> Headers => _header;

        #endregion

    }
}
