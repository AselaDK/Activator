using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

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
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<Models.RefPerson> refPersons = new List<Models.RefPerson>();

                refPersons = Models.RefPerson.GetAllRefPersons();

                lblLoading.Visibility = Visibility.Hidden;

                dataGridAllRefPersons.ItemsSource = refPersons;
                dataGridAllRefPersons.Items.Refresh();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadPersonsData();
        }
    }
}
