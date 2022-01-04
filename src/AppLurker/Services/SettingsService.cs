using AppLurker.Enums;
using AppLurker.Models;

namespace AppLurker.Services
{
    public class SettingsService: AppdataServiceBase<Settings>
    {
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
    }
}
