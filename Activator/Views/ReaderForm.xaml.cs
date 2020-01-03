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
using Microsoft.Win32;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;




namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ReaderForm.xaml
    /// </summary>
    public partial class ReaderForm : MetroWindow
    {
        public ReaderForm()
        {
            InitializeComponent();
            
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("this is after button click");
        }

        private void ButtonChooseImage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("this is after image click");
        }
    }
}
