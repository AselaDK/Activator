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
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteCamera.xaml
    /// </summary>
    public partial class ConfirmDeleteCamera : Window
    {
        private AmazonDynamoDBClient client;

        public ConfirmDeleteCamera()
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
        public string DeleteCamId { get; set; }

        private void BtnConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            var tableName = "Cameras";
            //load DynamoDB table
            var table = Table.LoadTable(client, tableName);
            var item = table.GetItem(DeleteCamId);

            try
            {

                if (item != null)
                {
                    table.DeleteItem(item);
                    MessageBox.Show("Successfully Deleted!");
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
    }
}
