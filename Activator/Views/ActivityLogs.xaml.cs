using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
