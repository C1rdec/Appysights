using System.Linq;
using System.Threading.Tasks;
using Squirrel;

namespace AppLurker.Services
{
    internal class UpdateManagerService
    {
        /// <summary>
        /// Represents the update manager.
        /// </summary>
        public class UpdateManager
        {
            #region Fields

            private static readonly string GithubUrl = "";

            #endregion

            #region Constructors

            public UpdateManager()
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
                using (var updateManager = await Squirrel.UpdateManager.GitHubUpdateManager(GithubUrl))
                {
                    await updateManager.UpdateApp();
                    Squirrel.UpdateManager.RestartApp();
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
                    using (var updateManager = await Squirrel.UpdateManager.GitHubUpdateManager(GithubUrl))
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

            #endregion
        }
    }
}
