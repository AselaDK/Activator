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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Activator.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using System.Drawing;

namespace Activator.Views.Admin
{
    /// <summary>
    /// Interaction logic for AdminActivityLog.xaml
    /// </summary>
    public partial class AdminActivityLog : UserControl
    {
        private AdminsPage ap;
        private MainView mv;
        private AdminProfile apr;
        private string id;
        private bool blocked;   //blocked admins check
        private string blockId;     //blocked admins id
        private bool isFromProfile;     //page loaded from profile or not

        public AdminActivityLog(AdminsPage ap, MainView mv, string id, bool isFromProfile)
        {
            InitializeComponent();
            this.ap = ap;
            this.mv = mv;
            this.id = id;
            this.isFromProfile = isFromProfile;

            //hide block button if page is loaded from ProfilePage
            if (isFromProfile == true)
            {
                block_toggle.Visibility = Visibility.Hidden;
                text2.Visibility = Visibility.Hidden;
            }

        }

        //load admins activity logs for each clicked
        public void LoadActivityLogs(string id)
        {
            blockId = id;

            var aList = Models.Dynamodb.GetActivitiesOfAdmin(blockId, "userid");
            dataGridActivityLogs.ItemsSource = aList;
            dataGridActivityLogs.Items.Refresh();

            //check admin is blocked or not
            Item item = Models.Dynamodb.GetItem(blockId, Models.MyAWSConfigs.AdminDBTableName);
            blocked = item["blocked"].AsBoolean();
            txtAdminName.Text = item["aName"];

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
                block_toggle.Visibility = Visibility.Hidden;
                text2.Text = "Root Admin";
                text2.DataContext = Color.Red;

            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadActivityLogs(blockId);
        }

        private void BtnBack_Click_1(object sender, RoutedEventArgs e)
        {
            // if page loaded from profile it will back to profile
            if (isFromProfile == true)
            {
                apr = new AdminProfile(id, mv, null);
                mv.MenuPage.Content = apr;
                apr.ShowProfileData(id).ConfigureAwait(false);
                Console.WriteLine("<<<<<<<<<<<<<<<<<<<< From Profilr");
                //apr.LoadData().ConfigureAwait(false);
            }
            else
            {
                // if page loaded from adminspage it will back to adminspage

                ap = new AdminsPage("", mv);
                mv.MenuPage.Content = ap;
                ap.LoadData().ConfigureAwait(false);
                Console.WriteLine("<<<<<<<<<<<<<<<<<<<< not From Profilr");

            }
        }

        //toggle button to block admins
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
