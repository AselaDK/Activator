using Amazon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activator.Models
{
    class MyAWSConfigs
    {
        public static readonly String refImagesBucketName = "refimagesbucket";
        public static readonly RegionEndpoint refImagesBucketRegion = RegionEndpoint.USWest2;
        public static readonly RegionEndpoint faceCollectionRegion = RegionEndpoint.USWest2;
        public static readonly RegionEndpoint dynamodbRegion = RegionEndpoint.APSoutheast2;
        public static readonly String faceCollectionID = "RefCollection";
    }
}
