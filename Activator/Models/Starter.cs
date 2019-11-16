using Amazon.Rekognition;
using System;

namespace Activator.Models
{
    class Starter
    {
        static String streamProcessorName = MyAWSConfigs.streamProcessorName;
        static String kinesisVideoStreamArn = MyAWSConfigs.kinesisVideoStreamArn;
        static String kinesisDataStreamArn = MyAWSConfigs.kinesisDataStreamArn;
        static String roleArn = MyAWSConfigs.roleArn;
        static String collectionId = MyAWSConfigs.faceCollectionID;
        static float matchThreshold = 50f;

        static StreamManager sm = new StreamManager(
                    streamProcessorName,
                    kinesisVideoStreamArn,
                    kinesisDataStreamArn,
                    roleArn,
                    collectionId,
                    matchThreshold);

        public static void CreateStreamProcessor()
        {
            try
            {
                sm.CreateStreamProcessor();
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        public static void StartStreamProcessor()
        {
            try
            {
                sm.StartStreamProcessor();
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        public static void DeleteStreamProcessor()
        {
            try
            {
                sm.DeleteStreamProcessor();
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        public static void StopStreamProcessor()
        {
            try
            {
                sm.StopStreamProcessor();
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        public static void ListStreamProcessors()
        {
            try
            {
                sm.ListStreamProcessors();
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        public static void DescribeStreamProcessor()
        {
            try
            {
                sm.DescribeStreamProcessor();
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }
}
