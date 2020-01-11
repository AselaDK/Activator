using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Windows.Input;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        Dictionary<string, Models.Camera> cameras = new Dictionary<string, Models.Camera>();

        public HomePageView()
        {
            InitializeComponent();
            GetAllCameras();
            ReadRefPersonStream();            
        }
        
        private void GetAllCameras()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<Models.Camera> temp = Models.Camera.GetAllCamers();
                foreach (Models.Camera camera in temp)
                {
                    cameras.Add(camera.id, camera);
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
            
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
                                        refPerson.status = (changedRefPersonStatus == "1")?true:false;
                                        refPerson.description = changedRefPersonDescription;
                                        refPerson.camera = changedRefPersonCamera;

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
                                            refPersons.Add(refPerson.id, refPerson);
                                            string camLocation = cameras[refPerson.camera].location;
                                            string message = $"{refPerson.name} has been detected by camera {refPerson.camera} at {camLocation}";
                                            AddNewNotification(message);
                                        }
                                        else
                                        {
                                            refPersons.Remove(refPerson.id);
                                            string camLocation = cameras[refPerson.camera].location;
                                            string message = $"{refPerson.name} has been disappeared by camera {refPerson.camera} at {camLocation}";
                                            AddNewNotification(message);
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

        private async Task ReadNotificationStream()
        {   
            string streamArn = Models.MyAWSConfigs.DynamodbNotificationTableStreamArn;
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
                                        string message = record.Dynamodb.NewImage["message"].S;

                                        AddNewNotification(message);
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

        private void AddNewNotification(string message)
        {
            this.notificationListView.Items.Add(new Notification { Message = message });
        }
    }

    class Notification
    {
        public string Message { get; set; }
    }
}
