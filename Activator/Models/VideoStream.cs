using System;
using System.Collections.Generic;

using Amazon.KinesisVideo.Model;
using Amazon.KinesisVideo;

namespace Activator.Models
{
    class VideoStream
    {
        public static string CreateVideoStream(string streamName)
        {
            string streamArn = "";

            try
            {
                List<StreamInfo> streamlList = GetVideoStreamList();

                if (streamlList != null)
                {
                    if (streamlList.FindAll(videoStream => videoStream.StreamName == streamName).Count > 0)
                        streamArn = "contain";
                    else
                    {
                        AmazonKinesisVideoClient kinesisVideoClient;

                        using (kinesisVideoClient = new AmazonKinesisVideoClient(Models.MyAWSConfigs.KinesisRegion))
                        {
                            CreateStreamRequest createStreamRequest = new CreateStreamRequest()
                            {
                                StreamName = streamName,
                                DataRetentionInHours = 1,
                                MediaType = "video/h264",
                            };

                            CreateStreamResponse createStreamResponse = kinesisVideoClient.CreateStream(createStreamRequest);

                            if (createStreamResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                                streamArn = createStreamResponse.StreamARN;
                            else
                                Console.WriteLine("Error creating kinesis video stream");
                        }
                    }
                }
            }
            catch (AmazonKinesisVideoException e)
            {
                Console.WriteLine("AmazonKinesisVideoException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }           

            return streamArn;
        }

        public static List<StreamInfo> GetVideoStreamList()
        {
            List<StreamInfo> streamList = null;

            try
            {
                AmazonKinesisVideoClient kinesisVideoClient;

                using (kinesisVideoClient = new AmazonKinesisVideoClient(Models.MyAWSConfigs.KinesisRegion))
                {
                    ListStreamsRequest listStreamsRequest = new ListStreamsRequest();
                    ListStreamsResponse listStreamsResponse = kinesisVideoClient.ListStreams(listStreamsRequest);

                    if (listStreamsResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        streamList = listStreamsResponse.StreamInfoList;
                    else
                        Console.WriteLine("Error listing kinesis video streams");
                }
            }
            catch (AmazonKinesisVideoException e)
            {
                Console.WriteLine("AmazonKinesisVideoException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }            

            return streamList;
        }
    }
}
