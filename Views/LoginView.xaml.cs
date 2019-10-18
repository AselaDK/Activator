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
using Activator.ViewModels;
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

            string passwd = TxtPassword.Password;
            string hashPassword = MD5Hash(passwd);
            Console.WriteLine(hashPassword.GetType());
            

            //LoginViewModel.

            if (LoginViewModel.CloseAction == null)
            {
               
                LoginViewModel.CloseAction = new Action(this.Close);
                Console.WriteLine("Close Set");
            }

            if (LoginViewModel.Passwd == null)
            {
                //LoginViewModel.Passwd = new string(hashPassword);
            }

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
    }
}