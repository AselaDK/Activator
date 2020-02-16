using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
namespace Activator.Models
{
    [DynamoDBTable("actvitylogs")]
    public class ActivityLogs
    {
        [DynamoDBHashKey]
        public string activityid
        {
            get; set;
        }

        public string userid
        {
            get; set;
        }
        public string description
        {
            get; set;
        }


        public string timestamp
        {
            get; set;
        }

        public void Activity(string id, string uid, string description, string timestamp)
        {

            bool isactivityidEmpty = string.IsNullOrEmpty(id);
            bool isuseridEmpty = string.IsNullOrEmpty(uid);
            bool isDescriptionEmpty = string.IsNullOrEmpty(description);
            bool istimestampEmpty = string.IsNullOrEmpty(timestamp);

            if (!isactivityidEmpty && !isDescriptionEmpty && !isuseridEmpty && !istimestampEmpty)
            {

                var act_item = new Document();


                act_item["activityid"] = id;
                act_item["userid"] = uid;
                act_item["description"] = description;
                act_item["timestamp"] = timestamp;

                Console.WriteLine(id);
                Console.WriteLine(uid);
                Console.WriteLine(description);
                Console.WriteLine(timestamp);

                Dynamodb.PutItem(act_item, Models.MyAWSConfigs.ActivitylogsDBtableName);
            }
            else
            {
                MessageBox.Show("Null Error!");
            }

        }

        public static List<ActivityLogs> GetAllactvityLogs()
        {
            List<ActivityLogs> logs = new List<ActivityLogs>();

            string tableName = MyAWSConfigs.ActivitylogsDBtableName;

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    DynamoDBContext context = new DynamoDBContext(client);
                    IEnumerable<ActivityLogs> logData = context.Scan<ActivityLogs>();
                    logs = logData.ToList();
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

            foreach (var log in logs)
            {
                Console.WriteLine(log.activityid + "   " + log.userid + "\n");
            }

            return logs;
        }
    }
}

