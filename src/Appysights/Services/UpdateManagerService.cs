using System;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Squirrel;

namespace Appysights.Services
{
    /// <summary>
    /// Represents the update manager.
    /// </summary>
    public class UpdateManagerService
    {
        #region Fields

        private static readonly string GithubUrl = "https://github.com/C1rdec/Appysights";
        private Timer _timer;

        #endregion

        #region Events

        public event EventHandler UpdateRequested;

        #endregion

        #region Methods

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>The task awaiter.</returns>
        public async Task Update()
        {
            using (var updateManager = await UpdateManager.GitHubUpdateManager(GithubUrl))
            {
                await updateManager.UpdateApp();
                UpdateManager.RestartApp();
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>True if needs update.</returns>
        public async Task<bool> CheckForUpdate()
        {
#if DEBUG
            return false;
#endif

#pragma warning disable CS0162
            try
            {
                using (var updateManager = await CreateUpdateManagerAsync())
                {
                    var information = await updateManager.CheckForUpdate();
                    return information.ReleasesToApply.Any();
                }
            }
            catch
            {
                return false;
            }
#pragma warning restore CS0162
        }

        public void HandleSquirrel()
        {
            SquirrelAwareApp.HandleEvents(onInitialInstall: OnInstall, onAppUninstall: OnUninstall);
        }

        public void Watch()
        {
            if (_timer != null)
            {
                DisposeTimer();
            }

            _timer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var needUpdate = await CheckForUpdate();
            if (needUpdate)
            {
                UpdateRequested?.Invoke(this, EventArgs.Empty);
                DisposeTimer();
            }
        }

        private void DisposeTimer()
        {
            _timer.Stop();
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Dispose();
            _timer = null;
        }

        private async void OnInstall(System.Version version)
        {
            using var updateManager = new Squirrel.UpdateManager(GithubUrl, "Appysights");
            await updateManager.CreateUninstallerRegistryEntry();
            updateManager.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private void OnUninstall(System.Version version)
        {
            using var updateManager = new Squirrel.UpdateManager(GithubUrl, "Appysights");
            updateManager.RemoveUninstallerRegistryEntry();
            updateManager.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private Task<UpdateManager> CreateUpdateManagerAsync() => UpdateManager.GitHubUpdateManager(GithubUrl);

        #endregion
    }
}
