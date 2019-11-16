using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activator.Models
{
    class StreamManager
    {
        private String streamProcessorName;
        private String kinesisVideoStreamArn;
        private String kinesisDataStreamArn;
        private String roleArn;
        private String collectionId;
        private float matchThreshold;

        private AmazonRekognitionClient rekognitionClient;

        public StreamManager(
            String spName,
            String kvStreamArn,
            String kdStreamArn,
            String iamRoleArn,
            String collId,
            float threshold)
        {
            streamProcessorName = spName;
            kinesisVideoStreamArn = kvStreamArn;
            kinesisDataStreamArn = kdStreamArn;
            roleArn = iamRoleArn;
            collectionId = collId;
            matchThreshold = threshold;
            rekognitionClient = new AmazonRekognitionClient(MyAWSConfigs.faceCollectionRegion);
        }

        public string CreateStreamProcessor()
        {
            //Setup input parameters
            KinesisVideoStream kinesisVideoStream = new KinesisVideoStream()
            {
                Arn = kinesisVideoStreamArn,
            };

            StreamProcessorInput streamProcessorInput = new StreamProcessorInput()
            {
                KinesisVideoStream = kinesisVideoStream,
            };

            KinesisDataStream kinesisDataStream = new KinesisDataStream()
            {
                Arn = kinesisDataStreamArn,
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

            //Create the stream processor
            CreateStreamProcessorResponse createStreamProcessorResponse = rekognitionClient.CreateStreamProcessor(
                    new CreateStreamProcessorRequest()
                    {
                        Input = streamProcessorInput,
                        Output = streamProcessorOutput,
                        Settings = streamProcessorSettings,
                        RoleArn = roleArn,
                        Name = streamProcessorName,
                    });

            //Display result
            Console.WriteLine("Stream Processor " + streamProcessorName + " created.");
            Console.WriteLine("StreamProcessorArn - " + createStreamProcessorResponse.StreamProcessorArn);

            return createStreamProcessorResponse.StreamProcessorArn;
        }

        public void StartStreamProcessor()
        {
            StartStreamProcessorResponse startStreamProcessorResponse =
                    rekognitionClient.StartStreamProcessor(new StartStreamProcessorRequest()
                    {
                        Name = streamProcessorName,
                    });
            Console.WriteLine("Stream Processor " + streamProcessorName + " started.");
        }

        public void StopStreamProcessor()
        {
            StopStreamProcessorResponse stopStreamProcessorResponse =
                    rekognitionClient.StopStreamProcessor(new StopStreamProcessorRequest()
                    {
                        Name = streamProcessorName
                    });
            Console.WriteLine("Stream Processor " + streamProcessorName + " stopped.");
        }

        public void DeleteStreamProcessor()
        {
            DeleteStreamProcessorResponse deleteStreamProcessorResponse = rekognitionClient
                    .DeleteStreamProcessor(new DeleteStreamProcessorRequest()
                    {
                        Name = streamProcessorName
                    });
            Console.WriteLine("Stream Processor " + streamProcessorName + " deleted.");
        }

        public StreamProcessor DescribeStreamProcessor()
        {
            DescribeStreamProcessorResponse describeStreamProcessorResult = rekognitionClient
                    .DescribeStreamProcessor(new DescribeStreamProcessorRequest()
                    {
                        Name = streamProcessorName,
                    });

            StreamProcessor sp = new StreamProcessor() {
                Name = describeStreamProcessorResult.Name,
                Status = describeStreamProcessorResult.Status
            };          

            return sp;
        }

        public List<string> ListStreamProcessors()
        {
            List<string> streamProcessors = new List<string>();
            ListStreamProcessorsResponse listStreamProcessorsResponse =
                    rekognitionClient.ListStreamProcessors(new ListStreamProcessorsRequest()
                    {
                        MaxResults = 100,
                    });

            //List all stream processors (and state) returned from Rekognition
            foreach (StreamProcessor streamProcessor in listStreamProcessorsResponse.StreamProcessors)
            {
                streamProcessors.Add(streamProcessor.Name);
                Console.WriteLine("StreamProcessor name - " + streamProcessor.Name);
                Console.WriteLine("Status - " + streamProcessor.Status + "\n");
            }

            return streamProcessors;
        }
    }
}
