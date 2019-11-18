using Amazon.DynamoDBv2.DataModel;
using System.Windows.Media.Imaging;

namespace Activator.Models
{
    [DynamoDBTable("ref_persons")]
    public class RefPerson
    {
        [DynamoDBHashKey]
        public string id
        {
            get; set;
        }

        public string name
        {
            get; set;
        }

        public bool status
        {
            get; set;
        }

        public string description
        {
            get; set;
        }

        public BitmapImage image
        {
            get; set;
        }
    }
}
