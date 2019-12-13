using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            HomePageView home = new HomePageView();
            MenuPage.Content = home;
            CheckStreamProcessorStatus();
        }

        private void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            CloseConfirmView closeconf = new CloseConfirmView();
            closeconf.ShowDialog();
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonMenuHome_Click(object sender, RoutedEventArgs e)
        {
            HomePageView home = new HomePageView();
            MenuPage.Content = home;
        }

        private void ButtonMenuPeopleIn_Click(object sender, RoutedEventArgs e)
        {
            PeopleInPageView pin = new PeopleInPageView();
            MenuPage.Content = pin;
        }
    

        private void ButtonMenuAllPeople_Click(object sender, RoutedEventArgs e)
        {
            AllPeoplePageView apin = new AllPeoplePageView();
            MenuPage.Content = apin;
        }

        private void ButtonMenuReaders_Click(object sender, RoutedEventArgs e)
        {
            Readers apin = new Readers();
            MenuPage.Content = apin;


        }

        private void ButtonMenuCameras_Click(object sender, RoutedEventArgs e)
        {
            CamerasPageView cams = new CamerasPageView();
            MenuPage.Content = cams;
        }

        private void CheckStreamProcessorStatus()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!Models.Starter.ListStreamProcessors().Contains(Models.MyAWSConfigs.streamProcessorName))
                {
                    Models.Starter.CreateStreamProcessor();
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
