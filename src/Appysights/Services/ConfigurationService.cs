using Appysights.Models;

namespace Appysights.Services
{
    public class ConfigurationService : AppdataServiceBase<Configuration>
    {
        #region Properties

        protected override string FileName => "Services.json";

        protected override string ImportFileExtension => ".sights";

        #endregion
    }
}