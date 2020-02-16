﻿using Activator.Models;
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

        public AdminsPage(string aid)
        {
            aId = aid;
            Console.WriteLine(">>>>>>>>>><<<<<<<<<<<<<,,,,,,,,,Constructor " + aid);
            InitializeComponent();
            aId = getId(aid);
        }

        public async Task LoadData()
        {
            progressBar.Visibility = Visibility.Visible;
            BtnRefresh.IsEnabled = false;
            try
            {
                IEnumerable<Models.Admin> tempAdmins = await Task.Run(() => Models.Admin.GetAdminDetails());

                List<Models.Admin> admins = new List<Models.Admin>(tempAdmins);

                string directoryPath = "Resources/Images/";

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
    }
}
