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
using System.Data.SqlClient;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using static Amazon.Internal.RegionEndpointProviderV2;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using System.Security.Cryptography;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : Window
    {


        public LoginView()
        {
            InitializeComponent();
            Console.WriteLine("Set");
        }

        //encrypter password
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            String aId = TxtUid.Text;
            String aPassword = TxtPassword.Password;
            String hashPassword = MD5Hash(aPassword);

            //Console.WriteLine(aId);
            //Console.WriteLine(aPassword);

            try
            {

                string tableName = "admin";

                var client = new AmazonDynamoDBClient();
                var table = Table.LoadTable(client, tableName);
                var item = table.GetItem(aId);

                //Console.WriteLine(item["aPassword"]);

                if (item != null && item["aPassword"] == hashPassword)
                {
                    Console.WriteLine("Successfully Logged in!!!");
                    MainView dashboard = new MainView();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username or Password is incorrect!");

                    //clear texboxes
                    TxtUid.Text = "";
                    TxtUid.BorderBrush = Brushes.Red;
                    //txtuid.Background = Brushes.LightSalmon;

                    TxtPassword.Password = "";
                    TxtPassword.BorderBrush = Brushes.Red;
                    //txtpassword.Background = Brushes.LightSalmon;
                }


            }
            catch (AmazonDynamoDBException ex)
            {
                MessageBox.Show("Message : Server Error", ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message : Unknown Error", ex.Message);
            }

        }

        private void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnForget_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please Contact the Developer Team. Thank You!");
        }
    }
}