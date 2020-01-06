using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Kinesis;
using Amazon.Kinesis.Model;

namespace Activator.Models
{
    class DataStream
    {
        public static string CreateDataStream(string streamName)
        {
            string streamArn = "";

            try
            {
                List<string> streamlList = GetDataStreamList();

                if (streamlList != null)
                {
                    if (streamlList.FindAll(dataStreamName => dataStreamName == streamName).Count > 0)
                        streamArn = "contain";
                    else
                    {
                        AmazonKinesisClient kinesisClient;

                        using (kinesisClient = new AmazonKinesisClient(Models.MyAWSConfigs.KinesisRegion))
                        {
                            CreateStreamRequest createStreamRequest = new CreateStreamRequest()
                            {
                                StreamName = streamName,                               
                                ShardCount = 1,
                            };

                            CreateStreamResponse createStreamResponse = kinesisClient.CreateStream(createStreamRequest);

                            if (createStreamResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                            {
                                StreamDescriptionSummary streamSummary = DescribeDataStream(streamName);
                                if (streamSummary != null)
                                {
                                    streamArn = streamSummary.StreamARN;

                                    while (streamSummary.StreamStatus != StreamStatus.ACTIVE || 
                                        streamSummary.StreamStatus == StreamStatus.CREATING)
                                    {
                                        Thread.Sleep(1 * 1000);
                                        streamSummary = DescribeDataStream(streamName);
                                        if (streamSummary == null)
                                        {
                                            streamArn = "";
                                            break;
                                        }                                                                                    
                                    }                                    
                                }                                    
                            }
                            else
                                Console.WriteLine("Error creating kinesis data stream");
                        }
                    }
                }
            }
            catch (AmazonKinesisException e)
            {
                Console.WriteLine("AmazonKinesisException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return streamArn;
        }

        public static List<string> GetDataStreamList()
        {
            List<string> streamList = null;

            try
            {
                AmazonKinesisClient kinesisClient;

                using (kinesisClient = new AmazonKinesisClient(Models.MyAWSConfigs.KinesisRegion))
                {                   
                    ListStreamsRequest listStreamsRequest = new ListStreamsRequest();
                    ListStreamsResponse listStreamsResponse = kinesisClient.ListStreams(listStreamsRequest);

                    if (listStreamsResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        streamList = listStreamsResponse.StreamNames;
                    else
                        Console.WriteLine("Error listing kinesis data streams");
                }
            }
            catch (AmazonKinesisException e)
            {
                Console.WriteLine("AmazonKinesisException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return streamList;
        }

        public static StreamDescriptionSummary DescribeDataStream(string streamName)
        {
            StreamDescriptionSummary summary = null;

            try
            {
                AmazonKinesisClient kinesisClient;

                using (kinesisClient = new AmazonKinesisClient(Models.MyAWSConfigs.KinesisRegion))
                {
                    DescribeStreamSummaryRequest streamSummaryRequest = new DescribeStreamSummaryRequest {
                        StreamName = streamName
                    };

                    DescribeStreamSummaryResponse streamSummaryResponse = kinesisClient.DescribeStreamSummary(streamSummaryRequest);

                    if (streamSummaryResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        summary = streamSummaryResponse.StreamDescriptionSummary;
                    else
                        Console.WriteLine("Error Describe kinesis data stream");
                }
            }
            catch (AmazonKinesisException e)
            {
                Console.WriteLine("AmazonKinesisException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return summary;
        }
    }
}
