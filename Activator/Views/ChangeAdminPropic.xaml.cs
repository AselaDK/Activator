using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Activator.Models;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ChangeAdminPropic.xaml
    /// </summary>
    public partial class ChangeAdminPropic : MetroWindow
    {
        private string uploadFilePath;
        private readonly string myId = "";
        private readonly AmazonDynamoDBClient client;
        readonly Table table = null;
        private Item item = null;

        public ChangeAdminPropic()
        {
            InitializeComponent();
        }
        
        public ChangeAdminPropic(string id) : this()
        {
            InitializeComponent();
            this.myId = id;
            try
            {
                this.client = new AmazonDynamoDBClient();
                string tableName = MyAWSConfigs.adminDBTableName;
                table = Table.LoadTable(client, tableName);
                item = table.GetItem(myId);
            }
            catch (AmazonDynamoDBException ex)
            {
                MessageBox.Show("Message : Server Error", ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message : Unknown Error", ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
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

                    item = table.GetItem(myId);
                    string oldImage = item["aPropic"];
                    Console.WriteLine("><><><><><><><><><><>" + oldImage);

                    //Delete old profile pic in local
                    string oldFilePath = BaseDirectoryPath + $"Resources\\Images\\{oldImage}";
                    DeleteOldPic(oldFilePath);

                    //Delete old profile pic in s3Bucket
                    S3Bucket.DeleteFile(oldImage);

                    item["aPropic"] = fileId;

                    await Task.Run(() => S3Bucket.UploadFile(uploadFilePath, fileId));
                    await Task.Run(() => Dynamodb.PutItem(item, MyAWSConfigs.adminDBTableName));

                    await controller.CloseAsync();

                    await this.ShowMessageAsync("Success", "New Admin is Successfully Registered", MessageDialogStyle.Affirmative);

                    imgUploadImage.Source = null;
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
