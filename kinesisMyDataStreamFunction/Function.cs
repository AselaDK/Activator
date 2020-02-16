using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace kinesisMyDataStreamFunction
{
    public class Function
    {
        Dictionary<string, Person> allRefPersons = new Dictionary<string, Person>();

        public void FunctionHandler(KinesisEvent kinesisEvent, ILambdaContext context)
        {
            context.Logger.LogLine($"Beginning to process {kinesisEvent.Records.Count} records...");

            foreach (var record in kinesisEvent.Records)
            {
                //context.Logger.LogLine($"Event ID: {record.EventId}");
                //context.Logger.LogLine($"Event Name: {record.EventName}");

                string recordData = GetRecordContents(record.Kinesis);
                string eventID = record.EventId;

                Rootobject dataObject = JsonConvert.DeserializeObject<Rootobject>(recordData);

                string[] temp = dataObject.InputInformation.KinesisVideo.StreamArn.Split('/');
                string detectedCamera = temp[temp.Length - 2];
                int detectedCameraId = int.Parse(detectedCamera[detectedCamera.Length - 1].ToString());

                if (dataObject.FaceSearchResponse.Length != 0)
                {
                    if (dataObject.FaceSearchResponse[0].MatchedFaces.Length != 0)
                    {
                        //context.Logger.LogLine(recordData);                                                                  

                        foreach (Facesearchresponse facesearchresponse in dataObject.FaceSearchResponse)
                        {
                            GetAllRefPersons(context).Wait();

                            List<string> detectedList = new List<string>();
                            detectedList.Clear();

                            foreach (Matchedface matchedface in facesearchresponse.MatchedFaces)
                            {
                                if (!detectedList.Contains(matchedface.Face.ExternalImageId))
                                {
                                    detectedList.Add(matchedface.Face.ExternalImageId);
                                }
                            }

                            foreach (Person person in allRefPersons.Values)
                            {
                                string id = person.Id;
                                string status = person.Status;
                                string name = person.Name;

                                //context.Logger.LogLine($"id: {id}, status: {status}");

                                if (detectedList.Contains(id))
                                {
                                    if (status == "0")
                                    {
                                        var itemUpdate = new Document();

                                        itemUpdate["id"] = id;
                                        itemUpdate["status"] = 1;
                                        itemUpdate["camera"] = detectedCameraId.ToString();

                                        UpdateItemAsync(itemUpdate, context, "ref_persons");

                                        var HistoryItem = new Document();
                                        HistoryItem["event_id"] = eventID;
                                        HistoryItem["id"] = id;
                                        HistoryItem["cameraId"] = detectedCameraId.ToString();
                                        HistoryItem["name"] = name;
                                        HistoryItem["timestamp"] = DateTime.UtcNow;

                                        WriteItemAsync(HistoryItem, context, "history");
                                    }
                                }
                                else
                                {
                                    if (status == "1")
                                    {
                                        var itemUpdate = new Document();

                                        itemUpdate["id"] = id;
                                        itemUpdate["status"] = 0;

                                        UpdateItemAsync(itemUpdate, context, "ref_persons");
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        foreach (Person person in allRefPersons.Values)
                        {
                            string id = person.Id;
                            string status = person.Status;
                            string name = person.Name;

                            if (status == "1")
                            {
                                var itemUpdate = new Document();

                                itemUpdate["id"] = id;
                                itemUpdate["status"] = 0;

                                UpdateItemAsync(itemUpdate, context, "ref_persons");
                            }
                        }
                    }
                }
                else
                {
                    foreach (Person person in allRefPersons.Values)
                    {
                        string id = person.Id;
                        string status = person.Status;
                        string name = person.Name;

                        if (status == "1")
                        {
                            var itemUpdate = new Document();

                            itemUpdate["id"] = id;
                            itemUpdate["status"] = 0;

                            UpdateItemAsync(itemUpdate, context, "ref_persons");
                        }
                    }
                }
            }

            //context.Logger.LogLine("Stream processing complete.");
        }

        private string GetRecordContents(KinesisEvent.Record streamRecord)
        {
            using (var reader = new StreamReader(streamRecord.Data, Encoding.ASCII))
            {
                return reader.ReadToEnd();
            }
        }

        private void WriteItemAsync(Document item, ILambdaContext context, string tableName)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    var table = Table.LoadTable(client, tableName);
                    table.PutItemAsync(item);
                }
            }
            catch (AmazonDynamoDBException e)
            {
                context.Logger.LogLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error: " + e);
            }
        }

        private void UpdateItemAsync(Document item, ILambdaContext context, string tableName)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    var table = Table.LoadTable(client, tableName);

                    table.UpdateItemAsync(item);
                }
            }
            catch (AmazonDynamoDBException e)
            {
                context.Logger.LogLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error: " + e);
            }
        }

        private async Task GetAllRefPersons(ILambdaContext context)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    string tableName = "ref_persons";

                    Dictionary<string, AttributeValue> lastKeyEvaluated = null;
                    do
                    {
                        var request = new ScanRequest
                        {
                            TableName = tableName,
                            Limit = 20,
                            ExclusiveStartKey = lastKeyEvaluated,
                            AttributesToGet = { "id", "status", "name" },
                        };

                        ScanResponse response = await client.ScanAsync(request);

                        allRefPersons.Clear();
                        foreach (Dictionary<string, AttributeValue> item
                          in response.Items)
                        {
                            Person person = new Person
                            {
                                Id = item["id"].S,
                                Status = item["status"].N,
                                Name = item["name"].S
                            };
                            allRefPersons.Add(item["id"].S, person);

                            //context.Logger.LogLine($"Get All Function\nid: {item["id"].S}, status: {item["status"].N}");
                        }
                        lastKeyEvaluated = response.LastEvaluatedKey;

                    } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);
                }
            }
            catch (AmazonDynamoDBException e)
            {
                context.Logger.LogLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error: " + e);
            }
        }
    }
}