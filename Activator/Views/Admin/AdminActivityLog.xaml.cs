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
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Activator.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

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
        private bool blocked;
        private string blockId;

        public AdminActivityLog(AdminsPage ap, MainView mv, string id, bool isFromProfile)
        {
            InitializeComponent();
            this.ap = ap;
            this.mv = mv;
            this.id = id;

            if (isFromProfile == true)
            {
                block_toggle.Visibility = Visibility.Hidden;
                text2.Visibility = Visibility.Hidden;
            }

        }
        public void LoadActivityLogs(string id)
        {
            blockId = id;

            var aList = Models.Dynamodb.GetActivitiesOfAdmin(blockId, Models.MyAWSConfigs.ActivitylogsDBtableName, "userid");
            dataGridActivityLogs.ItemsSource = aList;
            dataGridActivityLogs.Items.Refresh();

            //check admin is blocked or not
            Item item = Models.Dynamodb.GetItem(blockId, Models.MyAWSConfigs.AdminDBTableName);
            blocked = item["blocked"].AsBoolean();

            if (blocked == true)
            {
                block_toggle.IsChecked = true;
                //Console.WriteLine(blockId + ">>>>>>>>>>Blocked");
            }
            else
            {
                block_toggle.IsChecked = false;
                Console.WriteLine(blockId + ">>>>>>>>>NOtBlocked");
            }
            //cant block a root admin
            if (item["root"].AsBoolean() == true)
            {
                //MessageBox.Show("You can't Block Root Admins!");
                block_toggle.IsEnabled = false;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadActivityLogs(id);
        }

        private void dataGridActivityLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void BlockButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void block_toggle_Checked(object sender, RoutedEventArgs e)
        {                
            Document doc = new Document();
            doc["aId"] = blockId;
            // List of attribute updates.
            // The following replaces the existing authors list.
            doc["blocked"] = true;

            Dynamodb.UpdateItem(doc, Models.MyAWSConfigs.AdminDBTableName);
            MessageBox.Show(blockId + " is Blocked!");
           
        }

        private void block_toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Document doc = new Document();
            doc["aId"] = blockId;
            // List of attribute updates.
            // The following replaces the existing authors list.
            doc["blocked"] = false;

            Dynamodb.UpdateItem(doc, Models.MyAWSConfigs.AdminDBTableName);
        }
    }
}
