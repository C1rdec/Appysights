using System;

namespace Appysights.Models
{
    public interface IMenuItem
    {
        public string Title { get; }

        public object Icon { get; }

        public Action OnClick { get; }
    }
}
