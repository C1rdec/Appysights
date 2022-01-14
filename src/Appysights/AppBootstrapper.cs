using System;
using System.Collections.Generic;
using Appysights.Services;
using Appysights.ViewModels;
using Caliburn.Micro;

namespace Appysights
{
    public class AppBootstrapper : BootstrapperBase
    {
        #region Fields

        private SimpleContainer _container;
        private UpdateManagerService _updateManager;
        private ConfigurationManager _configurationManager;
        private SettingsService _settingsService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
        /// </summary>
        public AppBootstrapper()
        {
            _configurationManager = new ConfigurationManager();
            _configurationManager.Initialize();
            _settingsService = new SettingsService();
            _settingsService.Initialize();
            _updateManager = new UpdateManagerService();
            _updateManager.HandleSquirrel();
            Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            _container = new SimpleContainer();
            _container.Singleton<FlyoutService, FlyoutService>();
            _container.Singleton<ThemeService, ThemeService>(); 
            _container.Singleton<KeyboardService, KeyboardService>();
            _container.Singleton<SettingsViewModel, SettingsViewModel>();
            _container.Singleton<HamburgerSelectorViewModel, HamburgerSelectorViewModel>();
            _container.Singleton<SplashScreenViewModel, SplashScreenViewModel>();
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            _container.PerRequest<DialogService, DialogService>();
            _container.PerRequest<ShellViewModel, ShellViewModel>();


            _container.Instance(_configurationManager);
            _container.Instance(_settingsService);

            _container.Instance(Application);
            _container.Instance(_updateManager);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// The located service.
        /// </returns>
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>
        /// The located services.
        /// </returns>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        #endregion
    }
}
