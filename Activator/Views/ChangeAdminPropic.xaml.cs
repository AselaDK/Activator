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

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ChangeAdminPropic.xaml
    /// </summary>
    public partial class ChangeAdminPropic : MetroWindow
    {
        private string uploadFilePath;
        public ChangeAdminPropic()
        {
            InitializeComponent();
        }
        private readonly string myId = "";
        private readonly AmazonDynamoDBClient client;
        readonly Table table = null;
        readonly Item item = null;

        public ChangeAdminPropic(string id) : this()
        {
            InitializeComponent();
            this.myId = id;
            try
            {
                this.client = new AmazonDynamoDBClient();
                string tableName = "admin";
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
                    string filePath = BaseDirectoryPath + $"Resources/Images/{fileId}";
                    File.Delete(filePath);

                    var item = new Document();

                    item["aPropic"] = fileId;

                    await Task.Run(() => Models.S3Bucket.UploadFile(uploadFilePath, fileId));
                    await Task.Run(() => Models.Dynamodb.PutItem(item, Models.MyAWSConfigs.adminDBTableName));

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
    }
}
