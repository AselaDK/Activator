using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
    }
}
