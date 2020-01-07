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

using Amazon.DynamoDBv2.DocumentModel;

namespace Activator.Views
    
{
    /// <summary>
    /// Interaction logic for Readers.xaml
    /// </summary>
    public partial class Readers : UserControl
    {
        public Readers()
        {
            InitializeComponent();
            //Document item = new Document();
            //item["indexNo"] ="17000203";
            //item["name"] = "sahan";
            //Task.Run(() => Models.Dynamodb.PutItem(item, "bodima"));
            //MessageBox.Show("this is from db");

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           // M adding form when click 
           // AddNewRef addNewRef = new AddNewRef();
           // addNewRef.Show();
            ReaderForm readerForm= new ReaderForm();
            readerForm.Show();
            


        }
        private void message()
        {
            MessageBox.Show("this is one of the message");
        }
    }
}
