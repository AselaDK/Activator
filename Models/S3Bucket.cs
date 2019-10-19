using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activator.Models
{
    class S3Bucket
    {
        // bucket setup
        private static string bucketName = MyAWSConfigs.refImagesBucketName;
        private static RegionEndpoint bucketRegion = MyAWSConfigs.refImagesBucketRegion;

        private static IAmazonS3 s3Client;

        public static void UploadFile(string _filePath)
        {
            string filePath = _filePath;

            // uploading
            using (s3Client = new AmazonS3Client(bucketRegion))
            {
                UploadFileAsync().Wait();
            }

            async Task UploadFileAsync()
            {
                try
                {
                    var fileTransferUtility = new TransferUtility(s3Client);

                    await fileTransferUtility.UploadAsync(filePath, bucketName).ConfigureAwait(false);
                    Console.WriteLine("upload finished");

                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine("AmazonS3Exception: " + e);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e);
                }
            }
        }

        public static List<string> GetFilesList()
        {
            List<String> refNames = new List<string>();
            refNames.Clear();

            using (s3Client = new AmazonS3Client(bucketRegion))
            {
                ListingObjectsAsync().Wait();
            }

            async Task ListingObjectsAsync()
            {
                try
                {
                    ListObjectsRequest request = new ListObjectsRequest
                    {
                        BucketName = bucketName,
                        MaxKeys = 2
                    };

                    do
                    {
                        ListObjectsResponse response = await s3Client.ListObjectsAsync(request).ConfigureAwait(false);

                        foreach (S3Object entry in response.S3Objects)
                        {
                            refNames.Add(entry.Key);
                        }

                        if (response.IsTruncated)
                        {
                            request.Marker = response.NextMarker;
                        }
                        else
                        {
                            request = null;
                        }
                    } while (request != null);
                    Console.WriteLine("Got all files");
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine("AmazonS3Exception: " + e);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e);
                }
            }

            return refNames;
        }
 }
        
}

