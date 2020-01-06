using System;
using System.Collections.Generic;

using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace Activator.Models
{
    class StreamProcessorManager
    {
        private static String roleArn = MyAWSConfigs.RoleArn;
        private static String collectionId = MyAWSConfigs.FaceCollectionID;

        private static float matchThreshold = 90f;

        private static AmazonRekognitionClient rekognitionClient = null;

        public static bool CreateStreamProcessor(String streamProcessorName, String VideoStreamArn, String DataStreamArn)
        {
            // Setup input parameters
            KinesisVideoStream kinesisVideoStream = new KinesisVideoStream()
            {
                Arn = VideoStreamArn,
            };

            StreamProcessorInput streamProcessorInput = new StreamProcessorInput()
            {
                KinesisVideoStream = kinesisVideoStream,
            };

            KinesisDataStream kinesisDataStream = new KinesisDataStream()
            {
                Arn = DataStreamArn,
            };

            StreamProcessorOutput streamProcessorOutput = new StreamProcessorOutput()
            {
                KinesisDataStream = kinesisDataStream,
            };

            FaceSearchSettings faceSearchSettings = new FaceSearchSettings()
            {
                CollectionId = collectionId,
                FaceMatchThreshold = matchThreshold,
            };

            StreamProcessorSettings streamProcessorSettings = new StreamProcessorSettings()
            {
                FaceSearch = faceSearchSettings,
            };

            using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
            {
                // Create the stream processor
                CreateStreamProcessorResponse createStreamProcessorResponse = rekognitionClient.CreateStreamProcessor(
                        new CreateStreamProcessorRequest()
                        {
                            Input = streamProcessorInput,
                            Output = streamProcessorOutput,
                            Settings = streamProcessorSettings,
                            RoleArn = roleArn,
                            Name = streamProcessorName,
                        });

                // Display result
                Console.WriteLine("Stream Processor " + streamProcessorName + " created.");
                Console.WriteLine("StreamProcessorArn - " + createStreamProcessorResponse.StreamProcessorArn);

                return createStreamProcessorResponse.HttpStatusCode == System.Net.HttpStatusCode.OK ? true : false;
            }
        }

        public static bool StartStreamProcessor(String streamProcessorName)
        {
            using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
            {
                StartStreamProcessorResponse startStreamProcessorResponse =
                    rekognitionClient.StartStreamProcessor(new StartStreamProcessorRequest()
                    {
                        Name = streamProcessorName,
                    });
                Console.WriteLine("Stream Processor " + streamProcessorName + " started.");

                return startStreamProcessorResponse.HttpStatusCode == System.Net.HttpStatusCode.OK ? true : false;
            }
        }

        public static bool StopStreamProcessor(String streamProcessorName)
        {
            using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
            {
                StopStreamProcessorResponse stopStreamProcessorResponse =
                    rekognitionClient.StopStreamProcessor(new StopStreamProcessorRequest()
                    {
                        Name = streamProcessorName
                    });
                Console.WriteLine("Stream Processor " + streamProcessorName + " stopped.");

                return stopStreamProcessorResponse.HttpStatusCode == System.Net.HttpStatusCode.OK ? true : false;
            }
        }

        public static bool DeleteStreamProcessor(String streamProcessorName)
        {
            using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
            {
                DeleteStreamProcessorResponse deleteStreamProcessorResponse = rekognitionClient
                    .DeleteStreamProcessor(new DeleteStreamProcessorRequest()
                    {
                        Name = streamProcessorName
                    });
                Console.WriteLine("Stream Processor " + streamProcessorName + " deleted.");

                return deleteStreamProcessorResponse.HttpStatusCode == System.Net.HttpStatusCode.OK ? true : false;
            }
        }

        public static DescribeStreamProcessorResponse DescribeStreamProcessor(String streamProcessorName)
        {
            using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
            {
                DescribeStreamProcessorResponse describeStreamProcessorResponse = rekognitionClient
                    .DescribeStreamProcessor(new DescribeStreamProcessorRequest()
                    {
                        Name = streamProcessorName,
                    });

                return describeStreamProcessorResponse;
            }
        }

        public static List<string> ListStreamProcessors()
        {
            using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
            {
                List<string> streamProcessors = null;

                ListStreamProcessorsResponse listStreamProcessorsResponse =
                        rekognitionClient.ListStreamProcessors(new ListStreamProcessorsRequest()
                        {
                            MaxResults = 100,
                        });

                if (listStreamProcessorsResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    streamProcessors = new List<string>();

                    // List all stream processors (and state) returned from Rekognition
                    foreach (StreamProcessor streamProcessor in listStreamProcessorsResponse.StreamProcessors)
                    {
                        streamProcessors.Add(streamProcessor.Name);
                        Console.WriteLine("StreamProcessor name - " + streamProcessor.Name);
                        Console.WriteLine("Status - " + streamProcessor.Status + "\n");
                    }
                }

                return streamProcessors;
            }
        }
    }
}
