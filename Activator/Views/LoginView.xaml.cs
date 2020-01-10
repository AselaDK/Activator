using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Amazon.DynamoDBv2;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using System.Security.Cryptography;

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
            //Models.StreamProcessorManager.DeleteStreamProcessor("StreamProcessorCam1");
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainView dashboard = new MainView();
            dashboard.ShowDialog();

            //Mouse.OverrideCursor = Cursors.Wait;
            //try
            //{
            //    String aId = TxtUid.Text;
            //    String aPassword = TxtPassword.Password;
            //    String hashPassword = Models.HashMD5.MD5Hash(aPassword);

            //    try
            //    {

            //        string tableName = "admin";

            //        var client = new AmazonDynamoDBClient();
            //        var table = Table.LoadTable(client, tableName);
            //        var item = table.GetItem(aId);

            //        //Console.WriteLine(item["aPassword"]);

            //        if (item != null && item["aPassword"] == hashPassword)
            //        {
            //            //Console.WriteLine("Successfully Logged in!!!");
            //            Mouse.OverrideCursor = null;
            //            this.Hide();
            //            MainView dashboard = new MainView();
            //            dashboard.ShowDialog();
            //        }
            //        else
            //        {
            //            //MessageBox.Show("Username or Password is incorrect!");

            //            //clear texboxes
            //            TxtUid.Text = "";
            //            TxtUid.BorderBrush = Brushes.Red;
            //            //txtuid.Background = Brushes.LightSalmon;

            //            TxtPassword.Password = "";
            //            TxtPassword.BorderBrush = Brushes.Red;
            //            //txtpassword.Background = Brushes.LightSalmon;
            //        }
            //    }
            //    catch (AmazonDynamoDBException ex)
            //    {
            //        MessageBox.Show("Message : Server Error", ex.Message);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Message : Unknown Error", ex.Message);
            //    }
            //}
            //finally
            //{
            //    Mouse.OverrideCursor = null;
            //}           
        }

        private void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnForget_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please Contact the Developer Team. Thank You!");
        }

        private void ButtonLogin_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}