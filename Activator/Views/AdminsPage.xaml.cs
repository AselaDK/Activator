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
        private readonly string aId = null;

        public AdminsPage(string aid)
        {
            aId = aid;
            InitializeComponent();
            InitData();

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

        private void RegAdmin_Click(object sender, RoutedEventArgs e)
        {
            string tableName = MyAWSConfigs.adminDBTableName;
            var table = Table.LoadTable(client, tableName);
            var item = table.GetItem(aId);
            RegisterAdmin acv = new RegisterAdmin();
            if(item["root"].AsBoolean() == true)
            {
                acv.ShowDialog();
            }
            else
            {
                MessageBox.Show("Only the rood admin can register the new users");
            }
            
        }

        private void CamerasDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditCameraView ecv = new EditCameraView();
            ecv.DataContext = AdminDataGrid.SelectedItem;
            //ecv.TxtCamId.Text = row
            //ecv.TxtLocation.Text = Convert.ToString(ColLocation);
            //ecv.TxtQuality.Text = Convert.ToString(ColQuality);
            ecv.ShowDialog();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string tableName = MyAWSConfigs.adminDBTableName;
            var table = Table.LoadTable(client, tableName);
            var item = table.GetItem(aId);
            //LoadData(item);
        }
    }
}
