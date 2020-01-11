using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;

using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace kinesisMyDataStreamFunction
{
    public class Function
    {
        List<string> allRefPersonsId = new List<string>();

        public void FunctionHandler(KinesisEvent kinesisEvent, ILambdaContext context)
        {
            //context.Logger.LogLine($"Beginning to process {kinesisEvent.Records.Count} records...");

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

                GetAllRefPersonsId(context).Wait();                

                if (dataObject.FaceSearchResponse.Length != 0)
                {
                    if (dataObject.FaceSearchResponse[0].MatchedFaces.Length != 0)
                    {
                        //context.Logger.LogLine(recordData);                                                                  

                        foreach (Facesearchresponse facesearchresponse in dataObject.FaceSearchResponse)
                        {
                            List<string> detectedList = new List<string>();
                            detectedList.Clear();

                            foreach (Matchedface matchedface in facesearchresponse.MatchedFaces)
                            {
                                if (!detectedList.Contains(matchedface.Face.ExternalImageId))
                                {
                                    detectedList.Add(matchedface.Face.ExternalImageId);
                                                                        
                                    var notificationItem = new Document();
                                    notificationItem["event_id"] = eventID;
                                    notificationItem["timestamp"] = dataObject.InputInformation.KinesisVideo.ProducerTimestamp; ;
                                    notificationItem["message"] = $"{matchedface.Face.ExternalImageId} is " +
                                                                    $"detected by camera {detectedCameraId}";

                                    WriteItemAsync(notificationItem, context, "history");
                                }                               
                            }

                            foreach (string id in allRefPersonsId)
                            {
                                if (detectedList.Contains(id))
                                {
                                    Document refPerson = ReadItemAsync(id, context, "ref_persons");
                                    DynamoDBEntry entry = refPerson["status"];
                                    if (entry.AsBoolean() == false)
                                    {
                                        var itemUpdate = new Document();
                                        
                                        itemUpdate["id"] = id;
                                        itemUpdate["status"] = true;
                                        itemUpdate["camera"] = detectedCameraId.ToString();

                                        UpdateItemAsync(itemUpdate, context, "ref_persons");
                                    }
                                }
                                else
                                {
                                    Document refPerson = ReadItemAsync(id, context, "ref_persons");
                                    DynamoDBEntry entry = refPerson["status"];
                                    if (entry.AsBoolean() == true)
                                    {
                                        var itemUpdate = new Document();

                                        itemUpdate["id"] = id;
                                        itemUpdate["status"] = false;

                                        UpdateItemAsync(itemUpdate, context, "ref_persons");
                                    }
                                }
                            }

                        }                      
                    }  
                    else
                    {
                        foreach (string id in allRefPersonsId)
                        {
                            Document refPerson = ReadItemAsync(id, context, "ref_persons");
                            DynamoDBEntry entry = refPerson["status"];
                            if (entry.AsBoolean() == true)
                            {
                                var itemUpdate = new Document();

                                itemUpdate["id"] = id;
                                itemUpdate["status"] = false;

                                UpdateItemAsync(itemUpdate, context, "ref_persons");
                            }
                        }
                    }
                }
                else
                {
                    foreach (string id in allRefPersonsId)
                    {
                        Document refPerson = ReadItemAsync(id, context, "ref_persons");
                        DynamoDBEntry entry = refPerson["status"];
                        if (entry.AsBoolean() == true)
                        {
                            var itemUpdate = new Document();

                            itemUpdate["id"] = id;
                            itemUpdate["status"] = false;

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

        private Document ReadItemAsync(String id, ILambdaContext context, string tableName)
        {
            Document doc = new Document();
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {

                    GetAsync();
                }

                async void GetAsync()
                {
                    var table = Table.LoadTable(client, tableName);
                    doc = await table.GetItemAsync(id);
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
            return doc;
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

        private async Task GetAllRefPersonsId(ILambdaContext context)
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
                            AttributesToGet = {"id"},
                        };
                                                
                        ScanResponse response = await client.ScanAsync(request);

                        foreach (Dictionary<string, AttributeValue> item
                          in response.Items)
                        {
                            if (!allRefPersonsId.Contains(item["id"].S)) allRefPersonsId.Add(item["id"].S);                            
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