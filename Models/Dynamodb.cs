using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Activator.Models
{
    class Dynamodb
    {
        public static void PutItem(Document item, string tableName)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.dynamodbRegion))
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
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.dynamodbRegion))
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
    }
}
