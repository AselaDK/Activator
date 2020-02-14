﻿using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        private MainView mainView = null;

        public CameraView(MainView mv)
        {
            InitializeComponent();
            LoadCamerasData().ConfigureAwait(false);

            CheckSelection();

            mainView = mv;
        }

        public async Task LoadCamerasData()
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {                
                IEnumerable<Models.Camera> cameras = await Task.Run(() => Models.Camera.GetAllCamers());

                dataGridCameras.ItemsSource = cameras;
                dataGridCameras.Items.Refresh();
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private void CheckSelection()
        {
            int selectedCamera = dataGridCameras.SelectedIndex;
            if (selectedCamera == -1)
            {
                BtnStartSP.IsEnabled = false;
                BtnStopSP.IsEnabled = false;
                BtnDeleteCamera.IsEnabled = false;
            }
            else
            {
                BtnStartSP.IsEnabled = true;
                BtnStopSP.IsEnabled = true;
                BtnDeleteCamera.IsEnabled = true;
            }
        }

        private void BtnAddNewCamera_Click(object sender, RoutedEventArgs e)
        {
            AddNewCameraView addNewCameraView = new AddNewCameraView();
            addNewCameraView.Show();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCamerasData().ConfigureAwait(false);
        }

        private async void BtnStopSP_Click(object sender, RoutedEventArgs e)
        {
            int selectedCamera = dataGridCameras.SelectedIndex + 1;
            if (selectedCamera > 0)
            {
                string streamProcessorName = $"StreamProcessorCam{selectedCamera}";

                progressBar.Visibility = Visibility.Visible;
                await Task.Run(() => Models.StreamProcessorManager.StopStreamProcessor(streamProcessorName));
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private async void BtnStartSP_Click(object sender, RoutedEventArgs e)
        {
            int selectedCamera = dataGridCameras.SelectedIndex + 1;
            if (selectedCamera > 0)
            {
                string streamProcessorName = $"StreamProcessorCam{selectedCamera}";

                progressBar.Visibility = Visibility.Visible;
                await Task.Run(() => Models.StreamProcessorManager.StartStreamProcessor(streamProcessorName));
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private void BtnDeleteCamera_Click(object sender, RoutedEventArgs e)
        {
            int selectedCameraId = dataGridCameras.SelectedIndex + 1;
            if (selectedCameraId > 0)
            {
                Document camera = Models.Dynamodb.GetItem(selectedCameraId.ToString(), Models.MyAWSConfigs.CamerasDBTableName);

                string videoStreamArn = camera["videoStreamArn"];
                string eventSourceUUID = camera["eventSourceUUID"];
                string dataStreamName = $"AmazonRekognitionDataStreamCam{selectedCameraId}";
                string streamProcessorName = $"StreamProcessorCam{selectedCameraId}";

                mainView.DeleteCamera(selectedCameraId.ToString(), videoStreamArn, dataStreamName, eventSourceUUID, streamProcessorName, this);               
            }
        }        

        private void DataGridCameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckSelection();
        }
    }
}
