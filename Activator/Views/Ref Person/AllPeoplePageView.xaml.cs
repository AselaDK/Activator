using Activator.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Activator.Views.Ref_Person;
using System.Windows.Media;
using Activator.Views.Reader;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for AllPeoplePageView.xaml
    /// </summary>
    public partial class AllPeoplePageView : UserControl
    {
        List<Models.RefPerson> refPersons = new List<Models.RefPerson>();

        public AllPeoplePageView()
        {
            InitializeComponent();
            InitData();
            DeleteButton.IsEnabled = false;
        }

        private void BtnAddNewRef_Click(object sender, RoutedEventArgs e)
        {
            AddNewRef addNewRef = new AddNewRef();
            addNewRef.Show();
        }

        private void InitData()
        {
            LoadPersonsData();
        }

        private void LoadPersonsData()
        {
            refPersons.Clear();

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {               
                refPersons = Models.RefPerson.GetAllRefPersons();

                //lblLoading.Visibility = Visibility.Hidden;

                dataGridAllRefPersons.ItemsSource = refPersons;
                dataGridAllRefPersons.Items.Refresh();
            }
            finally
            {
                Mouse.OverrideCursor = null;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadPersonsData();
        }

        private void GetSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            GetCheckedList();
        }

        public List<String> GetCheckedRefPeople()
        {
            List<RefPerson> SelectedPeople = new List<RefPerson>();
            List<String> SelectedPeopleIdList = new List<String>();

            for (int i = 0; i < dataGridAllRefPersons.SelectedItems.Count; i++)
            {
                SelectedPeople.Add((RefPerson)dataGridAllRefPersons.SelectedItems[i]);
                SelectedPeopleIdList.Add(SelectedPeople[i].id);
                Console.WriteLine(SelectedPeopleIdList[i]);
            }
            return SelectedPeopleIdList;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            List<String> selectedList = GetCheckedRefPeople();
            ConfirmDeleteRef confirmDeleteRef = new ConfirmDeleteRef(selectedList);
            confirmDeleteRef.ShowDialog();
        }

        private void dataGridAllRefPersons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<String> selectedList = GetCheckedRefPeople();
            if (selectedList.Count != 0)
            {
                DeleteButton.IsEnabled = true;
            }
            else
            {
                DeleteButton.IsEnabled = false;
            }
            
            
        }

        private void dataGridAllRefPersons_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
                //CellValue is a variable of type string.
                EditReference editReader = new EditReference();
                editReader.DataContext = dataGridAllRefPersons.SelectedItem;
                editReader.ShowDialog();

        }
    }
}
