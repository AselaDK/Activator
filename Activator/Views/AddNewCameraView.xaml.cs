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
                string streamName = txtStreamName.Text;
                string location = txtLocation.Text;                
                string streamProcessorName = streamName + "_processor";
                string videoStreamArn = null;

                // create video stream
                videoStreamArn = Models.VideoStream.CreateVideoStream(streamName);

                if (string.IsNullOrEmpty(videoStreamArn))
                {
                    Mouse.OverrideCursor = null;
                    await this.ShowMessageAsync("Error creating video stream. Please try again later.", "", MessageDialogStyle.Affirmative);
                    return;
                }
                else if (videoStreamArn == "contain")
                {
                    Mouse.OverrideCursor = null;
                    txtStreamName.BorderBrush = Brushes.Red;
                    await this.ShowMessageAsync("Stream Name already exist", "", MessageDialogStyle.Affirmative);
                    return;
                }

                // create data stream

                // create stream processor
                bool success = Models.StreamManager.CreateStreamProcessor(streamProcessorName, videoStreamArn);

                if (!success)
                {
                    Mouse.OverrideCursor = null;
                    await this.ShowMessageAsync("Error creating stream processor. Please try again later.", "", MessageDialogStyle.Affirmative);
                    // delete created video stream
                    return;
                }

                // add new trigger to lambda function

                // create dynamodb record
                Document camera = new Document();
                camera["streamName"] = streamName;
                camera["location"] = location;
                Models.Dynamodb.PutItem(camera, Models.MyAWSConfigs.CamerasDBTableName);

                Mouse.OverrideCursor = null;
                await this.ShowMessageAsync("Success", "", MessageDialogStyle.Affirmative);
                this.Close();
            }
            else
            {
                Mouse.OverrideCursor = null;
                await this.ShowMessageAsync("Error", "Please check all fields", MessageDialogStyle.Affirmative);
            }
            Mouse.OverrideCursor = null;
        }
    }
}
