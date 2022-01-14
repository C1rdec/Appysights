using System;
using MahApps.Metro.IconPacks;

namespace Appysights.Models
{
    public class MenuItem : IMenuItem
    {
        #region Fields

        private string _title;
        private object _icon;

        #endregion

        #region Constructors

        public MenuItem(string title)
        {
            _title = title;
            _icon = BuildIcon("ShapeSquareRoundedPlus");
        }

        #endregion

        public string Title => _title;

        public object Icon => _icon;

        private static object BuildIcon(string kindValue)
        {
            if (Enum.TryParse<PackIconMaterialKind>(kindValue, out var icon))
            {
                return BuildIcon(icon);
            }
            else
            {
                return null;
            }
        }

        private static object BuildIcon(PackIconMaterialKind kind)
        {
            return new PackIconMaterial() { Kind = kind, Width = 24, Height = 24};
        }
    }
}
