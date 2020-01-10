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
using Amazon.DynamoDBv2.DocumentModel;
using static Amazon.Internal.RegionEndpointProviderV2;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Activator.Models;
using Amazon.DynamoDBv2.Model;


namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : UserControl
    {

       
        //private ObservableCollection<Customer> ;

        public Logs()
        {
            InitializeComponent();

        }

        private void LoadLogsData()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<Models.Logs> logs = new List<Models.Logs>();

                logs = Models.Dynamodb.GetAllLogs();

                dataGridLogs.ItemsSource = logs;
                dataGridLogs.Items.Refresh();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void dataGridLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
   
}
