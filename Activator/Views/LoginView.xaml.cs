using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Activator.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System.Windows.Input;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : MetroWindow
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
            String hashPassword = Models.HashMD5.MD5Hash(aPassword);

            try
            {
                if (string.IsNullOrEmpty(aId) || string.IsNullOrEmpty(aPassword))
                {
                    TxtUid.Text = "";
                    TxtUid.BorderBrush = Brushes.Red;

                    TxtPassword.Password = "";
                    TxtPassword.BorderBrush = Brushes.Red;
                    MessageBox.Show("Please Fill All Fields!");
                }
                else
                {
                    var item = await Task.Run(() => Models.Dynamodb.GetItem(aId, Models.MyAWSConfigs.AdminDBTableName));

                    if (item != null && item["aPassword"] == hashPassword)
                    {
                        if(item["blocked"].AsBoolean() == false)
                        {
                            notifyIcon.Visible = true;
                            notifyIcon.ShowBalloonTip(2000, "Welcome", $"{item["aName"]}", System.Windows.Forms.ToolTipIcon.Info);

                            this.Hide();

                            string adminName = item["aName"];
                            string adminId = item["aId"];
                            string adminPropic = item["aPropic"];

                            //saving session
                            Models.Session.id = adminId;

                            //activity log
                            Models.ActivityLogs.Activity(Models.Components.AdminComponent, "User login");

                            TxtUid.Clear();
                            TxtPassword.Clear();

                            MainView mainView = new MainView(adminId, adminName, adminPropic, this);
                            mainView.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Your Profile is BLOCKED!");
                        }
                        

                    }
                    else
                    {
                        //activity recorded
                        Models.ActivityLogs.Activity(Models.Components.AdminComponent, "User login attempt failed");

                        TxtUid.Text = "";
                        TxtUid.BorderBrush = Brushes.Red;

                        TxtPassword.Password = "";
                        TxtPassword.BorderBrush = Brushes.Red;
                        MessageBox.Show("User Name or Password is INCORRECT!");
                    }
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                MessageBox.Show("Message : Server Error", ex.Message);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Message : Unknown Error", ex.Message);
                MessageBox.Show("Message : User Name or Password is INCORRECT!");
                TxtUid.Text = "";
                TxtUid.BorderBrush = Brushes.Red;

                TxtPassword.Password = "";
                TxtPassword.BorderBrush = Brushes.Red;
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
                ButtonLogin.IsEnabled = true;
            }
        }

        private async void btnForget_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("Please Contact the Developer Team. Thank You!", "", MessageDialogStyle.Affirmative);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}