using Amazon;
using System;

namespace Activator.Models
{
    class MyAWSConfigs
    {
        public static readonly String refImagesBucketName = "refimagesbucket";
        public static readonly String faceCollectionID = "RefCollection";
        public static readonly String refPersonsDBTableName = "ref_persons";
<<<<<<< HEAD
        public static readonly String logsDBTableName = "logs";
=======
        public static readonly String adminDBTableName = "admin";
        public static readonly String RefImagesBucketName = "refimagesbucket";
        public static readonly String FaceCollectionID = "RefCollection";
>>>>>>> ea31f59ba324dd8c44375a26d4fc9dead433af55

        public static readonly String RefPersonsDBTableName = "ref_persons";
        public static readonly String CamerasDBTableName = "cameras";

        
        public static readonly String KinesisDataStreamArn = "arn:aws:kinesis:us-west-2:358403828169:stream/AmazonRekognition_DataStream";
        //public static readonly String KinesisDataStreamName = "my-data-stream";
        public static readonly String RoleArn = "arn:aws:iam::358403828169:role/RekognitionServiceRole";

        public static readonly RegionEndpoint S3BucketRegion = RegionEndpoint.USWest2;
        public static readonly RegionEndpoint KinesisRegion = RegionEndpoint.USWest2;
        public static readonly RegionEndpoint DynamodbRegion = RegionEndpoint.APSoutheast2;
    }
}
