using System;
using Appysights.Services;

namespace Appysights.ViewModels
{
    public class ConfigurationSelectorViewModel
    {
        private ConfigurationManager _configurationManager;
        private Action _onSuccess;

        public ConfigurationSelectorViewModel(ConfigurationManager manager, Action onSuccess)
        {
            _configurationManager = manager;
            _onSuccess = onSuccess;
        }

        public void Add()
        {
            _configurationManager.Add(null);
            _onSuccess?.Invoke();
        }
    }
}
