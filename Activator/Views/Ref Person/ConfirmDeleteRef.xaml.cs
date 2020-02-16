using Activator.Models;
using System;
using System.Collections.Generic;
using System.Windows;

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
            foreach (var i in deleteList)
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
