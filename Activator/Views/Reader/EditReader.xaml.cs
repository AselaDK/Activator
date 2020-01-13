using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Activator.Views.Reader
{
    /// <summary>
    /// Interaction logic for EditReader.xaml
    /// </summary>
    public partial class EditReader : MetroWindow
    {
        public EditReader()
        {
            InitializeComponent();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("this is submit button");
        }
    }
}
