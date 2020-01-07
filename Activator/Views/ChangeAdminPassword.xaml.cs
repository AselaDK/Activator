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
    /// Interaction logic for ChangeAdminPassword.xaml
    /// </summary>


    public partial class ChangeAdminPassword : MetroWindow
    {
        private readonly string myId = "";
        private readonly AmazonDynamoDBClient client;
        readonly Table table = null;
        readonly Item item = null;
        public ChangeAdminPassword()
        {
            InitializeComponent();
        }
        public ChangeAdminPassword(string id) : this()
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

        private void ButtonSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string tableName = MyAWSConfigs.adminDBTableName;
                var table = Table.LoadTable(client, tableName);
                var item = table.GetItem(myId);
                Console.Write("my adminid<<<<<<<<<<<<<<<<<<", myId, ">>>>>>>>>>>>>>>>>>>");

                bool isPasswordEmpty = string.IsNullOrEmpty(txtPassword.Password);
                bool isNewPasswordEmpty = string.IsNullOrEmpty(txtNewPassword.Password);
                bool isCPasswordEmpty = string.IsNullOrEmpty(txtNewCPassword.Password);

                if (!isPasswordEmpty && !isNewPasswordEmpty && !isCPasswordEmpty)
                {
                    if (HashMD5.MD5Hash(txtPassword.Password) == item["aPassword"])
                    {
                        Console.WriteLine("\n*** Executing UpdateMultipleAttributes() ***");
                        Console.WriteLine("\nUpdating multiple attributes....");
                        string partitionKey = myId;

                        Document doc = new Document();
                        doc["aId"] = partitionKey;
                        // List of attribute updates.
                        // The following replaces the existing authors list.
                        doc["aPassword"] = HashMD5.MD5Hash(txtNewPassword.Password);

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
    }
}
