using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Item = Amazon.DynamoDBv2.DocumentModel.Document;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

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

        //update item query
        public static void UpdateItem(Document item, string tableName)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    var table = Table.LoadTable(client, tableName);
                    table.UpdateItem(item);
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

        //delete query
        public static void DeleteItem(string id, string tableName)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
                    {
                        { "id", new AttributeValue { S = id } },
                    };

                    DeleteItemRequest deleteItemRequest = new DeleteItemRequest
                    {
                        Key = key,
                        TableName = tableName,
                    };

                    client.DeleteItem(deleteItemRequest);
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

        
        public static List<Document> GetAllDocumentsWithFilter(string tableName, string columnName, string filterValue)
        {
            try
            {
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion);

                Table table = Table.LoadTable(client, tableName);

                ScanFilter scanFilter = new ScanFilter();
                scanFilter.AddCondition(columnName, ScanOperator.Equal, filterValue);

                Search search = table.Scan(scanFilter);
                List<Document> docs = new List<Document>();
                do
                {
                    docs.AddRange(search.GetNextSet().ToList<Document>());

                } while (!search.IsDone);

                var temp = docs.ToList<Document>();

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

        public static List<Logs> GetAllLogs()
        {
            string tableName = MyAWSConfigs.LogsDBTableName;

            List<Logs> logsList = new List<Logs>();

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
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

        //filter query to 'where' clause activitylogs table for admin names
        public static List<ActivityLogs> GetActivitiesOfAdmin(String value, String columnName)
        {
            AmazonDynamoDBClient client;
            client = new AmazonDynamoDBClient();
            DynamoDBContext context = new DynamoDBContext(client);

            IEnumerable<ActivityLogs> activityList = context.Scan<ActivityLogs>(
                new ScanCondition(columnName, ScanOperator.Equal, value)
                );

            Console.WriteLine("\nFindProductsPricedLessThanZero: Printing result.....");
            foreach (ActivityLogs a in activityList)
                Console.WriteLine("{0}\t{1}\t{2}", a.activityid, a.description, a.timestamp);

            var alList = activityList.Cast<ActivityLogs>().ToList();
            return alList;
        }
    }
}
