using Appysights.Enums;

namespace Appysights.Models
{
    public class Settings
    {
        #region Constructors

        public Settings()
        {
            Theme = Theme.Dark;
            Scheme = Scheme.Steel;
        }

        #endregion

        #region Properties

        public Theme Theme { get; set; }

        public Scheme Scheme { get; set; }

        #endregion
    }
}
