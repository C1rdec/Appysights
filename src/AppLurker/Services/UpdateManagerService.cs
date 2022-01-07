using System.Linq;
using System.Threading.Tasks;
using Squirrel;

namespace AppLurker.Services
{
    /// <summary>
    /// Represents the update manager.
    /// </summary>
    public class UpdateManagerService
    {
        #region Fields

        private static readonly string GithubUrl = "https://github.com/C1rdec/App-Lurker";

        #endregion

        #region Constructors

        public UpdateManagerService()
        {
        }

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

        private async void OnInstall(System.Version version)
        {
            using var updateManager = new Squirrel.UpdateManager(GithubUrl, "AppLurker");
            await updateManager.CreateUninstallerRegistryEntry();
            updateManager.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private void OnUninstall(System.Version version)
        {
            using var updateManager = new Squirrel.UpdateManager(GithubUrl, "AppLurker");
            updateManager.RemoveUninstallerRegistryEntry();
            updateManager.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private Task<UpdateManager> CreateUpdateManagerAsync() => UpdateManager.GitHubUpdateManager(GithubUrl);

        #endregion
    }
}
