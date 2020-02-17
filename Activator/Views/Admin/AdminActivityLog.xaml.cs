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

namespace Activator.Views.Admin
{
    /// <summary>
    /// Interaction logic for AdminActivityLog.xaml
    /// </summary>
    public partial class AdminActivityLog : UserControl
    {
        private AdminsPage ap;
        private MainView mv;
        private string id;
        public AdminActivityLog(AdminsPage ap, MainView mv, string id)
        {
            InitializeComponent();
            this.ap = ap;
            this.mv = mv;
            this.id = id;
        }
        public void LoadActivityLogs(string id)
        {
            var aList = Models.Dynamodb.GetActivitiesOfAdmin(id, Models.MyAWSConfigs.ActivitylogsDBtableName, "userid");
            dataGridActivityLogs.ItemsSource = aList;
            dataGridActivityLogs.Items.Refresh();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            //LoadActivityLogs(id).ConfigureAwait(false);
        }

        private void dataGridActivityLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBack_Click_1(object sender, RoutedEventArgs e)
        {
            ap = new AdminsPage("", mv);
            mv.MenuPage.Content = ap;
            ap.LoadData().ConfigureAwait(false);
        }

        private void dataGridActivityLogs_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }
    }
}
