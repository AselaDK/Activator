using Activator.Views.Reader;
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
    /// Interaction logic for ReadersView.xaml
    /// </summary>
    public partial class ReadersPage : UserControl
    {
        public ReadersPage()
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
            InitData();
        }

        private void RegRaeder_Click(object sender, RoutedEventArgs e)
        {
            ReaderForm readerForm = new ReaderForm();
            readerForm.Show();
        }

        private AmazonDynamoDBClient client;
        private readonly string aId = null;

        private void InitData()
        {
            LoadData();
        }

        protected void LoadData()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<Models.Reader> readers = new List<Models.Reader>();

                readers = Models.Reader.GetReadersData();

                Console.WriteLine();

                lblLoading.Visibility = Visibility.Hidden;

                AdminDataGrid.ItemsSource = readers;
                AdminDataGrid.Items.Refresh();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void CamerasDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //EditCameraView ecv = new EditCameraView();
            //ecv.DataContext = AdminDataGrid.SelectedItem;
            ////ecv.TxtCamId.Text = row
            ////ecv.TxtLocation.Text = Convert.ToString(ColLocation);
            ////ecv.TxtQuality.Text = Convert.ToString(ColQuality);
            //ecv.ShowDialog();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string tableName = Models.MyAWSConfigs.AdminDBTableName;
            var table = Table.LoadTable(client, tableName);
            var item = table.GetItem(aId);
            //LoadData(item);
        }

        private void AdminDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("thi is from double click");
            EditReader editReader = new EditReader();
            editReader.Show();
        }
    }
}

