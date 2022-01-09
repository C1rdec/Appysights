using Appysights.Enums;
using Appysights.Models;

namespace Appysights.Services
{
    public class SettingsService: AppdataServiceBase<Settings>
    {
        #region Properties

        public Theme Theme
        {
            get
            {
                return Entity.Theme;
            }

            set
            {
                Entity.Theme = value;
            }
        }

        public Scheme Scheme
        {
            get
            {
                return Entity.Scheme;
            }

            set
            {
                Entity.Scheme = value;
            }
        }

        protected override string FileName => "Settings.json";

        #endregion
    }
}