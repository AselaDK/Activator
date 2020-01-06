using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda;
using Amazon.Lambda.Model;

namespace Activator.Models
{
    class Lambda
    {
        public static bool CreateEventSourceMapping(string eventSourceArn)
        {
            bool isSuccess = false;

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
                    Console.WriteLine("eventSourceMappingResponse State: ", eventSourceMappingResponse.State);
                    isSuccess = true;

                    //if (eventSourceMappingResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    //    isSuccess = true;
                    //else
                    //    Console.WriteLine("Error creating event source mapping");
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
