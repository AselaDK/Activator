using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

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
            await SubmitAsync();
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
                uploadImage.Source = new BitmapImage(fileUri);
            }
        }

        private async Task SubmitAsync()
        {
            bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
            bool isDescriptionEmpty = string.IsNullOrEmpty(txtDescription.Text);
            bool isFilePatthEmpty = string.IsNullOrEmpty(uploadFilePath);

            if (!isNameEmpty && !isDescriptionEmpty && !isFilePatthEmpty)
            {
                // show progressbar
                ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "Uploading data");
                controller.SetIndeterminate();
                controller.SetCancelable(false);

                try
                {
                    // upload image
                    await Task.Run(() => Models.S3Bucket.UploadFile(uploadFilePath));

                    // add new item to the dynamodb
                    var item = new Document();

                    item["name"] = txtName.Text;
                    item["description"] = txtDescription.Text;

                    await Task.Run(() => Models.Dynamodb.AddItem(item, Models.MyAWSConfigs.refPersonsDBTableName));

                    // close progressbar
                    await controller.CloseAsync();

                    // show success dialog
                    await this.ShowMessageAsync("Success", "New Person added", MessageDialogStyle.Affirmative);
                }
                catch
                {
                    // close progressbar
                    await controller.CloseAsync();

                    // show error dialog
                    await this.ShowMessageAsync("Error", "Task not completed", MessageDialogStyle.Affirmative);
                }
            }
        }
    }
}
