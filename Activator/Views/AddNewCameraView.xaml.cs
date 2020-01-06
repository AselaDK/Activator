using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

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
            Mouse.OverrideCursor = Cursors.Wait;
            if (!string.IsNullOrEmpty(txtLocation.Text) && !string.IsNullOrEmpty(txtStreamName.Text))
            {
                long nextCamID = Models.Dynamodb.GetItemCount(Models.MyAWSConfigs.CamerasDBTableName) + 1;

                // Maximum 10 cameras currently can have
                if (nextCamID > 10)
                {
                    Mouse.OverrideCursor = null;
                    await this.ShowMessageAsync("You reach the camera limit. Please contact Administrator.", "", MessageDialogStyle.Affirmative);
                    return;
                }

                string description = txtStreamName.Text;
                string location = txtLocation.Text;

                string videoStreamName = $"VideoStreamCam{nextCamID}";
                string dataStreamName = $"AmazonRekognitionDataStreamCam{nextCamID}";
                string streamProcessorName = $"StreamProcessorCam{nextCamID}";

                string videoStreamArn = "";
                string dataStreamArn = "";

                // Create video stream
                videoStreamArn = Models.VideoStream.CreateVideoStream(videoStreamName);
                if (videoStreamArn == "contain" || string.IsNullOrEmpty(videoStreamArn))
                {
                    Mouse.OverrideCursor = null;
                    await this.ShowMessageAsync("Error adding new camera. Please contact Administrator.", "video stream error", MessageDialogStyle.Affirmative);
                    return;
                }

                // Create data stream
                dataStreamArn = Models.DataStream.CreateDataStream(dataStreamName);
                if (dataStreamArn == "contain" || string.IsNullOrEmpty(dataStreamArn))
                {
                    Mouse.OverrideCursor = null;
                    // TODO: Delete added video stream
                    await this.ShowMessageAsync("Error adding new camera. Please contact Administrator.", "data stream error", MessageDialogStyle.Affirmative);
                    return;
                }

                // Add kinesis trigger to lambda
                bool success = Models.Lambda.CreateEventSourceMapping(dataStreamArn);

                if (!success)
                {
                    Mouse.OverrideCursor = null;
                    // TODO: Delete added video stream & Data stream
                    await this.ShowMessageAsync("Error adding new camera. Please contact Administrator.", "event source mapping error", MessageDialogStyle.Affirmative);
                    return;
                }

                // Create stream processor
                success = Models.StreamProcessorManager.CreateStreamProcessor(streamProcessorName, videoStreamArn, dataStreamArn);

                if (!success)
                {
                    Mouse.OverrideCursor = null;
                    // TODO: Delete added video stream, Data stream, & remove kinesis trigger
                    await this.ShowMessageAsync("Error adding new camera. Please contact Administrator.", "stream processor error", MessageDialogStyle.Affirmative);
                    return;
                }

                // Create dynamodb record
                Document camera = new Document();
                camera["id"] = nextCamID.ToString();
                camera["description"] = description;
                camera["location"] = location;
                Models.Dynamodb.PutItem(camera, Models.MyAWSConfigs.CamerasDBTableName);

                // Succeess message
                Mouse.OverrideCursor = null;
                await this.ShowMessageAsync("Success", "", MessageDialogStyle.Affirmative);
                this.Close();
            }
            else
            {
                Mouse.OverrideCursor = null;
                await this.ShowMessageAsync("Error", "Please check all the fields", MessageDialogStyle.Affirmative);
            }
            Mouse.OverrideCursor = null;
        }
    }
}
