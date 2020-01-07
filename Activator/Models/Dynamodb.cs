using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Activator.Models
{
    class Dynamodb
    {
        public static void PutItem(Document item, string tableName)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    var table = Table.LoadTable(client, tableName);
                    table.PutItem(item);
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
        }

        public static Document GetItem(String itemId, string tableName)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    var table = Table.LoadTable(client, tableName);
                    Document item = table.GetItem(itemId);

                    return item;
                }
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException: " + e);
                return new Document();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                return new Document();
            }
        }

        public static List<Logs> GetAllLogs()
        {
            string tableName = MyAWSConfigs.logsDBTableName;

            List<Logs> logsList = new List<Logs>();

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.dynamodbRegion))
                {
                    DynamoDBContext context = new DynamoDBContext(client);
                    IEnumerable<Logs> logsData = context.Scan<Logs>();
                    logsList = logsData.ToList();
                    
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

            return logsList;
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
                        if (!File.Exists(directoryPath+person.id))
                        {
                            Models.S3Bucket.DownloadFile(person.id);
                        }
                        
                        string exeDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
                        Console.WriteLine(exeDirectory);

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
