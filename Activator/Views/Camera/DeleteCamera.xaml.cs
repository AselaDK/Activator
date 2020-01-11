using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for DeleteCamera.xaml
    /// </summary>
    public partial class DeleteCamera : MetroWindow
    {
        string id;
        string videoStreamArn;
        string dataStreamName;
        string streamProcessorName;
        string eventSourceUUID;

        public DeleteCamera(string _id, string _videoStreamArn, string _dataStreamName, string _eventSourceUUID, string _streamProcessorName)
        {
            InitializeComponent();

            id = _id;
            videoStreamArn = _videoStreamArn;
            dataStreamName = _dataStreamName;
            eventSourceUUID = _eventSourceUUID;
            streamProcessorName = _streamProcessorName;

            Delete();
        }

        private async void Delete()
        {
            ProgressDialogController controller = await this.ShowProgressAsync("Please wait...", "");
            controller.SetIndeterminate();
            controller.SetCancelable(false);

            controller.SetMessage("Deleting event source mapping");
            await Task.Run(() => Models.Lambda.DeleteEventSourceMapping(eventSourceUUID));

            controller.SetMessage("Deleting data stream");
            await Task.Run(() => Models.DataStream.DeleteDataStream(dataStreamName));

            controller.SetMessage("Deleting video stream");
            await Task.Run(() => Models.VideoStream.DeleteVideoStream(videoStreamArn));

            controller.SetMessage("Deleting stream processor");
            await Task.Run(() => Models.StreamProcessorManager.DeleteStreamProcessor(streamProcessorName));

            controller.SetMessage("Deleting database record");
            await Task.Run(() => Models.Dynamodb.DeleteItem(id, Models.MyAWSConfigs.CamerasDBTableName));

            await controller.CloseAsync();

            await this.ShowMessageAsync("Deleted", "Camera deleted Successfully", MessageDialogStyle.Affirmative);
            this.Close();
        }
    }
}
