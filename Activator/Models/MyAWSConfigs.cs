using Amazon;
using System;

namespace Activator.Models
{
    class MyAWSConfigs
    {
        public static readonly String RefImagesBucketName = "refimagesbucket";
        public static readonly String ReaderS3BucketName = "readerforactivator";
        public static readonly String AdminS3BucketName = "adminpropics";

        public static readonly String FaceCollectionID = "RefFaceCollection";

        public static readonly String LogsDBTableName = "logs";
        public static readonly String AdminDBTableName = "admin";
        public static readonly String RefPersonsDBTableName = "ref_persons";
        public static readonly String CamerasDBTableName = "cameras";
        public static readonly String ReaderDBtableName = "reader";
        

        public static readonly String LambdaFunctionName = "DataStreamsToDynamoDBFunction";
        public static readonly String RoleArn = "arn:aws:iam::358403828169:role/RekognitionServiceRole";
        public static readonly String DynamodbRefPersonTableStreamArn = "arn:aws:dynamodb:ap-southeast-2:358403828169:table/ref_persons/stream/2019-11-18T05:31:40.045";

        public static readonly RegionEndpoint S3BucketRegion = RegionEndpoint.USWest2;
        public static readonly RegionEndpoint KinesisRegion = RegionEndpoint.APNortheast1;
        public static readonly RegionEndpoint DynamodbRegion = RegionEndpoint.APSoutheast2;
    }
}
