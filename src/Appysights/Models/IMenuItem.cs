namespace Appysights.Models
{
    public interface IMenuItem
    {
        public string Label { get; }

        public string Title { get; }

        public object Icon { get; }
    }
}
