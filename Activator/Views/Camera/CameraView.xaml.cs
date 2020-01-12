using Amazon.DynamoDBv2.DocumentModel;
using MahApps.Metro.Controls.Dialogs;
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
            LoadCamerasData();

            CheckSelection();

            mainView = mv;
        }

        public void LoadCamerasData()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<Models.Camera> cameras = new List<Models.Camera>();

                cameras = Models.Camera.GetAllCamers();

                // TODO: Loading indicator

                dataGridCameras.ItemsSource = cameras;
                dataGridCameras.Items.Refresh();
            }
            finally
            {
                Mouse.OverrideCursor = null;
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
            LoadCamerasData();
        }

        private async void BtnStopSP_Click(object sender, RoutedEventArgs e)
        {
            int selectedCamera = dataGridCameras.SelectedIndex + 1;
            if (selectedCamera > 0)
            {
                string streamProcessorName = $"StreamProcessorCam{selectedCamera}";

                Mouse.OverrideCursor = Cursors.Wait;
                await Task.Run(() => Models.StreamProcessorManager.StopStreamProcessor(streamProcessorName));
                Mouse.OverrideCursor = null;
            }
        }

        private async void BtnStartSP_Click(object sender, RoutedEventArgs e)
        {
            int selectedCamera = dataGridCameras.SelectedIndex + 1;
            if (selectedCamera > 0)
            {
                string streamProcessorName = $"StreamProcessorCam{selectedCamera}";

                Mouse.OverrideCursor = Cursors.Wait;
                await Task.Run(() => Models.StreamProcessorManager.StartStreamProcessor(streamProcessorName));
                Mouse.OverrideCursor = null;
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
