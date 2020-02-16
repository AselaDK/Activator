using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;


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
