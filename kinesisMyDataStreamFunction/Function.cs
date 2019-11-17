using System;
using System.IO;
using System.Text;

using Amazon;

using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace kinesisMyDataStreamFunction
{
    public class Function
    {

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

                if (dataObject.FaceSearchResponse.Length != 0)
                {
                    if (dataObject.FaceSearchResponse[0].MatchedFaces.Length != 0)
                    {
                        //context.Logger.LogLine($"Record Data:");
                        context.Logger.LogLine(recordData);

                        foreach (Facesearchresponse facesearchresponse in dataObject.FaceSearchResponse)
                        {
                            foreach (Matchedface matchedface in facesearchresponse.MatchedFaces)
                            {
                                var item = new Document();

                                item["id"] = eventID.ToString();
                                item["similarity"] = matchedface.Similarity;
                                item["confidence"] = matchedface.Face.Confidence;
                                item["externalImageId"] = matchedface.Face.ExternalImageId;

                                WriteItemAsync(item, context);
                            }
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

        private void WriteItemAsync(Document item, ILambdaContext context)
        {
            string tableName = "detected_persons";
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(RegionEndpoint.APSoutheast2))
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
    }
}