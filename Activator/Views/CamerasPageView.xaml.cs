using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Amazon.DynamoDBv2;
using Activator.Models;
using System.Collections.ObjectModel;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Amazon.DynamoDBv2.DocumentModel;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for CameraPageView.xaml
    /// </summary>
    public partial class CamerasPageView : UserControl
    {
      
        private AmazonDynamoDBClient client;

        public CamerasPageView()
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
            //load DynamoDB table
            var table = Table.LoadTable(client, "Cameras");
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
            var cameras = new ObservableCollection<Camera>();

            // getting DynamoDB Document data to Collection
            foreach (var doc in documentList)
            {
                var camera = new Camera();
                foreach (var attribute in doc.GetAttributeNames())
                {
                    var value = doc[attribute];
                    if (attribute == "camId")
                    {
                        camera.camId = value.AsPrimitive().Value.ToString();
                        //Console.WriteLine(camera.camId);
                    }
                    else if (attribute == "location")
                    {
                        camera.location = value.AsPrimitive().Value.ToString();
                        //Console.WriteLine(camera.location);
                    }
                    else if (attribute == "quality")
                    {
                        camera.quality = value.AsPrimitive().Value.ToString();
                        //Console.WriteLine("quality",camera.quality);
                    }
                }

                //Add camera data to collection
                cameras.Add(camera);
                //give itemsource to datagrid in the frontend, DataGrid's name is CamerasDataGrid
                CamerasDataGrid.ItemsSource = cameras;

            }
        }

        private void AddNewCamera_Click(object sender, RoutedEventArgs e)
        {
            AddCameraView acv = new AddCameraView();
            acv.ShowDialog();
        }

        private void CamerasDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditCameraView ecv = new EditCameraView();
            ecv.DataContext = CamerasDataGrid.SelectedItem;
            //ecv.TxtCamId.Text = row
            //ecv.TxtLocation.Text = Convert.ToString(ColLocation);
            //ecv.TxtQuality.Text = Convert.ToString(ColQuality);
            ecv.ShowDialog();
        }
    }
}
