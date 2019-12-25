using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Windows;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for EditCameraView.xaml
    /// </summary>
    public partial class EditCameraView : Window
    {
        private readonly AmazonDynamoDBClient client;

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
            var tableName = "cameras";
            //load DynamoDB table
            var table = Table.LoadTable(client, tableName);
            var item = table.GetItem(cid);

            try
            {
                //Console.WriteLine(item["aPassword"]);

                if (item != null)
                {
                    Console.WriteLine("\n*** Executing UpdateMultipleAttributes() ***");
                    Console.WriteLine("\nUpdating multiple attributes....");
                    string partitionKey = cid;

                    Document camObj = new Document();
                    camObj["location"] = loc;
                    camObj["quality"] = qlty;

                    // Optional parameters.
                    UpdateItemOperationConfig config = new UpdateItemOperationConfig
                    {
                        // Get updated item in response.
                        ReturnValues = ReturnValues.AllNewAttributes
                    };
                    Document updatedadmin = table.UpdateItem(camObj, config);
                    Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
                    MessageBox.Show("Successfully Updated!");

                    this.Close();

                }
                else
                {
                    MessageBox.Show("There is no such a Camera!");
                }

                MainView mainv = new MainView();
                CamerasPageView cams = new CamerasPageView();
                mainv.MenuPage.Content = cams;
                mainv.Show();

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

        private void BtnDeleteCamera_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDeleteCamera cdc = new ConfirmDeleteCamera();
            cdc.DeleteCamId = TxtCamId.Text;
            cdc.ShowDialog();
        }
    }
}
