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
using System.Data.SqlClient;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using static Amazon.Internal.RegionEndpointProviderV2;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            String aId = txtuid.Text;
            String aPassword = txtpassword.Password;

            Console.WriteLine(aId);
            Console.WriteLine(aPassword);

            try
            {
                
                string tableName = "admin";

                var client = new AmazonDynamoDBClient();
                var table = Table.LoadTable(client, tableName);
                var item = table.GetItem(aId);

                Console.WriteLine(item["aPassword"]);

                bool count = false;      
                
                if(item["aPassword"] == aPassword)
                {
                    count = true;
                }

                if (count == true)
                {
                    Console.WriteLine("Hello World!");
                    MainWindow dashboard = new MainWindow();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username or Password is incorrect!");
                }

    
            }
            catch (AmazonDynamoDBException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnForget_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
