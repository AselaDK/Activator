using Amazon;
using System;

namespace Activator.Models
{
    class MyAWSConfigs
    {
        public static readonly String RefImagesBucketName = "refimagesbucket";
        public static readonly String FaceCollectionID = "RefFaceCollection";

        public static readonly String RefPersonsDBTableName = "ref_persons";
        public static readonly String CamerasDBTableName = "cameras";

        public static readonly String LambdaFunctionName = "DataStreamsToDynamoDBFunction";
        public static readonly String RoleArn = "arn:aws:iam::358403828169:role/RekognitionServiceRole";

        public static readonly RegionEndpoint S3BucketRegion = RegionEndpoint.USWest2;
        public static readonly RegionEndpoint KinesisRegion = RegionEndpoint.APSoutheast2;
        public static readonly RegionEndpoint DynamodbRegion = RegionEndpoint.APSoutheast2;
    }
}
