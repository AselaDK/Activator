using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Activator.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Activator.Views.Reader
{
    /// <summary>
    /// Interaction logic for AssignReferences.xaml
    /// </summary>
    public partial class AssignReferences : MetroWindow
    {
        public AssignReferences()
        {
            InitializeComponent();
            InitData();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            GetCheckedRefPeople();
            //ReaderForm.
            this.Close();
        }

        public List<RefPerson> GetCheckedRefPeople()
        {
            List<RefPerson> SelectedPeople = new List<RefPerson>();
            List<String> SelectedPeopleIdList = new List<String>();

            for (int i = 0; i < RefDataGrid.SelectedItems.Count; i++)
            {
                SelectedPeople.Add((RefPerson)RefDataGrid.SelectedItems[i]);
                SelectedPeopleIdList.Add(SelectedPeople[i].id);
                Console.WriteLine(SelectedPeopleIdList[i]);
            }
            return SelectedPeople;
        }

        private void InitData()
        {
            LoadData();
        }

        protected void LoadData()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<RefPerson> refs = new List<RefPerson>();

                refs = RefPerson.GetAllRefPersons();

                Console.WriteLine();

                lblLoading.Visibility = Visibility.Hidden;

                RefDataGrid.ItemsSource = refs;
                RefDataGrid.Items.Refresh();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

    }
}
