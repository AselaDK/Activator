using Amazon.DynamoDBv2.DataModel;
using System.Windows.Media.Imaging;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using System.Linq;

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

        public static void Activity(int comp, string description)
        //public static void Activity(string id, string uid, string description, string timestamp)
        {
            var act_item = new Document();

            string id = DateTime.Now + "_" + Session.id + "_" + comp.ToString();
            act_item["activityid"] = id;
            act_item["userid"] = Session.id;
            act_item["description"] = description;
            act_item["timestamp"] = DateTime.UtcNow;

            Task.Run(() => Dynamodb.PutItem(act_item, MyAWSConfigs.ActivitylogsDBtableName));

            //Console.WriteLine($"{id} - {Session.id} - {description} - {timestamp}");

        }

        public static List<ActivityLogs> GetAllactvityLogs()
        {
            List<ActivityLogs> logs = new List<ActivityLogs>();

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

            return logs;
        }
    }
}

