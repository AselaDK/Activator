using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;


namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        public HomePageView()
        {
            InitializeComponent();
            ReadStream();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                StreamProcessor sp = Models.Starter.DescribeStreamProcessor();

                if (sp.Status == StreamProcessorStatus.STOPPED)
                {
                    Models.Starter.StartStreamProcessor();
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                StreamProcessor sp = Models.Starter.DescribeStreamProcessor();

                if (sp.Status == StreamProcessorStatus.RUNNING)
                {
                    Models.Starter.StopStreamProcessor();
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                StreamProcessor sp = Models.Starter.DescribeStreamProcessor();

                Console.WriteLine(sp.Name);
                Console.WriteLine(sp.Status);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void CheckAndCreateStreamProcessor()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!Models.Starter.ListStreamProcessors().Contains(Models.MyAWSConfigs.streamProcessorName))
                {
                    Models.Starter.CreateStreamProcessor();
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            CheckAndCreateStreamProcessor();
        }

        private void BtnList_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<string> temp = Models.Starter.ListStreamProcessors();
                foreach (string item in temp)
                {
                    Console.WriteLine(item);
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Models.Starter.DeleteStreamProcessor();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private async Task ReadStream()
        {
            List<Models.RefPerson> refPersons = new List<Models.RefPerson>();
            dataGridDetectedPersons.ItemsSource = refPersons;
            dataGridDetectedPersons.Items.Refresh();

            string streamArn = "arn:aws:dynamodb:ap-southeast-2:358403828169:table/ref_persons/stream/2019-11-18T05:31:40.045";
            int maxItemCount = 100;

            try
            {
                AmazonDynamoDBStreamsClient streamsClient;
                using (streamsClient = new AmazonDynamoDBStreamsClient(Models.MyAWSConfigs.dynamodbRegion))
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
                            while (currentShardIter != null && processedRecordCount < maxItemCount)
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
                                        Console.WriteLine($"{changedRefPersonId}:{changedRefPersonStatus}:{changedRefPersonName}:{changedRefPersonDescription}");

                                        Models.RefPerson refPerson = new Models.RefPerson();

                                        refPerson.id = changedRefPersonId;
                                        refPerson.name = changedRefPersonName;
                                        refPerson.status = (changedRefPersonStatus == "1")?true:false;
                                        refPerson.description = changedRefPersonDescription;

                                        string directoryPath = "Resources/Images/";

                                        if (!File.Exists(directoryPath + refPerson.id))
                                        {
                                            Models.S3Bucket.DownloadFile(refPerson.id,Models.MyAWSConfigs.refImagesBucketName);
                                        }

                                        string exeDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
                                        

                                        Uri fileUri = new Uri(exeDirectory + directoryPath + refPerson.id);

                                        refPerson.image = new BitmapImage(fileUri);

                                        if (refPerson.status)
                                        {                                            
                                            if (refPersons.FindAll(p => p.id == refPerson.id).Count == 0)
                                            {
                                                refPersons.Add(refPerson);
                                            }
                                        }
                                        else
                                        {
                                            refPersons.RemoveAll(p => p.id == refPerson.id);
                                        }

                                        dataGridDetectedPersons.ItemsSource = refPersons;
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
    }
}
