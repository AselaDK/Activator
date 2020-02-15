using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Activator.Models
{
    [DynamoDBTable("ref_persons")]
    public class RefPerson
    {
        [DynamoDBHashKey]
        public string id
        {
            get; set;
        }

        public string name
        {
            get; set;
        }

        public bool status
        {
            get; set;
        }

        public string camera
        {
            get; set;
        }
        public List<String> readerList 
        { 
            get; set; 
        }
        public string description
        {
            get; set;
        }

        public BitmapImage image
        {
            get; set;
        }
        public bool isCheckedRef
        {
            get; set;
        }

        public static List<RefPerson> GetAllRefPersons()
        {
            string directoryPath = "Resources/Images/";

            List<RefPerson> refPersons = new List<RefPerson>();

            string tableName = MyAWSConfigs.RefPersonsDBTableName;

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    DynamoDBContext context = new DynamoDBContext(client);
                    IEnumerable<RefPerson> refPersonsData = context.Scan<RefPerson>();
                    refPersons = refPersonsData.ToList();
                    foreach (RefPerson person in refPersons)
                    {
                        if (!File.Exists(directoryPath + person.id))
                        {
                            Models.S3Bucket.DownloadFile(person.id, Models.MyAWSConfigs.RefImagesBucketName);
                        }

                        string exeDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
                        Console.WriteLine("\n exeDirectory >>> "+ exeDirectory);

                        Uri fileUri = new Uri(exeDirectory + directoryPath + person.id);

                        person.image = new BitmapImage(fileUri);
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

            return refPersons;
        }
    }
}
