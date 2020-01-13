using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;


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
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
