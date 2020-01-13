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

        public static List<Reader> GetReadersData()
        {
            string directoryPath = "Resources/Images/";

            List<Reader> readers = new List<Reader>();

            string tableName = MyAWSConfigs.ReaderDBtableName;

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    DynamoDBContext context = new DynamoDBContext(client);
                    IEnumerable<Reader> readerData = context.Scan<Reader>();
                    readers = readerData.ToList();

                    foreach (Reader reader in readers)
                    {

                        if (!File.Exists(directoryPath + reader.propic))
                        {
                            S3Bucket.DownloadFile(reader.propic, MyAWSConfigs.ReaderS3BucketName);
                        }

                        string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
                        Console.WriteLine(exeDirectory);

                        Uri fileUri = new Uri(exeDirectory + directoryPath + reader.propic);

                        reader.rImage = new BitmapImage(fileUri);

                        Console.WriteLine(reader.propic);
                    }
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

            return readers;
        }
    }
}
