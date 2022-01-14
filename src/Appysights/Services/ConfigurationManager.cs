﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Appysights.Services
{
    public class ConfigurationManager : AppdataServiceBase<object>
    {
        public static string ConfigurationFolderName = "Configurations";
        public static string OldConfigurationName = "Services.json";

        private List<ConfigurationService> _configurations;

        public ConfigurationManager()
        {
            _configurations = new List<ConfigurationService>();
        }

        public event EventHandler<ConfigurationService> NewConfiguration;


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

            HandleMigration(configFolderPath);

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
            config.Import(onSuccess);
            _configurations.Add(config);
            NewConfiguration?.Invoke(this, config);
        }

        private void HandleMigration(string configFolderPath)
        {
            var oldFilePath = Path.Combine(FolderPath, OldConfigurationName);
            if (File.Exists(oldFilePath))
            {
                var oldConfig = new ConfigurationService(oldFilePath);
                oldConfig.Initialize();
                oldConfig.Entity.Name = "Micro Services";
                oldConfig.Save();
                File.Delete(oldFilePath);
            }
        }

        #endregion
    }
}
