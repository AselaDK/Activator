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

        public object AdminPropic { get; private set; }

        public AdminsPage()
        {
            InitializeComponent();

            try
            {
                this.client = new AmazonDynamoDBClient();
                this.LoadData(null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: failed to create a DynamoDB client; " + ex.Message);
            }
        }



        protected void LoadData(object obj)
        {
            var tableName = "admin";
            //load DynamoDB table
            var table = Table.LoadTable(client, tableName);
            //scan the table for get all details
            var search = table.Scan(new Amazon.DynamoDBv2.DocumentModel.Expression());

            // create DynamoDB document with scanned data 
            var documentList = new List<Document>();
            do
            {
                documentList.AddRange(search.GetNextSet());

            } while (!search.IsDone);

            // create a Collection
            //Camera is the name of the model in <Camera>, it is in Models Folder(Camera.cs)
            var admins = new ObservableCollection<Admin>();

            // getting DynamoDB Document data to Collection
            foreach (var doc in documentList)
            {
                var admin = new Admin();
                foreach (var attribute in doc.GetAttributeNames())
                {
                    var value = doc[attribute];
                    if (attribute == "aId")
                    {
                        admin.AId = value.AsPrimitive().Value.ToString();
                        Console.WriteLine(admin.AId);
                    }
                    else if (attribute == "aName")
                    {
                        admin.AName = value.AsPrimitive().Value.ToString();
                        Console.WriteLine(admin.AName);
                    }
                    else if (attribute == "aPhone")
                    {
                        admin.APhone = value.AsPrimitive().Value.ToString();
                        Console.WriteLine("phone", admin.APhone);
                    }
                    else if (attribute == "aPropic")
                    {
                        string imagename = value.AsPrimitive().Value.ToString();
                        Console.WriteLine("propic name >>>>>>>>",imagename);
                        //MessageBox.Show("propic name >>>>>>>>", imagename);
                        S3Bucket.DownloadFile(imagename);
                        //$"Resources/Images/{fileName}"
                        string propicUri = AppDomain.CurrentDomain.BaseDirectory + "Resources/Images/activatorlogo1.png";
                        propicUri = AppDomain.CurrentDomain.BaseDirectory + $"Resources/Images/{imagename}";
                        ImageSource imageSource = new BitmapImage(new Uri(@propicUri, UriKind.Relative));
                        //BitmapSource bmp = (BitmapSource)img;
                        ////...
                        //this.image2.Source = bmp;
                        //admin.AImage = (BitmapImage)imageSource;
                        AdminPropic = imageSource;
                    }
                }

                //Add camera data to collection
                admins.Add(admin);
                //give itemsource to datagrid in the frontend, DataGrid's name is CamerasDataGrid
                AdminDataGrid.ItemsSource = admins;

            }
        }

        private void RegAdmin_Click(object sender, RoutedEventArgs e)
        {
            RegisterAdmin acv = new RegisterAdmin();
            acv.ShowDialog();
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
    }
}
