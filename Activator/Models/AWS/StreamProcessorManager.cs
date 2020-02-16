using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;

namespace Activator.Models
{
    class StreamProcessorManager
    {
        private static String roleArn = MyAWSConfigs.RoleArn;
        private static String collectionId = MyAWSConfigs.FaceCollectionID;

        private static float matchThreshold = 90f;

        public static bool CreateStreamProcessor(String streamProcessorName, String VideoStreamArn, String DataStreamArn)
        {
            bool isSuccess = false;

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

            try
            {
                AmazonRekognitionClient rekognitionClient;
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

                    isSuccess = createStreamProcessorResponse.HttpStatusCode == System.Net.HttpStatusCode.OK ? true : false;
                }
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);

            }

            return isSuccess;
        }

        public static bool StartStreamProcessor(String streamProcessorName)
        {
            bool isSuccess = false;

            try
            {
                AmazonRekognitionClient rekognitionClient;
                using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
                {
                    var describeSP = DescribeStreamProcessor(streamProcessorName);
                    if (describeSP.Status == StreamProcessorStatus.STOPPED)
                    {
                        StartStreamProcessorResponse startStreamProcessorResponse =
                        rekognitionClient.StartStreamProcessor(new StartStreamProcessorRequest()
                        {
                            Name = streamProcessorName,
                        });
                        Console.WriteLine("Stream Processor " + streamProcessorName + " started.");

                        isSuccess = startStreamProcessorResponse.HttpStatusCode == System.Net.HttpStatusCode.OK ? true : false;
                    }
                }
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);

            }

            return isSuccess;
        }

        public static bool StopStreamProcessor(String streamProcessorName)
        {
            bool isSuccess = false;

            try
            {
                AmazonRekognitionClient rekognitionClient;
                using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
                {
                    var describeSP = DescribeStreamProcessor(streamProcessorName);
                    if (describeSP.Status == StreamProcessorStatus.RUNNING)
                    {
                        StopStreamProcessorResponse stopStreamProcessorResponse =
                        rekognitionClient.StopStreamProcessor(new StopStreamProcessorRequest()
                        {
                            Name = streamProcessorName
                        });
                        Console.WriteLine("Stream Processor " + streamProcessorName + " stopped.");

                        isSuccess = stopStreamProcessorResponse.HttpStatusCode == System.Net.HttpStatusCode.OK ? true : false;
                    }
                }
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);

            }

            return isSuccess;
        }

        public static bool DeleteStreamProcessor(String streamProcessorName)
        {
            bool isSuccess = false;

            try
            {
                AmazonRekognitionClient rekognitionClient;
                using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
                {
                    DeleteStreamProcessorResponse deleteStreamProcessorResponse = rekognitionClient
                        .DeleteStreamProcessor(new DeleteStreamProcessorRequest()
                        {
                            Name = streamProcessorName
                        });
                    Console.WriteLine("Stream Processor " + streamProcessorName + " deleted.");

                    isSuccess = deleteStreamProcessorResponse.HttpStatusCode == System.Net.HttpStatusCode.OK ? true : false;
                }
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);

            }

            return isSuccess;
        }

        public static DescribeStreamProcessorResponse DescribeStreamProcessor(String streamProcessorName)
        {
            DescribeStreamProcessorResponse describeStreamProcessorResponse = null;

            try
            {
                AmazonRekognitionClient rekognitionClient;
                using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
                {
                    describeStreamProcessorResponse = rekognitionClient
                        .DescribeStreamProcessor(new DescribeStreamProcessorRequest()
                        {
                            Name = streamProcessorName,
                        });
                }
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);

            }

            return describeStreamProcessorResponse;
        }

        public static List<string> ListStreamProcessors()
        {
            List<string> streamProcessors = null;

            try
            {
                AmazonRekognitionClient rekognitionClient;
                using (rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.KinesisRegion))
                {


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
                }
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);

            }

            return streamProcessors;
        }
    }
}
