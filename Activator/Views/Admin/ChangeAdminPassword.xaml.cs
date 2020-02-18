using Activator.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for ChangeAdminPassword.xaml
    /// </summary>


    public partial class ChangeAdminPassword : MetroWindow
    {
        private readonly string myId = "";

        public ChangeAdminPassword()
        {
            InitializeComponent();
        }
        public ChangeAdminPassword(string id) : this()
        {
            InitializeComponent();
            this.myId = id;
        }


        private void ButtonClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string tableName = MyAWSConfigs.AdminDBTableName;
                var item = Dynamodb.GetItem(myId, tableName);
                //Console.Write("my adminid<<<<<<<<<<<<<<<<<<", myId, ">>>>>>>>>>>>>>>>>>>");

                bool isPasswordEmpty = string.IsNullOrEmpty(txtPassword.Password);
                bool isNewPasswordEmpty = string.IsNullOrEmpty(txtNewPassword.Password);
                bool isCPasswordEmpty = string.IsNullOrEmpty(txtNewCPassword.Password);

                if (!isPasswordEmpty && !isNewPasswordEmpty && !isCPasswordEmpty)
                {
                    //check enetered and saved password hashes are same
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

                        // update password
                        Dynamodb.UpdateItem(doc, tableName);

                        //activity recorded
                        Models.ActivityLogs.Activity(Models.Components.AdminComponent, "User Changed Password");


                        Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
                        MessageBox.Show("Successfully Updated!");

                        this.Close();

                    }
                    else
                    {
                        //activity recorded
                        Models.ActivityLogs.Activity(Models.Components.AdminComponent, "Wrong Password for change Password");

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
