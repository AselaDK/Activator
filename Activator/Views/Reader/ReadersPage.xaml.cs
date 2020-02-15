using Activator.Views.Reader;
using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;



namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ReadersView.xaml
    /// </summary>
    public partial class ReadersPage : UserControl
    {
        List<Models.Reader> readers = new List<Models.Reader>();
        public ReadersPage()
        {
            InitializeComponent();
            //LoadData();
        }

        private void RegRaeder_Click(object sender, RoutedEventArgs e)
        {
            AddReader readerForm = new AddReader();
            readerForm.ShowDialog();
        }

        private AmazonDynamoDBClient client;
        private readonly string aId = null;

        protected void LoadData()
        {
            //readers.Clear();
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                readers = Models.Reader.GetReadersData();

                Console.WriteLine();

                lblLoading.Visibility = Visibility.Hidden;

                ReaderDataGrid.ItemsSource = readers;
                ReaderDataGrid.Items.Refresh();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string tableName = Models.MyAWSConfigs.AdminDBTableName;
            var table = Table.LoadTable(client, tableName);
            var item = table.GetItem(aId);
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

        private string GetSelectedValue(DataGrid grid)
        {
            DataGridCellInfo cellInfo = grid.SelectedCells[0];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, TagProperty, column.Binding);

            return element.Tag.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}

