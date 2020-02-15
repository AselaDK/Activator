using Activator.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for CameraPageView.xaml
    /// </summary>
    public partial class AdminsPage : UserControl
    {

        private AmazonDynamoDBClient client;
        private string aId;

        public AdminsPage(string aid)
        {
            aId = aid;
            Console.WriteLine(">>>>>>>>>><<<<<<<<<<<<<,,,,,,,,,Constructor " + aid);
            InitializeComponent();
            InitData();
            aId = getId(aid);

            try
            {
                this.client = new AmazonDynamoDBClient();
                
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: failed to create a DynamoDB client; " + ex.Message);
            }
        }

        private void InitData()
        {
            LoadData();
        }

        protected void LoadData()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<Admin> admins = new List<Admin>();

                admins = Admin.GetAdminDetails();

                Console.WriteLine();

                lblLoading.Visibility = Visibility.Hidden;

                AdminDataGrid.ItemsSource = admins;
                AdminDataGrid.Items.Refresh();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        
        }

        private String getId(String id)
        {
            aId = id;
            return id;
        }

        private void RegAdmin_Click(object sender, RoutedEventArgs e)
        {
            string tableName = MyAWSConfigs.AdminDBTableName;
            var table = Table.LoadTable(client, tableName);
            Console.WriteLine(">>>>>>>>>><<<<<<<<<<<<<,,,,,,,,, " + aId);

            var item = table.GetItem(aId);
            Console.WriteLine(">>>>>>>>>><<<<<<<<<<<<<,,,,,,,,, " + aId);
            RegisterAdmin acv = new RegisterAdmin();
            if(item["root"].AsBoolean() == true)
            {
                RegAdmin.IsEnabled = true;  
                acv.ShowDialog();
            }
            else
            {
                MessageBox.Show("Only the rood admin can register the new users");
            }
            
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string tableName = MyAWSConfigs.AdminDBTableName;
            var table = Table.LoadTable(client, tableName);
            var item = table.GetItem(aId);
            //LoadData(item);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
