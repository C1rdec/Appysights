using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace AppLurker.Models
{
    public class FlyoutRequest
    {
        #region Properties

        public string Header { get; set; }

        public Position Position { get; set; }

        public PropertyChangedBase Content { get; set; }

        #endregion
    }
}
