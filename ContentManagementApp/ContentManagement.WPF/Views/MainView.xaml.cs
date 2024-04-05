using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ContentManagement.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            var menuButton = (Button)sender;

            switch (menuButton.Name)
            {
                case "btnAdministration":
                    SubMenuCollapsed(stkSubAdministration);
                    break;
                case "btnAccount":
                    SubMenuCollapsed(stkSubAccount);
                    break;
            }
        }

        private void SubMenuCollapsed(StackPanel stackPanel)
        {
            if (stackPanel.Visibility == Visibility.Collapsed)
            {
                stackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                stackPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}