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
    /// Interaction logic for AllPeoplePageView.xaml
    /// </summary>
    public partial class AllPeoplePageView : UserControl
    {
        public AllPeoplePageView()
        {
            InitializeComponent();

            LoadPersonsData().ConfigureAwait(false);
        }

        private void BtnAddNewRef_Click(object sender, RoutedEventArgs e)
        {
            AddNewRef addNewRef = new AddNewRef();
            addNewRef.Show();
        }

        private async Task LoadPersonsData()
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                IEnumerable<Models.RefPerson> result = await Task.Run(() => Models.RefPerson.GetAllRefPersons());
                List<Models.RefPerson> temp = new List<Models.RefPerson>(result);
                                
                string directoryPath = "Resources/Images/";

                foreach (Models.RefPerson person in temp)
                {
                    if (!File.Exists(directoryPath + person.id))
                    {
                        await Task.Run(() => Models.S3Bucket.DownloadFile(person.id, Models.MyAWSConfigs.RefImagesBucketName));
                    }

                    string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

                    Uri fileUri = new Uri(exeDirectory + directoryPath + person.id);

                    person.image = new BitmapImage(fileUri);
                }

                dataGridAllRefPersons.ItemsSource = temp;
                dataGridAllRefPersons.Items.Refresh();
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private void GetCheckedList()
        {
            List<Models.RefPerson> selectedList = new List<Models.RefPerson>();

            foreach (Models.RefPerson person in dataGridAllRefPersons.ItemsSource)
            {
                CheckBox cb = SelectionColumn.GetCellContent(person) as CheckBox;
                if (cb != null && cb.IsChecked == true)
                {
                    selectedList.Add(person);
                }
            }

            foreach (Models.RefPerson person in selectedList)
            {
                Console.WriteLine(person.name);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPersonsData().ConfigureAwait(false);
        }

        private void GetSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            GetCheckedList();
        }
    }
}
