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
        //private ObservableCollection<Camera> cameras;
        //private IObservableCollection<Camera> cameras = new IObservableCollection<Camera>();

        public CamerasPageView()
        {
            InitializeComponent();

            
            try
            {
                this.client = new AmazonDynamoDBClient();
                //this.Cameras = new ObservableCollection<Camera>();
                this.LoadData(null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: failed to create a DynamoDB client; " + ex.Message);
            }
        }

        private void LoadData(object obj)
        {
            //load DynamoDB table
            var table = Table.LoadTable(client, "Cameras");
            //scan the table for get all details
            var search = table.Scan(new Amazon.DynamoDBv2.DocumentModel.Expression());


            var documentList = new List<Document>();
            do
            {
                documentList.AddRange(search.GetNextSet());

            } while (!search.IsDone);

            //Camera is the name of the model in <Camera>, it is in Models Folder(Camera.cs)
            var cameras = new ObservableCollection<Camera>();

            foreach (var doc in documentList)
            {
                var camera = new Camera();
                foreach (var attribute in doc.GetAttributeNames())
                {
                    var value = doc[attribute];
                    if (attribute == "camId")
                    {
                        camera.camId = value.AsPrimitive().Value.ToString();
                        Console.WriteLine(camera.camId);
                    }
                    else if (attribute == "location")
                    {
                        camera.location = value.AsPrimitive().Value.ToString();
                        Console.WriteLine(camera.location);
                    }
                    else if (attribute == "quality")
                    {
                        camera.quality = value.AsPrimitive().Value.ToString();
                        Console.WriteLine("quality",camera.quality);
                    }
                }
                //give itemsource to datagrid, DataGrid's name is CamerasDataGrid
                CamerasDataGrid.ItemsSource = cameras;

                cameras.Add(camera);
            }

            //this.cameras = cameras;
        }
    }
}
