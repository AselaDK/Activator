using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows.Input;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for AddNewRef.xaml
    /// </summary>
    public partial class AddNewRef : MetroWindow
    {
        private string uploadFilePath;

        private void ButtonCloseAddNewRef_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
                bool isDescriptionEmpty = string.IsNullOrEmpty(txtDescription.Text);
                bool isFilePathEmpty = string.IsNullOrEmpty(uploadFilePath);
                bool isFileIdEmpty = string.IsNullOrEmpty(txtId.Text);

                if (!isNameEmpty && !isDescriptionEmpty && !isFilePathEmpty && !isFileIdEmpty && !txtId.Text.Contains(" "))
                {
                    ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "Uploading data");
                    controller.SetIndeterminate();
                    controller.SetCancelable(false);

                    string[] temp = uploadFilePath.Split('.');
                    string fileId = $"{txtId.Text}.{temp[temp.Length - 1]}";

                    var item = new Document();

                    item["id"] = fileId;
                    item["name"] = txtName.Text;
                    item["description"] = txtDescription.Text;

                    await Task.Run(() => Models.S3Bucket.UploadFile(uploadFilePath, fileId,Models.MyAWSConfigs.refImagesBucketName));
                    await Task.Run(() => Models.Dynamodb.PutItem(item, Models.MyAWSConfigs.refPersonsDBTableName));
                    await Task.Run(() => Models.FaceCollection.AddFace(fileId, Models.MyAWSConfigs.faceCollectionID));

                    await controller.CloseAsync();

                    await this.ShowMessageAsync("Success", "New Person added", MessageDialogStyle.Affirmative);

                    txtName.Text = "";
                    txtDescription.Text = "";
                    txtId.Text = "";
                    imgUploadImage.Source = null;

                    MessageBox.Show("file path  :" + uploadFilePath + "and file Id :" + fileId);
                    
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

        public AddNewRef()
        {
            InitializeComponent();         
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
    }
}
