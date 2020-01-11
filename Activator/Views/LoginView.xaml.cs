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
        private readonly AmazonDynamoDBClient client;
        public LoginView()
        {
            InitializeComponent();
            try
            {
                this.client = new AmazonDynamoDBClient();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: failed to create a DynamoDB client; " + ex.Message);
            }
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                String aId = TxtUid.Text;
                String aPassword = TxtPassword.Password;
                String hashPassword = HashMD5.MD5Hash(aPassword);

                ////Console.WriteLine(aId);
                ////Console.WriteLine(aPassword);

                try
                {

                    string tableName = MyAWSConfigs.AdminDBTableName;
                    var table = Table.LoadTable(client, tableName);
                    var item = table.GetItem(aId);

                    //Console.WriteLine(item["aPassword"]);
                    if (item != null && item["aPassword"] == hashPassword)
                    {

                        //Console.WriteLine("Successfully Logged in!!!");
                        var AdminName = item["aName"];
                        string AdminId = item["aId"];
                        bool status = true;
                        Console.WriteLine(AdminId);

                        Session session = new Session(status, AdminId);
                        MainView dashboard = new MainView(AdminId, AdminName);
                        this.Hide();
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
                catch (AmazonDynamoDBException ex)
                {
                    MessageBox.Show("Message : Server Error", ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message : Unknown Error", ex.Message);
                }
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