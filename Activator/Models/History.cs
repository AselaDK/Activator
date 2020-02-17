using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace Activator.Models
{
    [DynamoDBTable("history")]
    class History
    {
        [DynamoDBHashKey]
        public string event_id { get; set; }
        public string cameraId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string timestamp { get; set; }


        public static List<History> GetHistoryDetails()
        {

            List<History> historylist = new List<History>();

            string tableName = MyAWSConfigs.HistoryDBtableName;

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    DynamoDBContext context = new DynamoDBContext(client);
                    IEnumerable<History> historyData = context.Scan<History>();
                    historylist = historyData.ToList();

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

            return historylist;
        }
    }
}

