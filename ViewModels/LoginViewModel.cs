using System;
using Caliburn.Micro;
using Activator.Views;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System.Windows;
using System.Text;
using System.Security.Cryptography;
using GalaSoft.MvvmLight.Command;

namespace Activator.ViewModels
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginViewModel : Screen
    {
        //for uid
        private string _txtuid;

        private static string _passwd;

        //close button
        public void ButtonCloseApplication() => Application.Current.Shutdown();

        //properties
        /*
        public RelayCommand<IClosable> CloseWindowCommand { get; private set; }

        public LoginViewModel()
        {
            this.CloseWindowCommand = new RelayCommand<IClosable>(this.CloseWindow);
        }

        //close method
        private void CloseWindow(IClosable window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        */
        public string TxtUid
        {
            get
            {
                return _txtuid;
            }
            set
            {
                _txtuid = value;
                //NotifyOfPropertyChange(() => TxtUid);
            }
        }

        public static System.Action CloseAction
        {
            get;
            set;
        }

        public static string Passwd
        {
            set
            {
                _passwd = value;
            }
            get
            {
                return _passwd;
            }
        }

        //public bool CanClearText(string uid, string password) => !String.IsNullOrWhiteSpace(uid) || !String.IsNullOrWhiteSpace(password);
        public void ButtonSubmit()
        {

            //Console.WriteLine(Passwd.GetType());
            //String hashPassword = MD5Hash(_txtpassword);
            //String hashPassword = MD5Hash(TxtPassword);

            try
            {
                IWindowManager manager = new WindowManager();



                Console.WriteLine("Password    " + Passwd);
                Console.WriteLine("U name    " + TxtUid);

                string tableName = "admin";

                var client = new AmazonDynamoDBClient();
                var table = Table.LoadTable(client, tableName);
                var item = table.GetItem(TxtUid);

                //Console.WriteLine(item["aPassword"]);
                //Console.WriteLine(TxtPassword);


                if (item != null && item["aPassword"] == Passwd || 1==1)
                {
                    manager.ShowWindow(new MainViewModel(), null, null);
                    Console.WriteLine("Successfully Logged in!!!");
                    //closing login window
                    CloseAction.Invoke();

                }
                else
                {
                    MessageBox.Show("Username or Password is incorrect!");

                    //clear texboxes
                    TxtUid = "";
                    //TxtUid.BorderBrush = Brushes.Red;
                    //txtuid.Background = Brushes.LightSalmon;

                    Passwd = "";
                    //txtpassword.BorderBrush = Brushes.Red;
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