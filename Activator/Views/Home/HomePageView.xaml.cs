using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        private static readonly double notifyInterval = 150;

        private MainView mv;
        private DetectedPerson dp;

        Dictionary<string, Models.Camera> cameras = new Dictionary<string, Models.Camera>();

        public HomePageView(MainView mv)
        {
            InitializeComponent();

            this.mv = mv;
            dp = new DetectedPerson();

            ReadHistoryStream().ConfigureAwait(false);
            ReadRefPersonStream().ConfigureAwait(false);
        }

        protected override void OnInitialized(EventArgs e)
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            notifyIcon.Click += notifyIcon_Click;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.BalloonTipClosed += (s, _e) => notifyIcon.Visible = false;

            base.OnInitialized(e);
        }

        void notifyIcon_Click(object sender, EventArgs e)
        {

        }

        void notifyIcon_DoubleClick(object sender, EventArgs e)
        {

        }

        public async Task GetAllCameras()
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                IEnumerable<Models.Camera> temp = await Task.Run(() => Models.Camera.GetAllCamers());

                cameras.Clear();

                foreach (Models.Camera camera in temp)
                {
                    cameras.Add(camera.id, camera);
                }
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private void dataGridDetectedPersons_Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            TextBlock id = dataGridDetectedPersons.Columns[1].GetCellContent(row) as TextBlock;
            TextBlock name = dataGridDetectedPersons.Columns[2].GetCellContent(row) as TextBlock;
            TextBlock description = dataGridDetectedPersons.Columns[5].GetCellContent(row) as TextBlock;
            ImageSource imageSource = (VisualTreeHelper.GetChild(dataGridDetectedPersons.Columns[0].GetCellContent(row), 0) as Image).Source;

            mv.MenuPage.Content = dp;
            dp.LoadPerson(id.Text, name.Text, description.Text, imageSource, mv, this, "home");
        }

        private async Task ReadRefPersonStream()
        {
            Dictionary<string, Models.RefPerson> refPersons = new Dictionary<string, Models.RefPerson>();
            refPersons.Clear();
            dataGridDetectedPersons.ItemsSource = refPersons.Values;
            dataGridDetectedPersons.Items.Refresh();

            string streamArn = Models.MyAWSConfigs.DynamodbRefPersonTableStreamArn;
            //int maxItemCount = 100;

            try
            {
                AmazonDynamoDBStreamsClient streamsClient;
                using (streamsClient = new AmazonDynamoDBStreamsClient(Models.MyAWSConfigs.DynamodbRegion))
                {
                    String lastEvaluatedShardId = null;

                    do
                    {
                        DescribeStreamRequest describeStreamRequest = new DescribeStreamRequest()
                        {
                            StreamArn = streamArn,
                            ExclusiveStartShardId = lastEvaluatedShardId,
                        };

                        DescribeStreamResponse describeStreamResponse = await streamsClient.DescribeStreamAsync(describeStreamRequest);

                        List<Shard> shards = describeStreamResponse.StreamDescription.Shards;

                        // Process each shard on this page

                        foreach (Shard shard in shards)
                        {
                            String shardId = shard.ShardId;

                            // Get an iterator for the current shard

                            GetShardIteratorRequest getShardIteratorRequest = new GetShardIteratorRequest()
                            {
                                StreamArn = streamArn,
                                ShardId = shardId,
                                ShardIteratorType = ShardIteratorType.LATEST,
                            };

                            GetShardIteratorResponse getShardIteratorResponse =
                                await streamsClient.GetShardIteratorAsync(getShardIteratorRequest);

                            String currentShardIter = getShardIteratorResponse.ShardIterator;

                            int processedRecordCount = 0;
                            //&& processedRecordCount < maxItemCount
                            while (currentShardIter != null)
                            {
                                // Use the shard iterator to read the stream records

                                GetRecordsRequest getRecordsRequest = new GetRecordsRequest()
                                {
                                    ShardIterator = currentShardIter
                                };

                                GetRecordsResponse getRecordsResponse = await streamsClient.GetRecordsAsync(getRecordsRequest);

                                List<Record> records = getRecordsResponse.Records;

                                foreach (Record record in records)
                                {
                                    foreach (KeyValuePair<string, AttributeValue> newImage in record.Dynamodb.NewImage)
                                    {
                                        string changedRefPersonId = record.Dynamodb.NewImage["id"].S;
                                        string changedRefPersonStatus = record.Dynamodb.NewImage["status"].N.ToString();
                                        string changedRefPersonName = record.Dynamodb.NewImage["name"].S;
                                        string changedRefPersonDescription = record.Dynamodb.NewImage["description"].S;
                                        string changedRefPersonCamera = record.Dynamodb.NewImage["camera"].S;

                                        Models.RefPerson refPerson = new Models.RefPerson();

                                        refPerson.id = changedRefPersonId;
                                        refPerson.name = changedRefPersonName;
                                        refPerson.status = (changedRefPersonStatus == "1") ? true : false;
                                        refPerson.description = changedRefPersonDescription;
                                        refPerson.camera = changedRefPersonCamera;
                                        refPerson.lastLocation = cameras[changedRefPersonCamera].location;

                                        string directoryPath = "Resources/Images/";

                                        if (!File.Exists(directoryPath + refPerson.id))
                                        {
                                            Models.S3Bucket.DownloadFile(refPerson.id, Models.MyAWSConfigs.RefImagesBucketName);
                                        }

                                        string exeDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

                                        Uri fileUri = new Uri(exeDirectory + directoryPath + refPerson.id);

                                        refPerson.image = new BitmapImage(fileUri);

                                        if (refPerson.status)
                                        {
                                            if (!refPersons.ContainsKey(refPerson.id))
                                                refPersons.Add(refPerson.id, refPerson);
                                        }
                                        else
                                        {
                                            if (refPersons.ContainsKey(refPerson.id))
                                                refPersons.Remove(refPerson.id);
                                        }

                                        dataGridDetectedPersons.ItemsSource = refPersons.Values;
                                        dataGridDetectedPersons.Items.Refresh();
                                    }
                                }
                                processedRecordCount += records.Count;
                                currentShardIter = getRecordsResponse.NextShardIterator;
                            }
                        }

                        // If LastEvaluatedShardId is set, then there is
                        // at least one more page of shard IDs to retrieve
                        lastEvaluatedShardId = describeStreamResponse.StreamDescription.LastEvaluatedShardId;

                    } while (lastEvaluatedShardId != null);
                }
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        private async Task ReadHistoryStream()
        {
            string streamArn = Models.MyAWSConfigs.DynamodbHistoryTableStreamArn;

            DateTime notifyTime = DateTime.Now;
            notifyTime = DateTime.Now.AddMilliseconds(notifyInterval);

            //int maxItemCount = 100;

            try
            {
                AmazonDynamoDBStreamsClient streamsClient;
                using (streamsClient = new AmazonDynamoDBStreamsClient(Models.MyAWSConfigs.DynamodbRegion))
                {
                    String lastEvaluatedShardId = null;

                    do
                    {
                        DescribeStreamRequest describeStreamRequest = new DescribeStreamRequest()
                        {
                            StreamArn = streamArn,
                            ExclusiveStartShardId = lastEvaluatedShardId,
                        };

                        DescribeStreamResponse describeStreamResponse = await streamsClient.DescribeStreamAsync(describeStreamRequest);

                        List<Shard> shards = describeStreamResponse.StreamDescription.Shards;

                        // Process each shard on this page

                        foreach (Shard shard in shards)
                        {
                            String shardId = shard.ShardId;

                            // Get an iterator for the current shard

                            GetShardIteratorRequest getShardIteratorRequest = new GetShardIteratorRequest()
                            {
                                StreamArn = streamArn,
                                ShardId = shardId,
                                ShardIteratorType = ShardIteratorType.LATEST,
                            };

                            GetShardIteratorResponse getShardIteratorResponse =
                                await streamsClient.GetShardIteratorAsync(getShardIteratorRequest);

                            String currentShardIter = getShardIteratorResponse.ShardIterator;

                            int processedRecordCount = 0;
                            //&& processedRecordCount < maxItemCount
                            while (currentShardIter != null)
                            {
                                // Use the shard iterator to read the stream records

                                GetRecordsRequest getRecordsRequest = new GetRecordsRequest()
                                {
                                    ShardIterator = currentShardIter
                                };

                                GetRecordsResponse getRecordsResponse = await streamsClient.GetRecordsAsync(getRecordsRequest);

                                List<Record> records = getRecordsResponse.Records;

                                foreach (Record record in records)
                                {
                                    foreach (KeyValuePair<string, AttributeValue> newImage in record.Dynamodb.NewImage)
                                    {
                                        if (DateTime.Now >= notifyTime)
                                        {
                                            string id = record.Dynamodb.NewImage["id"].S;
                                            string name = record.Dynamodb.NewImage["name"].S;
                                            string camId = record.Dynamodb.NewImage["cameraId"].S;
                                            string camLocation = cameras[camId].location;
                                            string notification = $"{name} has been detected by camera {camId} at {camLocation}";
                                            string trayMessage = $"{name} has been detected";

                                            AddNewNotification(notification, id).Wait();

                                            notifyTime = DateTime.Now.AddMilliseconds(notifyInterval);

                                            notifyIcon.Visible = true;
                                            notifyIcon.ShowBalloonTip(1000, "New Person Detected", trayMessage, System.Windows.Forms.ToolTipIcon.Info);
                                        }
                                    }
                                }
                                processedRecordCount += records.Count;
                                currentShardIter = getRecordsResponse.NextShardIterator;
                            }
                        }

                        // If LastEvaluatedShardId is set, then there is
                        // at least one more page of shard IDs to retrieve
                        lastEvaluatedShardId = describeStreamResponse.StreamDescription.LastEvaluatedShardId;

                    } while (lastEvaluatedShardId != null);
                }
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        private async Task AddNewNotification(string message, string id)
        {
            string directoryPath = "Resources/Images/";

            BitmapImage image = new BitmapImage();

            if (!File.Exists(directoryPath + id))
            {
                await Task.Run(() => Models.S3Bucket.DownloadFile(id, Models.MyAWSConfigs.RefImagesBucketName));
            }

            string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

            Uri fileUri = new Uri(exeDirectory + directoryPath + id);

            image = new BitmapImage(fileUri);
            message = DateTime.Now.ToShortTimeString() + " " + message;

            var myChip = new MaterialDesignThemes.Wpf.Chip()
            {
                Content = message,
                IsDeletable = false,
                Icon = new Image() { Source = image, Stretch = Stretch.Uniform },
            };

            Thickness margin = myChip.Margin;
            margin.Left = 10;
            margin.Top = 10;
            margin.Bottom = 10;
            myChip.Margin = margin;

            this.notificationListView.Items.Add(myChip);
        }
    }

    class Notification
    {
        public string Message { get; set; }
    }
}
