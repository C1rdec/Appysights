namespace Appysights.Models
{
    public class MenuItem : IMenuItem
    {
        #region Fields

        private string _title;

        #endregion

        #region Constructors

        public MenuItem(string title)
        {
            _title = title;
        }

        #endregion

        public string Label => "label";

        public string Title => _title;

        public IconPack Icon => null;
    }
}
