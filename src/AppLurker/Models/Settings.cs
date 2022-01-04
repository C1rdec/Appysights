using AppLurker.Enums;

namespace AppLurker.Models
{
    public class Settings
    {
        public Settings()
        {
            Theme = Theme.Dark;
            Scheme = Scheme.Steel;
        }

        public Theme Theme { get; set; }

        public Scheme Scheme { get; set; }
    }
}
