using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Lambda;
using Amazon.Lambda.Model;

namespace Activator.Models
{
    class Lambda
    {
        public static string CreateEventSourceMapping(string eventSourceArn)
        {
            string uuid = "";

            try
            {
                AmazonLambdaClient lambdaClient;

                using (lambdaClient = new AmazonLambdaClient(Models.MyAWSConfigs.KinesisRegion))
                {
                    CreateEventSourceMappingRequest eventSourceMappingRequest = new CreateEventSourceMappingRequest
                    {
                        EventSourceArn = eventSourceArn,
                        BatchSize = 100,
                        MaximumBatchingWindowInSeconds = 0,
                        StartingPosition = EventSourcePosition.LATEST,
                        Enabled = true,
                        FunctionName = MyAWSConfigs.LambdaFunctionName,                        
                    };
                    CreateEventSourceMappingResponse eventSourceMappingResponse = lambdaClient.CreateEventSourceMapping(eventSourceMappingRequest);

                    uuid = eventSourceMappingResponse.UUID;

                    string state = eventSourceMappingResponse.State;
                    while (state != "Enabled")
                    {
                        Thread.Sleep(1 * 1000);

                        GetEventSourceMappingRequest getRequest = new GetEventSourceMappingRequest
                        {
                            UUID = uuid,
                        };
                        GetEventSourceMappingResponse getResponse = lambdaClient.GetEventSourceMapping(getRequest);

                        state = getResponse.State;
                    }

                                        
                }
            }
            catch (AmazonLambdaException e)
            {
                Console.WriteLine("AmazonLambdaException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return uuid;
        }

        public static bool DeleteEventSourceMapping(string uuid)
        {
            bool isSuccess = false;

            try
            {
                AmazonLambdaClient lambdaClient;

                using (lambdaClient = new AmazonLambdaClient(Models.MyAWSConfigs.KinesisRegion))
                {
                    DeleteEventSourceMappingRequest request = new DeleteEventSourceMappingRequest
                    {
                        UUID = uuid,
                    };
                    DeleteEventSourceMappingResponse response = lambdaClient.DeleteEventSourceMapping(request);
                    isSuccess = true;

                    Thread.Sleep(1 * 1000);

                    //if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    //    isSuccess = true;
                    //else
                    //    Console.WriteLine("Error deleting event source mapping");
                }
            }
            catch (AmazonLambdaException e)
            {
                Console.WriteLine("AmazonLambdaException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return isSuccess;
        }
    }
}
