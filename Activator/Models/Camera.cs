using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activator.Models
{
    [DynamoDBTable("cameras")]
    public class Camera
    {
        [DynamoDBHashKey]
        public string id { get; set; }

        public string description { get; set; }

        public string location { get; set; }

        public string videoStreamArn { get; set; }

        public string eventSourceUUID { get; set; }

        public static IEnumerable<Camera> GetAllCamers()
        {
            try
            {
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion);

                DynamoDBContext context = new DynamoDBContext(client);
                IEnumerable<Camera> camerasData = context.Scan<Camera>();

                var temp = camerasData.ToList();

                client.Dispose();

                return temp;
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return null;
        }
    }
}

