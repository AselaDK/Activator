using Activator.Models;
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
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace Activator.Views.Ref_Person
{
    /// <summary>
    /// Interaction logic for EditReference.xaml
    /// </summary>
    public partial class EditReference : MetroWindow
    {
        private string uploadFilePath;
        private readonly AmazonDynamoDBClient client;
        private bool isPicChanged = false;

        public EditReference()
        {
            InitializeComponent();
            try
            {
                this.client = new AmazonDynamoDBClient();
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

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonChooseImage_Click(object sender, RoutedEventArgs e)
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

        private async void buttonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
                bool isDescriptionEmpty = string.IsNullOrEmpty(txtDescription.Text);
                bool isFilePathEmpty = string.IsNullOrEmpty(uploadFilePath);
                bool isFileIdEmpty = string.IsNullOrEmpty(txtId.Text);

                if (!isNameEmpty && !isDescriptionEmpty)
                {
                    string tableName = MyAWSConfigs.RefPersonsDBTableName;
                    Table table = Table.LoadTable(client, tableName);

                    ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "");
                    controller.SetIndeterminate();
                    controller.SetCancelable(false);


                    string partitionKey = txtId.Text;
                    Console.WriteLine("oooooooooooooooooooooooooooooooo" + partitionKey);

                    var item = new Document();

                    Document doc = new Document();
                    doc["id"] = partitionKey;
                    doc["name"] = txtName.Text;
                    doc["description"] = txtDescription.Text;
                    ///////////////////////////////////////////////////       //#ToDo : Add readerList
                    //item["readerList"] = readerList;

                    UpdateItemOperationConfig config = new UpdateItemOperationConfig
                    {
                        // Get updated item in response.
                        ReturnValues = ReturnValues.AllNewAttributes
                    };

                    if (uploadFilePath != null)
                    {
                        string[] temp = uploadFilePath.Split('.');
                        string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
                        string filePath = BaseDirectoryPath + $"Resources\\Images\\{partitionKey}";
                        item = table.GetItem(partitionKey);
                        string oldImage = item["aPropic"];
                        Console.WriteLine("><><><><><><><><><><>" + oldImage);

                        //Delete old profile pic in local
                        string oldFilePath = BaseDirectoryPath + $"Resources\\Images\\{oldImage}";
                        DeleteOldPic(oldFilePath);

                        //Delete old profile pic in s3Bucket
                        controller.SetMessage("Deleting File");
                        await Task.Run(() => S3Bucket.DeleteFile(oldImage, MyAWSConfigs.RefImagesBucketName));


                        controller.SetMessage("Uploading file");
                        await Task.Run(() => Models.S3Bucket.UploadFile(uploadFilePath, partitionKey, Models.MyAWSConfigs.RefImagesBucketName));
                    }

                    controller.SetMessage("Adding database record");
                    await Task.Run(() => table.UpdateItem(doc, config));
                    Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
                    //MessageBox.Show("Successfully Updated!");

                    controller.SetMessage("Creating face indexes");
                    await Task.Run(() => Models.FaceCollection.AddFace(partitionKey, Models.MyAWSConfigs.FaceCollectionID));

                    await controller.CloseAsync();

                    await this.ShowMessageAsync("Success", "Person Updated !", MessageDialogStyle.Affirmative);

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

        private void imgUploadImage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isPicChanged = true;
            Console.WriteLine("pic changed ______");
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
