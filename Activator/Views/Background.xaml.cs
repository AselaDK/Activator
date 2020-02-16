using System.Windows;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for Background.xaml
    /// </summary>
    public partial class Background : Window
    {
        public Background()
        {
            InitializeComponent();
            LoginView loginView = new LoginView();
            loginView.Show();
        }
    }
}
