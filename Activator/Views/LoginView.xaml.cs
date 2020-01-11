using Activator.Models;
using Amazon.DynamoDBv2;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : Window
    {        
        public LoginView()
        {
            InitializeComponent();            
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            String aId = TxtUid.Text;
            String aPassword = TxtPassword.Password;
            String hashPassword = HashMD5.MD5Hash(aPassword);

            try
            {
                var item = Dynamodb.GetItem(aId, MyAWSConfigs.AdminDBTableName);

                if (item != null && item["aPassword"] == hashPassword)
                {
                    Mouse.OverrideCursor = null;
                    this.Hide();

                    string adminName = item["aName"];
                    string adminId = item["aId"];
                    bool status = true;

                    Session session = new Session(status, adminId);

                    MainView dashboard = new MainView(adminId, adminName);
                    dashboard.ShowDialog();
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
                Mouse.OverrideCursor = null;
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