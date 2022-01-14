using Appysights.Models;

namespace Appysights.Services
{
    public class ConfigurationService : AppdataServiceBase<Configuration>
    {
        #region Fields

        private string _fileName;

        #endregion

        #region Constructors

        public ConfigurationService(string fileName)
        {
            _fileName = fileName;
        }

        #endregion

        #region Properties

        protected override string FileName => _fileName;

        protected override string SubFolderName => ConfigurationManager.ConfigurationFolderName;

        protected override string ImportFileExtension => ".sights";

        #endregion
    }
}
