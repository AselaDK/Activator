using Amazon.DynamoDBv2.DocumentModel;
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

            CheckSelection().ConfigureAwait(false);

            mainView = mv;
        }

        public async Task LoadCamerasData()
        {
            progressBar.Visibility = Visibility.Visible;
            BtnRefresh.IsEnabled = false;
            try
            {                
                IEnumerable<Models.Camera> cameras = await Task.Run(() => Models.Camera.GetAllCamers());

                dataGridCameras.ItemsSource = cameras;
                dataGridCameras.Items.Refresh();
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
                BtnRefresh.IsEnabled = true;
            }
        }

        private async Task CheckSelection()
        {
            int selectCount = dataGridCameras.SelectedItems.Count;

            if (selectCount <= 0 || selectCount > 1)
            {
                BtnStartSP.IsEnabled = false;
                BtnStopSP.IsEnabled = false;
                BtnDeleteCamera.IsEnabled = false;
            }
            else
            {
                string selectedId = (dataGridCameras.SelectedItem as Models.Camera).id;

                string streamProcessorName = $"StreamProcessorCam{selectedId}";

                progressBar.Visibility = Visibility.Visible;

                var response = await Task.Run(() => Models.StreamProcessorManager.DescribeStreamProcessor(streamProcessorName));

                if (response.Status == Amazon.Rekognition.StreamProcessorStatus.RUNNING)
                {
                    BtnStopSP.IsEnabled = true;
                    BtnStartSP.IsEnabled = false;                    
                }
                else if (response.Status == Amazon.Rekognition.StreamProcessorStatus.STOPPED)
                {
                    BtnStopSP.IsEnabled = false;
                    BtnStartSP.IsEnabled = true;
                }

                progressBar.Visibility = Visibility.Hidden;

                BtnDeleteCamera.IsEnabled = true;
            }
        }

        private void BtnAddNewCamera_Click(object sender, RoutedEventArgs e)
        {
            AddNewCameraView addNewCameraView = new AddNewCameraView(this);
            addNewCameraView.ShowDialog();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCamerasData().ConfigureAwait(false);
        }

        private async void BtnStopSP_Click(object sender, RoutedEventArgs e)
        {
            int selectedCount = dataGridCameras.SelectedItems.Count;
            if (selectedCount > 0)
            {
                string selectedId = (dataGridCameras.SelectedItem as Models.Camera).id;

                string streamProcessorName = $"StreamProcessorCam{selectedId}";

                progressBar.Visibility = Visibility.Visible;

                await Task.Run(() => Models.StreamProcessorManager.StopStreamProcessor(streamProcessorName));

                BtnStopSP.IsEnabled = false;
                BtnStartSP.IsEnabled = true;
                //await CheckProcessorStatus(streamProcessorName).ConfigureAwait(false);

                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private async void BtnStartSP_Click(object sender, RoutedEventArgs e)
        {
            int selectedCount = dataGridCameras.SelectedItems.Count;
            if (selectedCount > 0)
            {
                string selectedId = (dataGridCameras.SelectedItem as Models.Camera).id;

                string streamProcessorName = $"StreamProcessorCam{selectedId}";

                progressBar.Visibility = Visibility.Visible;

                await Task.Run(() => Models.StreamProcessorManager.StartStreamProcessor(streamProcessorName));

                BtnStopSP.IsEnabled = true;
                BtnStartSP.IsEnabled = false;
                //await CheckProcessorStatus(streamProcessorName).ConfigureAwait(false);

                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private void BtnDeleteCamera_Click(object sender, RoutedEventArgs e)
        {
            int selectedCount = dataGridCameras.SelectedItems.Count;
            if (selectedCount > 0)
            {
                string selectedId = (dataGridCameras.SelectedItem as Models.Camera).id;

                Document camera = Models.Dynamodb.GetItem(selectedId.ToString(), Models.MyAWSConfigs.CamerasDBTableName);

                string videoStreamArn = camera["videoStreamArn"];
                string eventSourceUUID = camera["eventSourceUUID"];
                string dataStreamName = $"AmazonRekognitionDataStreamCam{selectedId}";
                string streamProcessorName = $"StreamProcessorCam{selectedId}";

                mainView.DeleteCamera(selectedId.ToString(), videoStreamArn, dataStreamName, eventSourceUUID, streamProcessorName, this);               
            }
        }  
        
        private async Task CheckProcessorStatus(string streamProcessorName)
        {
            var response = await Task.Run(() => Models.StreamProcessorManager.DescribeStreamProcessor(streamProcessorName));

            if (response.Status == Amazon.Rekognition.StreamProcessorStatus.RUNNING)
            {
                BtnStopSP.IsEnabled = true;
                BtnStartSP.IsEnabled = false;
            }
            else if (response.Status == Amazon.Rekognition.StreamProcessorStatus.STOPPED)
            {
                BtnStopSP.IsEnabled = false;
                BtnStartSP.IsEnabled = true;
            }
        }

        private void DataGridCameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckSelection().ConfigureAwait(false);
        }
    }
}
