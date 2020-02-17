using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;

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
        ReadersPage readers;
        CameraView cameraView;
        AdminsPage admins;
        AdminProfile adminProfile;
        LoginView loginView;

        private string adminId;
        private string adminName;
        private string adminPropic;

        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        public MainView(String adminId, String adminName, string adminPropic, LoginView lv)
        {
            InitializeComponent();

            this.loginView = lv;

            this.adminId = adminId;
            this.adminName = adminName;
            this.adminPropic = adminPropic;

            InitUserControls();
            InitUserInfo().ConfigureAwait(false);

            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;

            MenuPage.Content = home;
            lblTitle.Content = "HOME";

            ButtonMenuHome.Background = Brushes.MediumSeaGreen;
            homeIcon.Foreground = Brushes.White;
            homeLable.Foreground = Brushes.White;

            ButtonMenuPeopleIn.Background = null;
            historyIcon.Foreground = Brushes.MediumSeaGreen;
            historyLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAllPeople.Background = null;
            refIcon.Foreground = Brushes.MediumSeaGreen;
            refLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuReaders.Background = null;
            readerIcon.Foreground = Brushes.MediumSeaGreen;
            readerLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuCameras.Background = null;
            cameraIcon.Foreground = Brushes.MediumSeaGreen;
            cameraLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAdmins.Background = null;
            adminIcon.Foreground = Brushes.MediumSeaGreen;
            adminLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuActivityLogs.Background = null;
            logsIcon.Foreground = Brushes.MediumSeaGreen;
            logsLable.Foreground = Brushes.MediumSeaGreen;

            home.GetAllCameras().ConfigureAwait(false);
        }

        protected override void OnInitialized(EventArgs e)
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            notifyIcon.Click += notifyIcon_Click;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.BalloonTipClosed += (s, _e) => notifyIcon.Visible = false;

            base.OnInitialized(e);
        }

        void notifyIcon_Click(object sender, EventArgs e)
        {

        }

        void notifyIcon_DoubleClick(object sender, EventArgs e)
        {

        }

        private void InitUserControls()
        {
            home = new HomePageView(this);
            peopleInPageView = new PeopleInPageView();
            allPeoplePageView = new AllPeoplePageView(this);
            readers = new ReadersPage();
            cameraView = new CameraView(this);
            admins = new AdminsPage(adminId, this);
            adminProfile = new AdminProfile(adminId);
        }

        private async Task InitUserInfo()
        {
            AdminName.Text = adminName;

            string directoryPath = "Resources/Images/";

            if (!File.Exists(directoryPath + adminPropic))
            {
                await Task.Run(() => Models.S3Bucket.DownloadFile(adminPropic, Models.MyAWSConfigs.AdminS3BucketName));
            }

            string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

            Uri fileUri = new Uri(exeDirectory + directoryPath + adminPropic);

            ImageSource imageSource = new BitmapImage(fileUri);

            MyPropicImage.ImageSource = imageSource;
        }

        private void ButtonMenuHome_Click(object sender, RoutedEventArgs e)
        {            
            MenuPage.Content = home;
            lblTitle.Content = "HOME";

            ButtonMenuHome.Background = Brushes.MediumSeaGreen;
            homeIcon.Foreground = Brushes.White;
            homeLable.Foreground = Brushes.White;

            ButtonMenuPeopleIn.Background = null;
            historyIcon.Foreground = Brushes.MediumSeaGreen;
            historyLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAllPeople.Background = null;
            refIcon.Foreground = Brushes.MediumSeaGreen;
            refLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuReaders.Background = null;
            readerIcon.Foreground = Brushes.MediumSeaGreen;
            readerLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuCameras.Background = null;
            cameraIcon.Foreground = Brushes.MediumSeaGreen;
            cameraLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAdmins.Background = null;
            adminIcon.Foreground = Brushes.MediumSeaGreen;
            adminLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuActivityLogs.Background = null;
            logsIcon.Foreground = Brushes.MediumSeaGreen;
            logsLable.Foreground = Brushes.MediumSeaGreen;

            home.GetAllCameras().ConfigureAwait(false);
        }

        private void ButtonMenuPeopleIn_Click(object sender, RoutedEventArgs e)
        {
            MenuPage.Content = peopleInPageView;
            lblTitle.Content = "HISTORY";

            ButtonMenuPeopleIn.Background = Brushes.MediumSeaGreen;
            historyIcon.Foreground = Brushes.White;
            historyLable.Foreground = Brushes.White;

            ButtonMenuAllPeople.Background = null;
            refIcon.Foreground = Brushes.MediumSeaGreen;
            refLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuHome.Background = null;
            homeIcon.Foreground = Brushes.MediumSeaGreen;
            homeLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuReaders.Background = null;
            readerIcon.Foreground = Brushes.MediumSeaGreen;
            readerLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuCameras.Background = null;
            cameraIcon.Foreground = Brushes.MediumSeaGreen;
            cameraLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAdmins.Background = null;
            adminIcon.Foreground = Brushes.MediumSeaGreen;
            adminLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuActivityLogs.Background = null;
            logsIcon.Foreground = Brushes.MediumSeaGreen;
            logsLable.Foreground = Brushes.MediumSeaGreen;
        }

        private void ButtonMenuAllPeople_Click(object sender, RoutedEventArgs e)
        {            
            MenuPage.Content = allPeoplePageView;
            lblTitle.Content = "REFERENCES";

            ButtonMenuAllPeople.Background = Brushes.MediumSeaGreen;
            refIcon.Foreground = Brushes.White;
            refLable.Foreground = Brushes.White;

            ButtonMenuPeopleIn.Background = null;
            historyIcon.Foreground = Brushes.MediumSeaGreen;
            historyLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuHome.Background = null;
            homeIcon.Foreground = Brushes.MediumSeaGreen;
            homeLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuReaders.Background = null;
            readerIcon.Foreground = Brushes.MediumSeaGreen;
            readerLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuCameras.Background = null;
            cameraIcon.Foreground = Brushes.MediumSeaGreen;
            cameraLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAdmins.Background = null;
            adminIcon.Foreground = Brushes.MediumSeaGreen;
            adminLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuActivityLogs.Background = null;
            logsIcon.Foreground = Brushes.MediumSeaGreen;
            logsLable.Foreground = Brushes.MediumSeaGreen;

            allPeoplePageView.LoadPersonsData().ConfigureAwait(false);
        }

        private void ButtonMenuReaders_Click(object sender, RoutedEventArgs e)
        {
            MenuPage.Content = readers;
            lblTitle.Content = "READERS";

            ButtonMenuReaders.Background = Brushes.MediumSeaGreen;
            readerIcon.Foreground = Brushes.White;
            readerLable.Foreground = Brushes.White;

            ButtonMenuPeopleIn.Background = null;
            historyIcon.Foreground = Brushes.MediumSeaGreen;
            historyLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuHome.Background = null;
            homeIcon.Foreground = Brushes.MediumSeaGreen;
            homeLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAllPeople.Background = null;
            refIcon.Foreground = Brushes.MediumSeaGreen;
            refLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuCameras.Background = null;
            cameraIcon.Foreground = Brushes.MediumSeaGreen;
            cameraLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAdmins.Background = null;
            adminIcon.Foreground = Brushes.MediumSeaGreen;
            adminLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuActivityLogs.Background = null;
            logsIcon.Foreground = Brushes.MediumSeaGreen;
            logsLable.Foreground = Brushes.MediumSeaGreen;

            readers.LoadData().ConfigureAwait(false);
        }

        private void ButtonMenuCameras_Click(object sender, RoutedEventArgs e)
        {            
            MenuPage.Content = cameraView;
            lblTitle.Content = "CAMERAS";

            ButtonMenuCameras.Background = Brushes.MediumSeaGreen;
            cameraIcon.Foreground = Brushes.White;
            cameraLable.Foreground = Brushes.White;

            ButtonMenuPeopleIn.Background = null;
            historyIcon.Foreground = Brushes.MediumSeaGreen;
            historyLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuHome.Background = null;
            homeIcon.Foreground = Brushes.MediumSeaGreen;
            homeLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAllPeople.Background = null;
            refIcon.Foreground = Brushes.MediumSeaGreen;
            refLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuReaders.Background = null;
            readerIcon.Foreground = Brushes.MediumSeaGreen;
            readerLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAdmins.Background = null;
            adminIcon.Foreground = Brushes.MediumSeaGreen;
            adminLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuActivityLogs.Background = null;
            logsIcon.Foreground = Brushes.MediumSeaGreen;
            logsLable.Foreground = Brushes.MediumSeaGreen;

            cameraView.LoadCamerasData().ConfigureAwait(false);
        }

        private void ButtonMenuAdmins_Click(object sender, RoutedEventArgs e)
        {
            Item item = Models.Dynamodb.GetItem(adminId, Models.MyAWSConfigs.AdminDBTableName);
            if (item["root"].AsBoolean() == true)
            {
                MenuPage.Content = admins;
                lblTitle.Content = "ADMINS";
                admins.LoadData().ConfigureAwait(false);
            }
            else
            {

                MessageBox.Show("Only the Root-Admin can Access the this section");
            }

            ButtonMenuAdmins.Background = Brushes.MediumSeaGreen;
            adminIcon.Foreground = Brushes.White;
            adminLable.Foreground = Brushes.White;

            ButtonMenuPeopleIn.Background = null;
            historyIcon.Foreground = Brushes.MediumSeaGreen;
            historyLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuHome.Background = null;
            homeIcon.Foreground = Brushes.MediumSeaGreen;
            homeLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAllPeople.Background = null;
            refIcon.Foreground = Brushes.MediumSeaGreen;
            refLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuReaders.Background = null;
            readerIcon.Foreground = Brushes.MediumSeaGreen;
            readerLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuCameras.Background = null;
            cameraIcon.Foreground = Brushes.MediumSeaGreen;
            cameraLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuActivityLogs.Background = null;
            logsIcon.Foreground = Brushes.MediumSeaGreen;
            logsLable.Foreground = Brushes.MediumSeaGreen;

            admins.LoadData().ConfigureAwait(false);
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            //adminProfile = null;
            //adminProfile = new AdminProfile(adminId);
            MenuPage.Content = adminProfile;
            lblTitle.Content = "MY PROFILE";
            adminProfile.ShowProfileData(adminId).ConfigureAwait(false);
        }
        
        private void ButtonMenuActivityLogs_Click(object sender, RoutedEventArgs e)
        {
            ActivityLogs activityLogs = new ActivityLogs();
            MenuPage.Content = activityLogs;
            lblTitle.Content = "ACTIVITY LOGS";

            ButtonMenuActivityLogs.Background = Brushes.MediumSeaGreen;
            logsIcon.Foreground = Brushes.White;
            logsLable.Foreground = Brushes.White;

            ButtonMenuPeopleIn.Background = null;
            historyIcon.Foreground = Brushes.MediumSeaGreen;
            historyLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuHome.Background = null;
            homeIcon.Foreground = Brushes.MediumSeaGreen;
            homeLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAllPeople.Background = null;
            refIcon.Foreground = Brushes.MediumSeaGreen;
            refLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuReaders.Background = null;
            readerIcon.Foreground = Brushes.MediumSeaGreen;
            readerLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuCameras.Background = null;
            cameraIcon.Foreground = Brushes.MediumSeaGreen;
            cameraLable.Foreground = Brushes.MediumSeaGreen;

            ButtonMenuAdmins.Background = null;
            adminIcon.Foreground = Brushes.MediumSeaGreen;
            adminLable.Foreground = Brushes.MediumSeaGreen;

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

        private async void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowMessageAsync("Are you sure want to log out ?", "", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                this.Close();
                loginView.Close();
                LoginView tempLoginView = new LoginView();
                tempLoginView.Show();
            }
                
        }

        public async void DeleteCamera(string id, string videoStreamArn, string dataStreamName, string eventSourceUUID, string streamProcessorName, CameraView cv)
        {
            ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "");
            controller.SetIndeterminate();
            controller.SetCancelable(false);

            controller.SetMessage("Deleting event source mapping");
            await Task.Run(() => Models.Lambda.DeleteEventSourceMapping(eventSourceUUID));

            controller.SetMessage("Deleting data stream");
            await Task.Run(() => Models.DataStream.DeleteDataStream(dataStreamName));

            controller.SetMessage("Deleting video stream");
            await Task.Run(() => Models.VideoStream.DeleteVideoStream(videoStreamArn));

            controller.SetMessage("Deleting stream processor");
            await Task.Run(() => Models.StreamProcessorManager.DeleteStreamProcessor(streamProcessorName));

            controller.SetMessage("Deleting database record");
            await Task.Run(() => Models.Dynamodb.DeleteItem(id, Models.MyAWSConfigs.CamerasDBTableName));

            await controller.CloseAsync();

            cv.LoadCamerasData().ConfigureAwait(false);

            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(1000, "Deleted", "Camera deleted Successfully", System.Windows.Forms.ToolTipIcon.Info);

        }

        public async void DeleteRefPerson(string id, AllPeoplePageView allPeoplePageView)
        {
            ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "");
            controller.SetIndeterminate();
            controller.SetCancelable(false);

            controller.SetMessage("Deleting database record");
            await Task.Run(() => Models.Dynamodb.DeleteItem(id, Models.MyAWSConfigs.RefPersonsDBTableName));

            controller.SetMessage("Deleting image");
            await Task.Run(() => Models.S3Bucket.DeleteFile(id, Models.MyAWSConfigs.RefImagesBucketName));

            await controller.CloseAsync();

            allPeoplePageView.LoadPersonsData().ConfigureAwait(false);

            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(1000, "Deleted", "Person deleted Successfully", System.Windows.Forms.ToolTipIcon.Info);

        }

        private void ButtonMessage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}