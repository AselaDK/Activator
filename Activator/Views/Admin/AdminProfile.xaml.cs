using Activator.Models;
using Amazon.DynamoDBv2;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for AdminProfile.xaml
    /// </summary>
    public partial class AdminProfile : UserControl
    {
        private string uploadFilePath;
        private string myId = null;
        private string tableName = null;
        private Item item = null;

        public AdminProfile(String id)
        {
            InitializeComponent();
            myId = id;
        }

        public async Task ShowProfileData(string myid)
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                Console.WriteLine("name   vvvvv- - - ", myid);
                ////Console.WriteLine(aPassword);

                try
                {
                    tableName = MyAWSConfigs.AdminDBTableName;
                    item = Dynamodb.GetItem(myid, tableName);

                    //Console.WriteLine(item["aPassword"]);

                    if (item != null)
                    {
                        Console.WriteLine("name   - - - ", item["aName"]);
                        AdminName.Text = item["aName"];
                        AdminPhone.Text = item["aPhone"];
                        AdminEMail.Text = item["aId"];

                        string imagename = item["aPropic"];
                        await Task.Run(() => Models.S3Bucket.DownloadFile(imagename, MyAWSConfigs.AdminS3BucketName));
                        var BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
                        string filePath = BaseDirectoryPath + $"Resources/Images/{imagename}";
                        AdminDp.Source = new BitmapImage(new Uri(filePath));

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

        private async void BtnChangePropic_Click(object sender, RoutedEventArgs e)
        {
            //ChangeAdminPropic changeAdminPassword = new ChangeAdminPropic(myId);
            //changeAdminPassword.ShowDialog();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files | *.jpg; *.jpeg; *.png";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            //open file dialog
            if (openFileDialog.ShowDialog() == true)
            {
                uploadFilePath = openFileDialog.FileName;
                AdminDp.Source = null;
            }

            try
            {

                bool isFilePathEmpty = string.IsNullOrEmpty(uploadFilePath);

                if (!isFilePathEmpty)
                {
                    tableName = MyAWSConfigs.AdminDBTableName;

                    string[] temp = uploadFilePath.Split('.');
                    string fileId = $"{myId}.{temp[temp.Length - 1]}";

                    string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

                    //get current dp name
                    item = Dynamodb.GetItem(myId, tableName);
                    string oldImage = item["aPropic"];
                    Console.WriteLine("><><><><><><><><><><>" + oldImage);

                    //Delete old profile pic in local
                    string oldFilePath = BaseDirectoryPath + $"Resources\\Images\\{oldImage}";
                    DeleteOldPic(oldFilePath);

                    //Delete old profile pic in s3Bucket
                    S3Bucket.DeleteFile(oldImage, MyAWSConfigs.AdminS3BucketName);

                    item["aPropic"] = fileId;

                    await Task.Run(() => S3Bucket.UploadFile(uploadFilePath, fileId, MyAWSConfigs.AdminS3BucketName));

                    MessageBox.Show("Success", "Successfully Updated!");

                }
                else
                {
                    MessageBox.Show("Error", "Please check all fields");
                }
            }
            catch
            {
                MessageBox.Show("Error", "Task not completed");
            }

        }

        private static void DeleteOldPic(string filePath)
        {
            try
            {
                // Check if file exists with its full path
                string dFilePath = filePath;
                Console.WriteLine(dFilePath);
                if (File.Exists(dFilePath))
                {
                    //remove other processes on file
                    StreamReader sr = new StreamReader(@dFilePath);
                    //Console.Write(sr.ReadToEnd());
                    sr.Close();
                    sr.Dispose();
                    // If file found, delete it  
                    File.Delete(@dFilePath);
                    Console.WriteLine(">>>>>>>>>>>>>>>>File deleted.");
                }
                else Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>File not found");
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine(ioExp.Message);
            }
        }
    }
}
