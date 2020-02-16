using Activator.Models;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using Amazon.DynamoDBv2.DocumentModel;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Amazon.DynamoDBv2;
using System.IO;
using System.Threading.Tasks;
using Activator.Models;

namespace Activator.Views.Reader
{
    /// <summary>
    /// Interaction logic for EditReader.xaml
    /// </summary>
    public partial class EditReader : MetroWindow
    {
        private string uploadFilePath;
        private readonly AmazonDynamoDBClient client;
        string readid;

        public EditReader(string id)
        {
            InitializeComponent();
            readid = txtId.Text;
            LoadData(id).ConfigureAwait(false);

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

        private async Task LoadData(string id)
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                IEnumerable<RefPerson> temp = await Task.Run(() => RefPerson.GetAllRefPersons());
                List<RefPerson> refs = new List<RefPerson>(temp);
                List<String> selectedrefs = new List<String>();
                List<String> tickedreaders = new List<String>();

                //selectedrefs =
                
                var item = Dynamodb.GetItem(id , MyAWSConfigs.ReaderDBtableName);
                Console.WriteLine("dsdddddddddddddddddddddddd" + item["description"]);
                selectedrefs = item["refList"].AsListOfString();

                foreach (var r in refs)
                {
                    r.isCheckedRef = false;
                }               

                for (int i = 0; i < selectedrefs.Count; i++)
                {
                    foreach (var j in refs)
                    {
                        if (j.id == selectedrefs[i])
                        {
                            tickedreaders.Add(j.id);
                            j.isCheckedRef = true;
                        }
                    }
                }

                lblLoading.Visibility = Visibility.Hidden;

                foreach (var j in refs)
                {
                    
                    //tickedreaders.Add(j.id);
                    Console.WriteLine("\n"+j.isCheckedRef);
                    
                }

                RefDataGrid.ItemsSource = refs;
                RefDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private async void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("this is submit button");
            String name = txtName.Text;
            //MessageBox.Show(name);

            try
            {
                bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
                bool isPhoneEmpty = string.IsNullOrEmpty(txtPhone.Text);
                bool isDescriptionEmpty = string.IsNullOrEmpty(txtDescription.Text);
                bool isFilePathEmpty = string.IsNullOrEmpty(uploadFilePath);
                bool isFileIdEmpty = string.IsNullOrEmpty(txtId.Text);

                if (!isNameEmpty && !isDescriptionEmpty && !isPhoneEmpty)
                {
                    string tableName = MyAWSConfigs.ReaderDBtableName;
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
                    doc["phone"] = txtPhone.Text;
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

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
