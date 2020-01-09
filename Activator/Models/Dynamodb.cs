﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;

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

        public static long GetItemCount(string tableName)
        {
            long itemCount = 0;

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {                    
                    Dictionary<string, AttributeValue> lastKeyEvaluated = null;
                    do
                    {                      
                        ScanRequest scanRequest = new ScanRequest
                        {
                            TableName = tableName,
                            ExclusiveStartKey = lastKeyEvaluated
                        };

                        ScanResponse scanResponse = client.Scan(scanRequest);
                        itemCount += scanResponse.Count;
                        Console.WriteLine("Item Count in DynamoDB: ", itemCount);

                        lastKeyEvaluated = scanResponse.LastEvaluatedKey;                        
                    }
                    while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);                    
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

            return itemCount;
        }
    }
}
