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
using Activator.Models;

namespace Activator.Views.Ref_Person
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteRef.xaml
    /// </summary>
    public partial class ConfirmDeleteRef : Window
    {
        List<String> deleteList;
        public ConfirmDeleteRef(List<String> selectedList)
        {
            InitializeComponent();
            deleteList = selectedList;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var i in deleteList)
            {
                Dynamodb.DeleteItem(i, MyAWSConfigs.RefPersonsDBTableName);
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
