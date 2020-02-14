using Activator.Models;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        public LoginView()
        {
            InitializeComponent();
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

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            ButtonLogin.IsEnabled = false;

            String aId = TxtUid.Text;
            String aPassword = TxtPassword.Password;
            String hashPassword = HashMD5.MD5Hash(aPassword);
            
            try
            {
                var item = await Task.Run(() => Dynamodb.GetItem(aId, MyAWSConfigs.AdminDBTableName));

                if (item != null && item["aPassword"] == hashPassword)
                {
                    notifyIcon.Visible = true;
                    notifyIcon.ShowBalloonTip(2000, "Welcome", $"{item["aName"]}", System.Windows.Forms.ToolTipIcon.Info);
                                        
                    this.Hide();

                    string adminName = item["aName"];
                    string adminId = item["aId"];
                    string adminPropic = item["aPropic"];
                    bool status = true;

                    Session session = new Session(status, adminId);

                    MainView mainView = new MainView(adminId, adminName, adminPropic);
                    mainView.ShowDialog();
                }
                else
                {
                    //MessageBox.Show("Username or Password is incorrect!");

                    //clear texboxes
                    TxtUid.Text = "";
                    TxtUid.BorderBrush = Brushes.Red;
                    //txtuid.Background = Brushes.LightSalmon;

                    TxtPassword.Password = "";
                    TxtPassword.BorderBrush = Brushes.Red;
                    //txtpassword.Background = Brushes.LightSalmon;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message : Unknown Error", ex.Message);
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
                ButtonLogin.IsEnabled = true;
            }
        }

        private void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnForget_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please Contact the Developer Team. Thank You!");
        }

    }
}