using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Activator.Models;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for EditAdminDetails.xaml
    /// </summary>
    public partial class EditAdminDetails : MetroWindow
    {
        private readonly string myId = null;
        private readonly AmazonDynamoDBClient client;
        readonly Table table = null;
        readonly Item item = null;
        public EditAdminDetails()
        {
            InitializeComponent();
        }

        public EditAdminDetails(String id) : this()
        {
            InitializeComponent();
            this.myId = id;
            try
            {
                this.client = new AmazonDynamoDBClient();
                string tableName = MyAWSConfigs.AdminDBTableName;
                table = Table.LoadTable(client, tableName);
                item = table.GetItem(myId);
                ShowProfileData();
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

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tableName = MyAWSConfigs.AdminDBTableName;
                Table table = Table.LoadTable(client, tableName);
                Item item = table.GetItem(myId);
                Console.Write("my adminid<<<<<<<<<<<<<<<<<<",myId,">>>>>>>>>>>>>>>>>>>");

                bool isFileIdEmpty = string.IsNullOrEmpty(txtEmail.Text);
                bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
                bool isPasswordEmpty = string.IsNullOrEmpty(txtPassword.Password);
                bool isPhoneEmpty = string.IsNullOrEmpty(txtPhone.Text);

                if (!isNameEmpty && !isPhoneEmpty && !isFileIdEmpty && !txtEmail.Text.Contains(" ") && !isPasswordEmpty)
                {
                    if (Models.HashMD5.MD5Hash(txtPassword.Password) == item["aPassword"])
                    {
                        Console.WriteLine("\n*** Executing UpdateMultipleAttributes() ***");
                        Console.WriteLine("\nUpdating multiple attributes....");
                        string partitionKey = myId;

                        Document doc = new Document();
                        doc["aId"] = partitionKey;
                        // List of attribute updates.
                        // The following replaces the existing authors list.
                        doc["aName"] = txtName.Text;
                        doc["aPhone"] = txtPhone.Text;

                        // Optional parameters.
                        UpdateItemOperationConfig config = new UpdateItemOperationConfig
                        {
                            // Get updated item in response.
                            ReturnValues = ReturnValues.AllNewAttributes
                        };
                        Document updatedadmin = table.UpdateItem(doc, config);
                        Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
                        MessageBox.Show("Successfully Updated!");

                        this.Close();
                        
                    }
                    else
                    {
                        MessageBox.Show("Message : Wrong Password!");
                    }
                }
                else
                {
                    MessageBox.Show("Message : Please fill all all the fields!!!");
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
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ShowProfileData()
        {
            txtEmail.Text = myId;
            txtName.Text = item["aName"];
            txtPhone.Text = item["aPhone"];
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
