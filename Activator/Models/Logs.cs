using Amazon.DynamoDBv2.DataModel;

namespace Activator.Models
{
    [DynamoDBTable("logs")]
    public class Logs
    {
        [DynamoDBHashKey]
        public string Id
        {
            get; set;
        }
        
        public string Timestamp
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

       

        
    }
}
