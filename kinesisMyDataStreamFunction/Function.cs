using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Amazon;

using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

using Newtonsoft.Json;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;

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
                        //context.Logger.LogLine(recordData);
                        AllRefPersonsId(context).Wait();

                        var itemEvent = new Document();

                        itemEvent["event_id"] = eventID;
                        itemEvent["data"] = recordData;                       

                        WriteItemAsync(itemEvent, context, "history");

                        foreach (Facesearchresponse facesearchresponse in dataObject.FaceSearchResponse)
                        {
                            foreach (Matchedface matchedface in facesearchresponse.MatchedFaces)
                            {
                                var item = new Document();

                                item["id"] = matchedface.Face.ExternalImageId;
                                item["status"] = true;

                                UpdateItemAsync(item, context, "ref_persons");
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

        private void WriteItemAsync(Document item, ILambdaContext context, string tableName)
        {            
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
        
        private void UpdateItemAsync(Document item, ILambdaContext context, string tableName)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(RegionEndpoint.APSoutheast2))
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

        private async Task AllRefPersonsId(ILambdaContext context)
        {
            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(RegionEndpoint.APSoutheast2))
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
                            context.Logger.Log(item["id"].S + " "); 
                        }
                        lastKeyEvaluated = response.LastEvaluatedKey;

                    } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);
                }
                context.Logger.LogLine("");
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

        //private async Task ReadStream(ILambdaContext context)
        //{
        //    string streamArn = "arn:aws:dynamodb:ap-southeast-2:358403828169:table/ref_persons/stream/2019-11-18T05:31:40.045";

        //    try
        //    {
        //        AmazonDynamoDBStreamsClient streamsClient;
        //        using (streamsClient = new AmazonDynamoDBStreamsClient(RegionEndpoint.APSoutheast2))
        //        {
        //            String lastEvaluatedShardId = null;

        //            do
        //            {
        //                DescribeStreamRequest describeStreamRequest = new DescribeStreamRequest() {
        //                    StreamArn = streamArn,
        //                    ExclusiveStartShardId = lastEvaluatedShardId,
        //                };                               

        //                DescribeStreamResponse describeStreamResponse = await streamsClient.DescribeStreamAsync(describeStreamRequest);

        //                List<Shard> shards = describeStreamResponse.StreamDescription.Shards;

        //                // Process each shard on this page

        //                foreach (Shard shard in shards)
        //                {
        //                    String shardId = shard.ShardId;

        //                    // Get an iterator for the current shard

        //                    GetShardIteratorRequest getShardIteratorRequest = new GetShardIteratorRequest() {
        //                        StreamArn = streamArn,
        //                        ShardId = shardId,
        //                        ShardIteratorType = ShardIteratorType.LATEST,                                
        //                    };
                                
        //                    GetShardIteratorResponse getShardIteratorResponse =
        //                        await streamsClient.GetShardIteratorAsync(getShardIteratorRequest);

        //                    String currentShardIter = getShardIteratorResponse.ShardIterator;
                                                       
        //                    int processedRecordCount = 0;
        //                    while (currentShardIter != null && processedRecordCount < maxItemCount)
        //                    {
        //                        // Use the shard iterator to read the stream records

        //                        GetRecordsRequest getRecordsRequest = new GetRecordsRequest() {
        //                               ShardIterator = currentShardIter
        //                        };

        //                        GetRecordsResponse getRecordsResponse = await streamsClient.GetRecordsAsync(getRecordsRequest);

        //                        List<Record> records = getRecordsResponse.Records;

        //                        foreach (Record record in records)
        //                        {
        //                            System.out.println("        " + record.Dynamodb.);
        //                        }
        //                        processedRecordCount += records.Count;
        //                        currentShardIter = getRecordsResponse.NextShardIterator;
        //                    }
        //                }

        //                // If LastEvaluatedShardId is set, then there is
        //                // at least one more page of shard IDs to retrieve
        //                lastEvaluatedShardId = describeStreamResponse.StreamDescription.LastEvaluatedShardId;

        //            } while (lastEvaluatedShardId != null);
        //        }
        //    }
        //    catch (AmazonDynamoDBException e)
        //    {
        //        context.Logger.LogLine("AmazonDynamoDBException: " + e);
        //    }
        //    catch (Exception e)
        //    {
        //        context.Logger.LogLine("Error: " + e);
        //    }            
        //}
    }
}