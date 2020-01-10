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

        public static List<Camera> GetAllCamers()
        {
            List<Camera> cameras = new List<Camera>();

            string tableName = MyAWSConfigs.CamerasDBTableName;

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    DynamoDBContext context = new DynamoDBContext(client);
                    IEnumerable<Camera> camerasData = context.Scan<Camera>();
                    
                    cameras = camerasData.ToList();                    
                }
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return cameras;
        }


    }
}

