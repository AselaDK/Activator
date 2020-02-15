using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Activator.Models
{
    [DynamoDBTable("reader")]
    class Reader
    {
        [DynamoDBHashKey]
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public List<String> refList { get; set; }
        public bool status { get; set; }
        public string propic { get; set; }
        public BitmapImage rImage { get; set; }

        public static IEnumerable<Reader> GetReadersData()
        {
            try
            {
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion);

                DynamoDBContext context = new DynamoDBContext(client);

                IEnumerable<Reader> readerData = context.Scan<Reader>();

                var tempReader = readerData.ToList<Reader>();

                client.Dispose();

                return tempReader;
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
