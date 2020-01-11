using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        HomePageView home;
        PeopleInPageView peopleInPageView;
        AllPeoplePageView allPeoplePageView;
        ReadersView readers;
        CameraView cameraView;
        AdminsPage admins;
        AdminProfile adminProfile;

        private string myname;
        private string myid;

        public MainView(String adminid, String adminname)
        {
            InitializeComponent();

            InitUserControls();

            myname = adminname;
            myid = adminid;

            AdminName.Text = myname;

            string imagename = "tamanna.jpg";
            string directoryPath = "Resources/Images/";

            if (!File.Exists(directoryPath + imagename))
            {
                Models.S3Bucket.DownloadFile(imagename, Models.MyAWSConfigs.AdminS3BucketName);
            }

            string exeDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

            Uri fileUri = new Uri(exeDirectory + directoryPath + imagename);

            ImageSource imageSource = new BitmapImage(fileUri);

            MyPropicImage.Source = imageSource;

            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;

            MenuPage.Content = home;
            lblTitle.Content = "HOME";
        }

        private void InitUserControls()
        {
            home = new HomePageView();
            peopleInPageView = new PeopleInPageView();
            allPeoplePageView = new AllPeoplePageView();
            readers = new ReadersView();
            cameraView = new CameraView();
            admins = new AdminsPage(myid);
            adminProfile = new AdminProfile();
        }

        private void ButtonMenuHome_Click(object sender, RoutedEventArgs e)
        {
            MenuPage.Content = home;
            lblTitle.Content = "HOME";
        }

        private void ButtonMenuPeopleIn_Click(object sender, RoutedEventArgs e)
        {
            MenuPage.Content = peopleInPageView;
            lblTitle.Content = "HISTORY";
        }

        private void ButtonMenuAllPeople_Click(object sender, RoutedEventArgs e)
        {
            MenuPage.Content = allPeoplePageView;
            lblTitle.Content = "ALL PEOPLE";
        }

        private void ButtonMenuReaders_Click(object sender, RoutedEventArgs e)
        {
            MenuPage.Content = readers;
            lblTitle.Content = "READERS";
        }

        private void ButtonMenuCameras_Click(object sender, RoutedEventArgs e)
        {
            MenuPage.Content = cameraView;
            lblTitle.Content = "CAMERAS";
        }

        private void ButtonMenuAdmins_Click(object sender, RoutedEventArgs e)
        {
            MenuPage.Content = admins;
            lblTitle.Content = "ADMINS";
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            adminProfile = null;
            adminProfile = new AdminProfile(myid);
            MenuPage.Content = adminProfile;
            lblTitle.Content = "MY PROFILE";
        }

        private async void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
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

        private void ButtonMessage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}