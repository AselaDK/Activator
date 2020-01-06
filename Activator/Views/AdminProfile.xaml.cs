using Amazon.DynamoDBv2;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Activator.Models;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for AdminProfile.xaml
    /// </summary>
    public partial class AdminProfile : UserControl
    {
        private string myId = "";

        public AdminProfile()
        {
            InitializeComponent();
        }

        public AdminProfile(String id) : this()
        {
            this.myId = id;
            ShowProfileData(myId);
        }

        private void ShowProfileData(string myid)
        {
            try
            {
                Console.WriteLine("name   vvvvv- - - ", myid);
                ////Console.WriteLine(aPassword);

                try
                {
                    string tableName = "admin";

                    var client = new AmazonDynamoDBClient();
                    var table = Table.LoadTable(client, tableName);
                    var item = table.GetItem(myid);

                    //Console.WriteLine(item["aPassword"]);

                    if (item != null)
                    {
                        Console.WriteLine("name   - - - ",item["aName"]);
                        AdminName.Text = item["aName"];
                        AdminPhone.Text = item["aPhone"];
                        AdminEMail.Text = item["aId"];

                        string imagename = item["aPropic"];
                        S3Bucket.DownloadFile(imagename);
                        var BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
                        string filePath = BaseDirectoryPath + $"Resources/Images/{imagename}";
                        AdminDp.Source = new BitmapImage(new Uri(filePath));

                        // Create Image and set its width and height  
                        Image dynamicImage = new Image();
                        dynamicImage.Stretch = Stretch.Fill;
                        dynamicImage.StretchDirection = StretchDirection.Both;
                        dynamicImage.Width = 300;
                        dynamicImage.Height = 200;

                        // Create a BitmapSource  
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(@"");
                        bitmap.EndInit();

                        // Set Image.Source  
                        dynamicImage.Source = bitmap;

                        // Add Image to Window  
//                        AdminDp.Children.Add(dynamicImage);

                        //this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Can not Load Data!!!");

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
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnEditDetails_Click(object sender, RoutedEventArgs e)
        {
            EditAdminDetails editAdminDetails = new EditAdminDetails(myId);
            editAdminDetails.ShowDialog();
        }

        private void BtnEditPassword_Click(object sender, RoutedEventArgs e)
        {
            ChangeAdminPassword changeAdminPassword = new ChangeAdminPassword(myId);
            changeAdminPassword.ShowDialog();
        }
    }
}
