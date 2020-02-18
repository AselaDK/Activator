using Activator.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ChangeAdminPropic.xaml
    /// </summary>
    public partial class ChangeAdminPropic : MetroWindow
    {
        private string uploadFilePath;
        private readonly string myId = "";

        public ChangeAdminPropic()
        {
            InitializeComponent();
        }

        public ChangeAdminPropic(string id) : this()
        {
            InitializeComponent();
            this.myId = id;
        }

        private void ButtonClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
        private void ButtonChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files | *.jpg; *.jpeg; *.png";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                uploadFilePath = openFileDialog.FileName;

                Uri fileUri = new Uri(uploadFilePath);
                imgUploadImage.Source = new BitmapImage(fileUri);
            }
        }

        private async void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool isFilePathEmpty = string.IsNullOrEmpty(uploadFilePath);

                if (!isFilePathEmpty)
                {
                    ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "Uploading data");
                    controller.SetIndeterminate();
                    controller.SetCancelable(false);

                    string[] temp = uploadFilePath.Split('.');
                    string fileId = $"{myId}.{temp[temp.Length - 1]}";

                    string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
                    string filePath = BaseDirectoryPath + $"Resources\\Images\\{fileId}";

                    Item item = Dynamodb.GetItem(myId, MyAWSConfigs.AdminDBTableName);
                    string oldImage = item["aPropic"];
                    Console.WriteLine("><><><><><><><><><><>" + oldImage);

                    //Delete old profile pic in local
                    //string oldFilePath = BaseDirectoryPath + $"Resources\\Images\\{oldImage}";
                    //DeleteOldPic(oldFilePath);

                    //Delete old profile pic in s3Bucket
                    S3Bucket.DeleteFile(oldImage, MyAWSConfigs.AdminS3BucketName);

                    item["aPropic"] = fileId;

                    await Task.Run(() => Models.S3Bucket.UploadFile(uploadFilePath, fileId, MyAWSConfigs.AdminS3BucketName));
                    MessageBox.Show(fileId);
                    await Task.Run(() => Models.Dynamodb.UpdateItem(item, Models.MyAWSConfigs.AdminDBTableName));

                    await controller.CloseAsync();

                    await this.ShowMessageAsync("Success", "Changed Successfully..", MessageDialogStyle.Affirmative);

                    //activity recorded
                    Models.ActivityLogs.Activity(Models.Components.AdminComponent, "User Changed Profile Picture");

                    //imgUploadImage.Source = null;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Please check all fields", MessageDialogStyle.Affirmative);
                }
            }
            catch
            {
                await this.ShowMessageAsync("Error", "Task not completed", MessageDialogStyle.Affirmative);
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
                    File.Delete(dFilePath);
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
