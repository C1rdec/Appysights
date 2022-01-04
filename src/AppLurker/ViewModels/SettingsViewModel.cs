using System.Collections.Generic;
using AppLurker.Enums;
using AppLurker.Services;

namespace AppLurker.ViewModels
{
    public class SettingsViewModel : Caliburn.Micro.PropertyChangedBase
    {
        private ThemeService _themeService;
        private Scheme _selectedScheme;
        private Theme _selectedTheme;

        public SettingsViewModel(ThemeService themeService)
        {
            _themeService = themeService;
            _selectedScheme = themeService.Scheme;
            _selectedTheme = themeService.Theme;
            PropertyChanged += SettingsViewModel_PropertyChanged;
        }

        private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _themeService.Change(SelectedTheme, SelectedScheme);
        }

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

        public void Random()
        {
            var result = _themeService.Random();
            SelectedScheme = result.Scheme;
            SelectedTheme = result.Theme;
        }
    }
}
