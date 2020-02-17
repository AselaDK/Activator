using System.Threading.Tasks;
using System.Windows.Controls;
using Activator.Views.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for PeopleInPageView.xaml
    /// </summary>
    public partial class PeopleInPageView : UserControl
    {
        public PeopleInPageView()
        {
            InitializeComponent();
        }

        public async Task LoadData()
        {
            progressBar.Visibility = Visibility.Visible;
            BtnRefresh.IsEnabled = false;
            try
            {
                IEnumerable<Models.History> tempHistory= await Task.Run(() => Models.History.GetHistoryDetails());

                List<Models.History> readers = new List<Models.History>(tempHistory);

                HistoryDataGrid.ItemsSource = readers;
                HistoryDataGrid.Items.Refresh();
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
                BtnRefresh.IsEnabled = true;
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData().ConfigureAwait(false);
        }
    }
}
