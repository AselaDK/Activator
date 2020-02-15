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
    /// Interaction logic for Activitylogs.xaml
    /// </summary>
    public partial class ActivityLogs : UserControl
    {
        public ActivityLogs()
        {
            InitializeComponent();
            LoadActivityLogsData();
        }
    
        private void LoadActivityLogsData()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<Models.ActivityLogs> logs = new List<Models.ActivityLogs>();

                logs = Models.ActivityLogs.GetAllactvityLogs();

                dataGridActivityLogs.ItemsSource = logs;
                dataGridActivityLogs.Items.Refresh();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadActivityLogsData();
        }
        private void dataGridActivityLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
