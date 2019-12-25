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
using System.Windows.Shapes;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using static Amazon.Internal.RegionEndpointProviderV2;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Activator.Models;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for AddCameraView.xaml
    /// </summary>
    public partial class AddCameraView : Window
    {
        private AmazonDynamoDBClient client;

        public AddCameraView()
        {
            InitializeComponent();

            try
            {
                //this.Cameras = new ObservableCollection<Camera>();
                this.client = new AmazonDynamoDBClient();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: failed to create a DynamoDB client; " + ex.Message);
            }

        }

        private void ButtonAddCamera_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("QWFQf");
            string camid = TxtCamId.Text;
            string location = TxtLocation.Text;
            string quality = TxtQuality.Text;
            AddCamera(camid, location, quality);
        }

        private void AddCamera(string camid, string loc, string qlty)
        {
            try
            {
                string tableName = "Cameras";

                var table = Table.LoadTable(client, tableName);
                var search = table.Scan(new Amazon.DynamoDBv2.DocumentModel.Expression());
                
                if (search.Count != 0)
                {
                    var item = table.GetItem(camid);

                    if (item == null)
                    {
                        //Console.WriteLine("search  = ", search);
                        //Console.WriteLine("search.count  = ", search.Count);
                        Document camObj = new Document();
                        camObj["camId"] = camid;
                        camObj["location"] = loc;
                        camObj["quality"] = qlty;
                        table.PutItem(camObj);

                        this.Close();
                        MessageBox.Show("New Camera Was Successfully Added!");
                    }
                    else
                    {
                        MessageBox.Show("Camera ID is already exist! (Please try a different one)");
                    }
                    MainView mainv = new MainView();
                    CamerasPageView cams = new CamerasPageView();
                    mainv.MenuPage.Content = cams;
                    mainv.Show();
                }
                else
                {
                    MessageBox.Show("Table scan doesn't gives results");
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
    }
}
