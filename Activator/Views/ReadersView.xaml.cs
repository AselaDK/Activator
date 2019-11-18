using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ReadersView.xaml
    /// </summary>
    public partial class ReadersView : Window
    {
        public ReadersView()
        {
            InitializeComponent();
        }

        private void Add_Readers(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add readers ");
        }

        private void CamerasDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
