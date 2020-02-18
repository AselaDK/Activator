using Activator.Models;
using Activator.Views.Admin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for CameraPageView.xaml
    /// </summary>
    public partial class AdminsPage : UserControl
    {

        private string aId;
        private MainView mv;
        private AdminActivityLog al;

        public AdminsPage(string aid, MainView mv)
        {
            aId = aid;
            //Console.WriteLine(">>>>>>>>>><<<<<<<<<<<<<,,,,,,,,,Constructor " + aid);
            InitializeComponent();
            aId = getId(aid);

            this.mv = mv;
            al = new AdminActivityLog(this, mv, aId, false);
        }

        //get admins list
        public async Task LoadData()
        {
            // progress bar
            progressBar.Visibility = Visibility.Visible;
            BtnRefresh.IsEnabled = false;

            try
            {
                IEnumerable<Models.Admin> tempAdmins = await Task.Run(() => Models.Admin.GetAdminDetails());

                List<Models.Admin> admins = new List<Models.Admin>(tempAdmins);

                string directoryPath = "Resources/Images/";

                // set images to list
                foreach (Models.Admin admin in admins)
                {
                    if (!File.Exists(directoryPath + admin.aPropic))
                    {
                        await Task.Run(() => Models.S3Bucket.DownloadFile(admin.aPropic, Models.MyAWSConfigs.AdminS3BucketName));
                    }

                    string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

                    Uri fileUri = new Uri(exeDirectory + directoryPath + admin.aPropic);

                    admin.aImage = new BitmapImage(fileUri);
                }

                AdminDataGrid.ItemsSource = admins;
                AdminDataGrid.Items.Refresh();
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
                BtnRefresh.IsEnabled = true;
            }
        }

        private String getId(String id)
        {
            aId = id;
            return id;
        }

        // only root admins can register admins, otherwise gives warnings
        private void RegAdmin_Click(object sender, RoutedEventArgs e)
        {
            string tableName = MyAWSConfigs.AdminDBTableName;

            var item = Dynamodb.GetItem(aId, tableName);

            if (item["root"].AsBoolean() == true)
            {
                RegisterAdmin acv = new RegisterAdmin();
                RegAdmin.IsEnabled = true;
                acv.ShowDialog();
            }
            else
            {
                MessageBox.Show("Only the rood admin can register the new users");
            }

        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            //string tableName = MyAWSConfigs.AdminDBTableName;
            //var table = Table.LoadTable(client, tableName);
            //var item = table.GetItem(aId);
            //LoadData(item);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadData().ConfigureAwait(false);
        }

        private void AdminDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            List<Models.Admin> SelectedPeople = new List<Models.Admin>();
            String SelectedPeopleIdList;

            SelectedPeople.Add((Models.Admin)AdminDataGrid.SelectedItems[0]);
            SelectedPeopleIdList = SelectedPeople[0].aId;
            //Console.WriteLine(SelectedPeopleIdList);

            if (SelectedPeopleIdList != null)
            {
                //load activity logs for selected admin
                mv.MenuPage.Content = al;
                al.LoadActivityLogs(SelectedPeopleIdList);
            }
            else
            {
                //DeleteButton.IsEnabled = false;
                MessageBox.Show("id is null");
            }
        }



    }
}
