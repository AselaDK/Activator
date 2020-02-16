using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows.Media;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for AddNewRef.xaml
    /// </summary>
    public partial class AddNewRef : MetroWindow
    {
        private string uploadFilePath;

        bool isNew;
        string id, name, description;
        ImageSource imageSource;

        DetectedPerson detectedPerson;
        AllPeoplePageView allPeoplePageView;

        public AddNewRef(string id, string name, string description, ImageSource imageSource, DetectedPerson detectedPerson, AllPeoplePageView allPeoplePageView, bool isNew)
        {
            InitializeComponent();

            this.isNew = isNew;

            if (!isNew)
            {
                lblTitle.Content = "UPDATE PERSON";
                buttonSubmit.Content = "UPDATE";

                this.detectedPerson = detectedPerson;

                this.id = id;
                this.name = name;
                this.description = description;
                this.imageSource = imageSource;                

                txtId.Text = id.Split('.')[0];
                txtId.IsEnabled = false;

                txtName.Text = name;
                txtDescription.Text = description;
                imgUploadImage.Source = imageSource;
            }
            else
            {
                this.allPeoplePageView = allPeoplePageView;
                lblTitle.Content = "NEW PERSON";
                buttonSubmit.Content = "SUBMIT";
            }
               
        }

        private async void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
                bool isDescriptionEmpty = string.IsNullOrEmpty(txtDescription.Text);
                bool isFilePathEmpty = string.IsNullOrEmpty(uploadFilePath);
                bool isFileIdEmpty = string.IsNullOrEmpty(txtId.Text);

                if (isNew)
                {
                    if (!isNameEmpty && !isDescriptionEmpty && !isFilePathEmpty && !isFileIdEmpty && !txtId.Text.Contains(" "))
                    {
                        ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "");
                        controller.SetIndeterminate();
                        controller.SetCancelable(false);

                        string[] temp = uploadFilePath.Split('.');
                        string fileId = $"{txtId.Text}.{temp[temp.Length - 1]}";

                        var item = new Document();
                        List<string> readerList = new List<string>() { "a" };

                        item["id"] = fileId;
                        item["name"] = txtName.Text;
                        item["description"] = txtDescription.Text;
                        item["status"] = 0;
                        item["readerList"] = readerList;

                        Console.WriteLine("done here");

                        controller.SetMessage("Uploading file");
                        await Task.Run(() => Models.S3Bucket.UploadFile(uploadFilePath, fileId, Models.MyAWSConfigs.RefImagesBucketName));

                        controller.SetMessage("Adding database record");
                        await Task.Run(() => Models.Dynamodb.PutItem(item, Models.MyAWSConfigs.RefPersonsDBTableName));

                        controller.SetMessage("Creating face indexes");
                        await Task.Run(() => Models.FaceCollection.AddFace(fileId, Models.MyAWSConfigs.FaceCollectionID));

                        await controller.CloseAsync();

                        txtName.Text = "";
                        txtDescription.Text = "";
                        txtId.Text = "";
                        imgUploadImage.Source = null;

                        this.allPeoplePageView.LoadPersonsData().ConfigureAwait(false); 

                        await this.ShowMessageAsync("Success", "New Person added", MessageDialogStyle.Affirmative);

                        this.Close();

                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Please check all fields", MessageDialogStyle.Affirmative);
                    }
                }
                else
                {
                    if (!isNameEmpty && !isDescriptionEmpty)
                    {
                        ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "");
                        controller.SetIndeterminate();
                        controller.SetCancelable(false);

                        var item = new Document();

                        item["id"] = this.id;
                        item["name"] = txtName.Text;
                        item["description"] = txtDescription.Text;

                        controller.SetMessage("Adding database record");
                        await Task.Run(() => Models.Dynamodb.UpdateItem(item, Models.MyAWSConfigs.RefPersonsDBTableName));

                        await controller.CloseAsync();

                        txtName.Text = "";
                        txtDescription.Text = "";
                        txtId.Text = "";
                        imgUploadImage.Source = null;

                        detectedPerson.BackToRef();

                        await this.ShowMessageAsync("Success", "Person Updated", MessageDialogStyle.Affirmative);

                        this.Close();
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Please check all fields", MessageDialogStyle.Affirmative);
                    } 
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
                Console.WriteLine(uploadFilePath);

                Uri fileUri = new Uri(uploadFilePath);
                imgUploadImage.Source = new BitmapImage(fileUri);
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
