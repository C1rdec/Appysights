using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AppLurker.Enums;

namespace AppLurker.Services
{
    public class ThemeService
    {
        private static Random _random = new();
        private Application _application;
        private SettingsService _settingsService;

        public ThemeService(Application application, SettingsService settingsService)
        {
            _settingsService = settingsService;
            _application = application;
        }

        public Theme Theme => _settingsService.Theme;

        public Scheme Scheme => _settingsService.Scheme;

        public IEnumerable<Scheme> GetSchemes() => GetEnumValues<Scheme>();

        public IEnumerable<Theme> GetThemes() => GetEnumValues<Theme>();

        public void Apply()
        {
            ControlzEx.Theming.ThemeManager.Current.ChangeTheme(_application, $"{_settingsService.Theme}.{_settingsService.Scheme}");
        }

        public void Change(Theme theme, Scheme scheme)
        {
            _settingsService.Scheme = scheme;
            _settingsService.Theme = theme;
            _settingsService.Save();
            ControlzEx.Theming.ThemeManager.Current.ChangeTheme(_application, $"{theme}.{scheme}");
        }

        public (Theme Theme, Scheme Scheme) Random()
        {
            var theme = RandomEnumValue<Theme>();
            var scheme = RandomEnumValue<Scheme>();
            Change(theme, scheme);

            return (theme, scheme);
        }

        private static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_random.Next(v.Length));
        }

        private static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray().OrderBy(t => t);
        }
    }
}
