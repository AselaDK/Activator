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
using Activator.Views;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using static Amazon.Internal.RegionEndpointProviderV2;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Activator.Models;
using System.Collections.ObjectModel;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for EditCameraView.xaml
    /// </summary>
    public partial class EditCameraView : Window
    {
        private AmazonDynamoDBClient client;

        public EditCameraView()
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
        
        private void UpdateCamera(string cid, string loc, string qlty)
        {
            var tableName = "Cameras";
            //load DynamoDB table
            var table = Table.LoadTable(client, tableName);
            var item = table.GetItem(cid);

            try
            {
                //Console.WriteLine(item["aPassword"]);

                if (item != null)
                {
                    Document camObj = new Document();
                    camObj["camId"] = cid;
                    camObj["location"] = loc;
                    camObj["quality"] = qlty;
                    table.PutItem(camObj);
                    MessageBox.Show("Successfully Updated!");

                    MainView mainv = new MainView();
                    CamerasPageView cams = new CamerasPageView();
                    mainv.MenuPage.Content = cams;
                    mainv.Show();
                }
                else
                {
                    MessageBox.Show("There is no such a Camera!");
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

        private void ButtonUpdateCamera_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("QWFQf");
            string camid = TxtCamId.Text;
            string location = TxtLocation.Text;
            string quality = TxtQuality.Text;
            UpdateCamera(camid, location, quality);
        }

        private void TxtCamId_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
