using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for RegisterAdmin.xaml
    /// </summary>
    public partial class RegisterAdmin : MetroWindow
    {
        private string uploadFilePath;
        private bool isRoot = false;
        private string aRoot = "";
        public RegisterAdmin()
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

        private async void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isFileIdEmpty = string.IsNullOrEmpty(txtEmail.Text);
                bool isNameEmpty = string.IsNullOrEmpty(txtName.Text);
                bool isPasswordEmpty = string.IsNullOrEmpty(txtPassword.Password);
                bool isCPasswordEmpty = string.IsNullOrEmpty(txtCPassword.Password);
                bool isPhoneEmpty = string.IsNullOrEmpty(txtPhone.Text);
                bool isFilePathEmpty = string.IsNullOrEmpty(uploadFilePath);

                if (!isNameEmpty && !isPhoneEmpty && !isFilePathEmpty && !isFileIdEmpty && !txtEmail.Text.Contains(" ") && !isPasswordEmpty && !isCPasswordEmpty)
                {
                    if (txtPassword.Password == txtCPassword.Password)
                    {
                        ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "Uploading data");
                        controller.SetIndeterminate();
                        controller.SetCancelable(false);

                        string[] temp = uploadFilePath.Split('.');
                        string fileId = $"{txtEmail.Text}.{temp[temp.Length - 1]}";

                        var item = new Document();

                        item["aId"] = txtEmail.Text;
                        item["aName"] = txtName.Text;
                        item["aPassword"] = Models.HashMD5.MD5Hash(txtPassword.Password);
                        item["aPhone"] = txtPhone.Text;
                        item["aPropic"] = fileId;
                        item["root"] = isRoot;
                        item["blocked"] = false;

                        await Task.Run(() => Models.S3Bucket.UploadFile(uploadFilePath, fileId, Models.MyAWSConfigs.AdminS3BucketName));
                        await Task.Run(() => Models.Dynamodb.PutItem(item, Models.MyAWSConfigs.AdminDBTableName));

                        await controller.CloseAsync();

                        await this.ShowMessageAsync("Success", "New Admin is Successfully Registered", MessageDialogStyle.Affirmative);

                        txtEmail.Text = "";
                        txtName.Text = "";
                        txtPassword.Password = "";
                        txtCPassword.Password = "";
                        txtPhone.Text = "";
                        imgUploadImage.Source = null;

                        // activity recorded
                        string srnd = Models.Session.id + DateTime.Now.ToString();
                        Models.ActivityLogs.Activity(srnd, Models.Session.id, "Added new "+ aRoot + " admin", DateTime.Now.ToString());

                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Password confirmation must match Password!!!", MessageDialogStyle.Affirmative);
                    }
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

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void root_toggle_Checked(object sender, RoutedEventArgs e)
        {
            isRoot = true;
            aRoot = "ROOT";
        }

        private void root_toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            isRoot = false;
            aRoot = "";
        }
    }
}
