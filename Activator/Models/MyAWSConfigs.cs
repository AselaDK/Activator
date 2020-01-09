using Amazon;
using System;

namespace Activator.Models
{
    class MyAWSConfigs
    {
        public static readonly String refImagesBucketName = "refimagesbucket";
        public static readonly String faceCollectionID = "RefCollection";
        public static readonly String refPersonsDBTableName = "ref_persons";
        public static readonly String logsDBTableName = "logs";
        public static readonly String adminDBTableName = "admin";
        public static readonly String RefImagesBucketName = "refimagesbucket";
        public static readonly String FaceCollectionID = "RefFaceCollection";

        public static readonly String RefPersonsDBTableName = "ref_persons";
        public static readonly String CamerasDBTableName = "cameras";

        public static readonly String LambdaFunctionName = "DataStreamsToDynamoDBFunction";
        public static readonly String RoleArn = "arn:aws:iam::358403828169:role/RekognitionServiceRole";
        public static readonly String DynamodbRefPersonTableStreamArn = "arn:aws:dynamodb:ap-southeast-2:358403828169:table/ref_persons/stream/2019-11-18T05:31:40.045";

        public static readonly RegionEndpoint S3BucketRegion = RegionEndpoint.USWest2;
        public static readonly RegionEndpoint KinesisRegion = RegionEndpoint.APNortheast1;
        public static readonly RegionEndpoint DynamodbRegion = RegionEndpoint.APSoutheast2;
    }
}
