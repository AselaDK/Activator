using Amazon.Rekognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activator.Models
{
    class Starter
    {
        static String streamProcessorName = "my-stream-processor";
        static String kinesisVideoStreamArn = "arn:aws:kinesisvideo:us-west-2:358403828169:stream/my-stream/1569206107887";
        static String kinesisDataStreamArn = "arn:aws:kinesis:us-west-2:358403828169:stream/AmazonRekognitionMyDataStream";
        static String roleArn = "arn:aws:iam::358403828169:role/RekognitionServiceRole";
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
