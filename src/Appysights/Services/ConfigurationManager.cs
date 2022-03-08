using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Appysights.Services
{
    public class ConfigurationManager : AppysightFileBase<object>
    {
        public static string ConfigurationFolderName = "Configurations";

        private List<ConfigurationService> _configurations;

        public ConfigurationManager()
        {
            _configurations = new List<ConfigurationService>();
        }

        public event EventHandler<ConfigurationService> AddedConfiguration;

        public event EventHandler<ConfigurationService> RemovedConfiguration;

        public IEnumerable<ConfigurationService> Configurations => _configurations;

        protected override string FileName => "Manager.json";

        #region Methods

        public override void Initialize()
        {
            base.Initialize();

            var configFolderPath = Path.Combine(FolderPath, ConfigurationFolderName);
            if (!Directory.Exists(configFolderPath))
            {
                Directory.CreateDirectory(configFolderPath);
            }

            var files = Directory.GetFiles(Path.Combine(FolderPath, ConfigurationFolderName));
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var config = new ConfigurationService(name);
                config.Initialize();
                _configurations.Add(config);
            }
        }

        public void Add(Action onSuccess)
        {
            var config = new ConfigurationService(FileName);
            config.Import(() => OnSuccess(onSuccess, config));
        }

        private void OnSuccess(Action onSuccess, ConfigurationService config)
        {
            var existingConfiguration = _configurations.FirstOrDefault(c => c.Entity.Name == config.Entity.Name);
            if (existingConfiguration != null)
            {
                _configurations.Remove(existingConfiguration);
                RemovedConfiguration?.Invoke(this, config);
            }

            _configurations.Add(config);
            AddedConfiguration?.Invoke(this, config);
            onSuccess?.Invoke();
        }

        #endregion
    }
}
