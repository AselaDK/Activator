using Activator.Views.Reader;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Activator.Models;
using System.Threading.Tasks;
using System.IO;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ReadersView.xaml
    /// </summary>
    public partial class ReadersPage : UserControl
    {
        public ReadersPage()
        {
            InitializeComponent();
        }

        private void RegRaeder_Click(object sender, RoutedEventArgs e)
        {
            AddReader readerForm = new AddReader();
            readerForm.ShowDialog();
            readerForm.LoadData().ConfigureAwait(false);
        }

        private readonly string aId = null;


        public async Task LoadData()
        {
            progressBar.Visibility = Visibility.Visible;
            BtnRefresh.IsEnabled = false;
            try
            {
                IEnumerable<Models.Reader> tempReaders = await Task.Run(() => Models.Reader.GetReadersData());

                List<Models.Reader> readers = new List<Models.Reader>(tempReaders);

                string directoryPath = "Resources/Images/";

                foreach (Models.Reader reader in readers)
                {
                    if (!File.Exists(directoryPath + reader.propic))
                    {
                        await Task.Run(() => Models.S3Bucket.DownloadFile(reader.propic, Models.MyAWSConfigs.ReaderS3BucketName));
                    }

                    string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

                    Uri fileUri = new Uri(exeDirectory + directoryPath + reader.propic);

                    reader.rImage = new BitmapImage(fileUri);                    
                }

                ReaderDataGrid.ItemsSource = readers;
                ReaderDataGrid.Items.Refresh();
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
                BtnRefresh.IsEnabled = true;
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string tableName = Models.MyAWSConfigs.AdminDBTableName;
            //var table = Table.LoadTable(client, tableName);
            //var item = table.GetItem(aId);
            //LoadData(item);
        }

        public String GetCheckedRefPeople()
        {
            List<Models.Reader> SelectedPeople = new List<Models.Reader>();
            String SelectedPeopleIdList;

            SelectedPeople.Add((Models.Reader)ReaderDataGrid.SelectedItems[0]);
            SelectedPeopleIdList = SelectedPeople[0].id;
            Console.WriteLine(SelectedPeopleIdList);

           
            return SelectedPeopleIdList;
        }

        private void ReaderDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            String selectedList = GetCheckedRefPeople();
            if (selectedList != null)
            {
                EditReader editReader = new EditReader(selectedList);
                editReader.DataContext = ReaderDataGrid.SelectedItem;
                editReader.ShowDialog();
            }
            else
            {
                //DeleteButton.IsEnabled = false;
                MessageBox.Show("id is null");
            }

            //editReader.txtName.Text = Convert.ToString(this.name);
            //editReader.txtPhone.Text = Convert.ToString(this.phone);
            //editReader.txtDescription.Text = Convert.ToString(this.description);



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadData().ConfigureAwait(false); 
        }
    }
}

