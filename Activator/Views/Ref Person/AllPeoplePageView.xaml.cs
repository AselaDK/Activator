﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for AllPeoplePageView.xaml
    /// </summary>
    public partial class AllPeoplePageView : UserControl
    {
        private MainView mv;
        private DetectedPerson dp;
        public AllPeoplePageView(MainView mv)
        {
            InitializeComponent();

            this.mv = mv;
            dp = new DetectedPerson();
        }

        private void BtnAddNewRef_Click(object sender, RoutedEventArgs e)
        {
            AddNewRef addNewRef = new AddNewRef();
            addNewRef.Show();
        }

        public async Task LoadPersonsData()
        {
            progressBar.Visibility = Visibility.Visible;
            BtnRefresh.IsEnabled = false;
            try
            {
                IEnumerable<Models.RefPerson> tempPersons = await Task.Run(() => Models.RefPerson.GetAllRefPersons());
                IEnumerable<Models.Camera> tempCameras = await Task.Run(() => Models.Camera.GetAllCamers());
                
                List<Models.RefPerson> persons = new List<Models.RefPerson>(tempPersons);
                List<Models.Camera> cameras= new List<Models.Camera>(tempCameras);

                string directoryPath = "Resources/Images/";

                foreach (Models.RefPerson person in persons)
                {
                    if (!File.Exists(directoryPath + person.id))
                    {
                        await Task.Run(() => Models.S3Bucket.DownloadFile(person.id, Models.MyAWSConfigs.RefImagesBucketName));
                    }

                    string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

                    Uri fileUri = new Uri(exeDirectory + directoryPath + person.id);

                    person.image = new BitmapImage(fileUri);

                    if (string.IsNullOrEmpty(person.camera))
                        person.lastLocation = "Unknown";
                    else
                        person.lastLocation = cameras.Find(c => c.id == person.camera).location;
                }
                
                dataGridAllRefPersons.ItemsSource = persons;
                dataGridAllRefPersons.Items.Refresh();
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
                BtnRefresh.IsEnabled = true;
            }
        }

        private void dataGridRefPersons_Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            TextBlock id = dataGridAllRefPersons.Columns[1].GetCellContent(row) as TextBlock;
            TextBlock name = dataGridAllRefPersons.Columns[2].GetCellContent(row) as TextBlock;
            TextBlock description = dataGridAllRefPersons.Columns[5].GetCellContent(row) as TextBlock;
            ImageSource imageSource = (VisualTreeHelper.GetChild(dataGridAllRefPersons.Columns[0].GetCellContent(row), 0) as Image).Source;

            mv.MenuPage.Content = dp;
            dp.LoadPerson(id.Text, name.Text, description.Text, imageSource, mv, this, "ref");
        }

        //private void GetCheckedList()
        //{
        //    List<Models.RefPerson> selectedList = new List<Models.RefPerson>();

        //    foreach (Models.RefPerson person in dataGridAllRefPersons.ItemsSource)
        //    {
        //        CheckBox cb = SelectionColumn.GetCellContent(person) as CheckBox;
        //        if (cb != null && cb.IsChecked == true)
        //        {
        //            selectedList.Add(person);
        //        }
        //    }

        //    foreach (Models.RefPerson person in selectedList)
        //    {
        //        Console.WriteLine(person.name);
        //    }
        //}

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPersonsData().ConfigureAwait(false);
        }

        //private void GetSelectedButton_Click(object sender, RoutedEventArgs e)
        //{
        //    GetCheckedList();
        //}
    }
}
