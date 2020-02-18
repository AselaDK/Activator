using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Activator.Models
{
    [DynamoDBTable("admin")]
    class Admin
    {
        [DynamoDBHashKey]
        public string aId { get; set; }
        public string aName { get; set; }
        public string aPassword { get; set; }
        public string aPhone { get; set; }
        public string aPropic { get; set; }

        public bool root { get; set; }
        public BitmapImage aImage { get; set; }

        public bool blocked { get; set; }

        //Get All Admin details list
        public static List<Admin> GetAdminDetails()
        {
            //dp image location
            string directoryPath = "Resources/Images/";

            List<Admin> admins = new List<Admin>();

            string tableName = MyAWSConfigs.AdminDBTableName;

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    //get admin list from database
                    DynamoDBContext context = new DynamoDBContext(client);
                    IEnumerable<Admin> adminData = context.Scan<Admin>();
                    admins = adminData.ToList();

                    // assign their dps to from s3 bucket
                    foreach (Admin admin in admins)
                    {
                        //check file exists in local
                        if (!File.Exists(directoryPath + admin.aPropic))
                        {
                            //download files if not exists
                            S3Bucket.DownloadFile(admin.aPropic, MyAWSConfigs.AdminS3BucketName);
                        }
                        //derectory path to application
                        string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

                        //get file uri
                        Uri fileUri = new Uri(exeDirectory + directoryPath + admin.aPropic);

                        //bind it to aImage attribute
                        admin.aImage = new BitmapImage(fileUri);
                    }
                }
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return admins;
        }
    }
}
