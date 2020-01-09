using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for AddNewCameraView.xaml
    /// </summary>
    public partial class AddNewCameraView : MetroWindow
    {
        public AddNewCameraView()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLocation.Text) && !string.IsNullOrEmpty(txtDescription.Text))
            {
                ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "");
                controller.SetIndeterminate();
                controller.SetCancelable(false);

                controller.SetMessage("Getting current cameras information");

                long currentCamCount = await Task.Run(() =>
                                        Models.Dynamodb.GetItemCount(Models.MyAWSConfigs.CamerasDBTableName));
                long nextCamID = currentCamCount + 1;

                // Maximum 10 cameras currently can have
                if (nextCamID > 10)
                {
                    await controller.CloseAsync();
                    await this.ShowMessageAsync("You have reached the camera limit. " +
                                    "Please contact Administrator.", "", MessageDialogStyle.Affirmative);
                    return;
                }

                // Setup
                string description = txtDescription.Text;
                string location = txtLocation.Text;

                string videoStreamName = $"VideoStreamCam{nextCamID}";
                string dataStreamName = $"AmazonRekognitionDataStreamCam{nextCamID}";
                string streamProcessorName = $"StreamProcessorCam{nextCamID}";

                string videoStreamArn = "";
                string dataStreamArn = "";
                string eventSourceUUID = "";

                // Create video stream
                controller.SetMessage("Creating video stream");
                videoStreamArn = await Task.Run(() =>
                                                    Models.VideoStream.CreateVideoStream(videoStreamName));
                if (videoStreamArn == "contain" || string.IsNullOrEmpty(videoStreamArn))
                {
                    await controller.CloseAsync();
                    await this.ShowMessageAsync("Error adding new camera. Please contact Administrator.",
                                                "video stream error", MessageDialogStyle.Affirmative);
                    return;
                }

                // Create data stream
                controller.SetMessage("Creating data stream");
                dataStreamArn = await Task.Run(() =>
                                                    Models.DataStream.CreateDataStream(dataStreamName));
                if (dataStreamArn == "contain" || string.IsNullOrEmpty(dataStreamArn))
                {
                    await controller.CloseAsync();
                    await this.ShowMessageAsync("Error adding new camera. Please contact Administrator.",
                                "data stream error", MessageDialogStyle.Affirmative);

                    controller = await this.ShowProgressAsync("Reverting changes...", "");
                    controller.SetIndeterminate();
                    controller.SetCancelable(false);

                    controller.SetMessage("Deleting video stream");
                    await Task.Run(() => Models.VideoStream.DeleteVideoStream(videoStreamArn));

                    await controller.CloseAsync();
                    return;
                }

                // Add kinesis trigger to lambda
                controller.SetMessage("Creating lambda event source");
                eventSourceUUID = await Task.Run(() =>
                                                Models.Lambda.CreateEventSourceMapping(dataStreamArn));
                if (string.IsNullOrEmpty(eventSourceUUID))
                {
                    await controller.CloseAsync();
                    await this.ShowMessageAsync("Error adding new camera. Please contact Administrator.",
                                        "event source mapping error", MessageDialogStyle.Affirmative);

                    controller = await this.ShowProgressAsync("Reverting changes...", "");
                    controller.SetIndeterminate();
                    controller.SetCancelable(false);

                    controller.SetMessage("Deleting video stream");
                    await Task.Run(() => Models.VideoStream.DeleteVideoStream(videoStreamArn));

                    controller.SetMessage("Deleting data stream");
                    await Task.Run(() => Models.DataStream.DeleteDataStream(dataStreamName));

                    await controller.CloseAsync();
                    return;
                }

                // Create stream processor
                controller.SetMessage("Creating stream processor");
                bool success = await Task.Run(() =>
                                                        Models.StreamProcessorManager.CreateStreamProcessor
                                                        (
                                                            streamProcessorName,
                                                            videoStreamArn,
                                                            dataStreamArn
                                                        ));
                if (!success)
                {
                    await controller.CloseAsync();
                    await this.ShowMessageAsync("Error adding new camera. Please contact Administrator.", "stream processor error", MessageDialogStyle.Affirmative);

                    controller = await this.ShowProgressAsync("Reverting changes...", "");
                    controller.SetIndeterminate();
                    controller.SetCancelable(false);

                    controller.SetMessage("Deleting video stream");
                    await Task.Run(() => Models.VideoStream.DeleteVideoStream(videoStreamArn));

                    controller.SetMessage("Deleting data stream");
                    await Task.Run(() => Models.DataStream.DeleteDataStream(dataStreamName));

                    controller.SetMessage("Deleting event source mapping");
                    await Task.Run(() => Models.Lambda.DeleteEventSourceMapping(eventSourceUUID));

                    await controller.CloseAsync();
                    return;
                }

                // Create dynamodb record
                controller.SetMessage("Adding new record to the database");

                Document camera = new Document();
                camera["id"] = nextCamID.ToString();
                camera["description"] = description;
                camera["location"] = location;
                await Task.Run(() => Models.Dynamodb.PutItem(camera, Models.MyAWSConfigs.CamerasDBTableName));

                // Succeess message
                await controller.CloseAsync();
                await this.ShowMessageAsync("Success", "New Camera Added Successfully", MessageDialogStyle.Affirmative);
                this.Close();
            }
            else
            {
                await this.ShowMessageAsync("Error", "Please check all the fields", MessageDialogStyle.Affirmative);
            }
        }
    }
}
