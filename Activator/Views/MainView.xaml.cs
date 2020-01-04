using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
using Activator.Models;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        private string myname = "Admin";
        private string myid = "Admin@111.com";

        public MainView()
        {
            InitializeComponent();

            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;

            lblTitle.Content = "HOME";

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                LoginView loginView = new LoginView();
                loginView.Close();
                HomePageView home = new HomePageView();
                MenuPage.Content = home;
                CheckStreamProcessorStatus();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }            
        }

        public MainView(String adminid, String adminname) : this()
        {
            this.myname = adminname;
            AdminName.Text = myname;
            string imagename = null;
            this.myid = adminid;
            S3Bucket.DownloadFile(myid);
            var BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = BaseDirectoryPath + $"Resources/Images/{imagename}";
            ImageSource imageSource = new BitmapImage(new Uri(filePath, UriKind.Relative));
            //MyPropicImage.Source = new BitmapImage(new Uri($@"\myserver\folder1\Customer Data\{myid}"));
            MyPropicImage.Source = imageSource;
            //Console.WriteLine(MyAdminName);
        }

        private void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            //CloseConfirmView closeconf = new CloseConfirmView();
            //closeconf.ShowDialog();
            InitDialog();
        }

        private async Task InitDialog()
        {
            var result = await this.ShowMessageAsync("Are you sure want to quit?", "", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative) Application.Current.Shutdown();
            
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
            lblTitle.Content = "HOME";
            
        }

        private void ButtonMenuPeopleIn_Click(object sender, RoutedEventArgs e)
        {
            PeopleInPageView pin = new PeopleInPageView();
            MenuPage.Content = pin;
            lblTitle.Content = "HISTORY";
            
        }
    

        private void ButtonMenuAllPeople_Click(object sender, RoutedEventArgs e)
        {
            AllPeoplePageView apin = new AllPeoplePageView();
            MenuPage.Content = apin;
            lblTitle.Content = "ALL PEOPLE";
            
        }

        private void ButtonMenuReaders_Click(object sender, RoutedEventArgs e)
        {
            ReadersView readers = new ReadersView();
            MenuPage.Content = readers;
            lblTitle.Content = "READERS";
            
        }

        private void ButtonMenuCameras_Click(object sender, RoutedEventArgs e)
        {
            CameraView cameraView = new CameraView();
            MenuPage.Content = cameraView;
            lblTitle.Content = "CAMERAS";
            
        }

        private void CheckStreamProcessorStatus()
        {
            //Mouse.OverrideCursor = Cursors.Wait;
            //try
            //{
            //    if (!Models.Starter.ListStreamProcessors().Contains(Models.MyAWSConfigs.StreamProcessorName))
            //    {
            //        Models.Starter.CreateStreamProcessor();
            //    }
            //}
            //finally
            //{
            //    Mouse.OverrideCursor = null;
            //}
        }

        // function for check a window is open & avoid opening it twice
        //private void ButtonMessage_Click(object sender, RoutedEventArgs e)
        //{
        //    bool isWindowOpen = false;

        //    foreach (Window w in Application.Current.Windows)
        //    {
        //        if (w is ChatView)
        //        {
        //            isWindowOpen = true;
        //            w.Activate();
        //        }
        //    }

        //    if (!isWindowOpen)
        //    {
        //        ChatView chatView = new ChatView();
        //        chatView.Show();
        //    }
        //}

        private void ButtonMenuAdmins_Click(object sender, RoutedEventArgs e)
        {
            AdminsPage admins = new AdminsPage();
            MenuPage.Content = admins;
            lblTitle.Content = "Admins";
        }

        private void ButtonMessage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            AdminProfile adminProfile = new AdminProfile(myid);
            MenuPage.Content = adminProfile;
            lblTitle.Content = "My Profile";

        }
    }
}
