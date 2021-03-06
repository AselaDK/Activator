﻿using Activator.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for EditAdminDetails.xaml
    /// </summary>
    public partial class EditAdminDetails : MetroWindow
    {
        private readonly string myId = null;

        public EditAdminDetails()
        {
            InitializeComponent();
        }

        public EditAdminDetails(String id) : this()
        {
            InitializeComponent();
            this.myId = id;
            ShowProfileData();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tableName = MyAWSConfigs.AdminDBTableName;
                Item item = Dynamodb.GetItem(myId, tableName);
                //Console.Write("my adminid<<<<<<<<<<<<<<<<<<", myId, ">>>>>>>>>>>>>>>>>>>");

                bool isFileIdEmpty = string.IsNullOrEmpty(txtEmail.Text);
                bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
                bool isPasswordEmpty = string.IsNullOrEmpty(txtPassword.Password);
                bool isPhoneEmpty = string.IsNullOrEmpty(txtPhone.Text);

                // check wether the fields are empty
                if (!isNameEmpty && !isPhoneEmpty && !isFileIdEmpty && !txtEmail.Text.Contains(" ") && !isPasswordEmpty)
                {
                    // check the password is correct
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

                        //update item
                        Dynamodb.UpdateItem(doc, tableName);
                        Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
                        MessageBox.Show("Successfully Updated!");

                        //activity recorded
                        Models.ActivityLogs.Activity(Models.Components.AdminComponent, "Admin Updated his Details");

                        this.Close();

                    }
                    else
                    {
                        //activity recorded
                        Models.ActivityLogs.Activity(Models.Components.AdminComponent, "Failed:Attempt: Wrong Pwd, update Details");

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

        //show profile in the edit window
        private void ShowProfileData()
        {
            try
            {
                string tableName = MyAWSConfigs.AdminDBTableName;
                Item item = Dynamodb.GetItem(myId, tableName);
                Console.Write("my adminid<<<<<<<<<<<<<<<<<<", myId, ">>>>>>>>>>>>>>>>>>>");
                txtEmail.Text = myId;
                txtName.Text = item["aName"];
                txtPhone.Text = item["aPhone"];

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

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
