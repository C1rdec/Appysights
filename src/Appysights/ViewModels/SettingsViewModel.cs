using System.Collections.Generic;
using Appysights.Enums;
using Appysights.Models;
using Appysights.Services;
using Caliburn.Micro;

namespace Appysights.ViewModels
{
    public class SettingsViewModel : PropertyChangedBase
    {
        #region Fields

        private ThemeService _themeService;
        private ConfigurationManager _configurationManager;
        private Scheme _selectedScheme;
        private Theme _selectedTheme;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        public SettingsViewModel(ThemeService themeService, ConfigurationManager configurationManager, IEventAggregator eventAggregator)
        {
            _themeService = themeService;
            _configurationManager = configurationManager;
            _selectedScheme = themeService.Scheme;
            _selectedTheme = themeService.Theme;
            _eventAggregator = eventAggregator;
            PropertyChanged += SettingsViewModel_PropertyChanged;
        }

        #endregion

        #region Properties

        public IEnumerable<Theme> Themes => _themeService.GetThemes();

        public IEnumerable<Scheme> Schemes => _themeService.GetSchemes();

        public Scheme SelectedScheme
        {
            get
            {
                return _selectedScheme;
            }

            set
            {
                _selectedScheme = value;
                NotifyOfPropertyChange();
            }
        }

        public Theme SelectedTheme
        {
            get
            {
                return _selectedTheme;
            }

            set
            {
                _selectedTheme = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        public void EditConfigurations()
        {
        }

        public void Random()
        {
            var result = _themeService.Random();
            SelectedScheme = result.Scheme;
            SelectedTheme = result.Theme;
        }

        private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _themeService.Change(SelectedTheme, SelectedScheme);
        }

        #endregion
    }
}
