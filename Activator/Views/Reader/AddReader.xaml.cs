using Activator.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ReaderForm.xaml
    /// </summary>
    public partial class AddReader : MetroWindow
    {
        private string uploadFilePath;
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        public AddReader()
        {
            InitializeComponent();
            LoadData();
        }


        public async Task LoadData()
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                IEnumerable<Models.RefPerson> temp = await Task.Run(() => Models.RefPerson.GetAllRefPersons());
                List<Models.RefPerson> refs = new List<RefPerson>(temp);

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

        public List<String> GetCheckedRefPeople()
        {
            List<RefPerson> SelectedPeople = new List<RefPerson>();
            List<String> SelectedPeopleIdList = new List<String>();

            for (int i = 0; i < RefDataGrid.SelectedItems.Count; i++)
            {
                SelectedPeople.Add((RefPerson)RefDataGrid.SelectedItems[i]);
                SelectedPeopleIdList.Add(SelectedPeople[i].id);
                Console.WriteLine(SelectedPeopleIdList[i]);
            }
            return SelectedPeopleIdList;
        }

        private async void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
                bool isDescriptionEmpty = string.IsNullOrEmpty(txtDescription.Text);
                bool isFilePathEmpty = string.IsNullOrEmpty(uploadFilePath);
                bool isFileIdEmpty = string.IsNullOrEmpty(txtId.Text);
                bool isPhoneEmpty = string.IsNullOrEmpty(txtPhone.Text);

                if (!isNameEmpty && !isDescriptionEmpty && !isFilePathEmpty && !isFileIdEmpty && !isPhoneEmpty)
                {
                    ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "Uploading data");
                    controller.SetIndeterminate();
                    controller.SetCancelable(false);

                    string[] temp = uploadFilePath.Split('.');
                    string propic = $"{txtId.Text}.{temp[temp.Length - 1]}";

                    var r_item = new Document();

                    List<String> SelectedPeople = new List<String>();
                    SelectedPeople = GetCheckedRefPeople();

                    r_item["id"] = txtId.Text;
                    r_item["name"] = txtName.Text;
                    r_item["description"] = txtDescription.Text;
                    r_item["phone"] = txtPhone.Text;
                    r_item["refList"] = SelectedPeople;
                    r_item["propic"] = propic;

                    await Task.Run(() => Models.S3Bucket.UploadFile(uploadFilePath, propic, Models.MyAWSConfigs.ReaderS3BucketName));
                    await Task.Run(() => Models.Dynamodb.PutItem(r_item, Models.MyAWSConfigs.ReaderDBtableName));

                    await controller.CloseAsync();

                    await this.ShowMessageAsync("Success", "New Person added", MessageDialogStyle.Affirmative);

                    txtName.Text = "";
                    txtDescription.Text = "";
                    txtId.Text = "";
                    txtPhone.Text = "";
                    imgUploadImage.Source = null;

                    GiveRedersToRefernces(SelectedPeople, propic);

                }
                else
                {
                    await this.ShowMessageAsync("Error", "Fill All The Fields", MessageDialogStyle.Affirmative);
                }
            }
            catch
            {
                await this.ShowMessageAsync("Error", "Task not completed", MessageDialogStyle.Affirmative);
            }
        }

        private void GiveRedersToRefernces(List<String> refList, String readerId)
        {
            try
            {
                Console.WriteLine(readerId);
                foreach (String chkd_ref in refList)
                {
                    string tableName = MyAWSConfigs.RefPersonsDBTableName;
                    Table table = Table.LoadTable(client, tableName);

                    Console.WriteLine("\n*** Executing UpdateMultipleAttributes() ***");
                    Console.WriteLine("\nUpdating multiple attributes....");
                    string partitionKey = chkd_ref;

                    Document doc = new Document();
                    doc["id"] = partitionKey;
                    // List of attribute updates.
                    // The following replaces the existing authors list.

                    Item item = table.GetItem(chkd_ref);
                    Console.WriteLine(item["id"]);
                    //Console.WriteLine(item["readerList"]);
                    List<string> readersList = new List<string>();
                    if (item["readerList"] != null)
                    {
                        readersList = item["readerList"].AsListOfString();
                        var match = readersList.FirstOrDefault(stringToCheck => stringToCheck.Contains(readerId));
                        if (match != null)
                        {
                            readersList.Add(readerId);
                            foreach (string i in readersList)
                            {
                                Console.WriteLine("reader !match >>>>>> " + i);
                            }
                            doc["readerList"] = readersList;
                            // Optional parameters.
                            UpdateItemOperationConfig config = new UpdateItemOperationConfig
                            {
                                // Get updated item in response.
                                ReturnValues = ReturnValues.AllNewAttributes
                            };
                            Document updatedadmin = table.UpdateItem(doc, config);
                            Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
                            //MessageBox.Show("Successfully Updated! not null");
                        }
                        else
                        {
                            readersList.Add(readerId);
                            foreach (string i in readersList)
                            {
                                Console.WriteLine("reader match >>>>>> " + i);
                            }
                            doc["readerList"] = readersList;
                            // Optional parameters.
                            UpdateItemOperationConfig config = new UpdateItemOperationConfig
                            {
                                // Get updated item in response.
                                ReturnValues = ReturnValues.AllNewAttributes
                            };
                            Document updatedadmin = table.UpdateItem(doc, config);
                            Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
                            //MessageBox.Show("Successfully Updated! not null");
                        }
                    }
                    else
                    {
                        foreach (string i in readersList)
                        {
                            Console.WriteLine("reader null >>>>>> " + i);
                        }

                        doc["readerList"] = readersList;
                        // Optional parameters.
                        UpdateItemOperationConfig config = new UpdateItemOperationConfig
                        {
                            // Get updated item in response.
                            ReturnValues = ReturnValues.AllNewAttributes
                        };
                        Document updatedadmin = table.UpdateItem(doc, config);
                        Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
                        MessageBox.Show("Successfully Updated! null");
                    }

                }
            }
            catch (AmazonDynamoDBException ex)
            {
                MessageBox.Show("Message : Server Error", ex.Message);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Message : Unknown Error- Updating Refs", ex.Message);
            }
            finally
            {

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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
