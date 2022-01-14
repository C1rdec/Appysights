using MahApps.Metro.Controls;

namespace Appysights.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {
        public ShellView()
        {
            InitializeComponent();
        }

        public void SetFlyoutWidth(Position position)
        {
            // Call me magic!
            MyFlyout.Width = FlyoutWidth.ActualWidth;
            if (position == Position.Right)
            {
                MyFlyout.Width -= 25;
            }

            if (position == Position.Left)
            {
                MyFlyout.Width += 6;
            }
        }
    }
}
