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

        public static List<Admin> GetAdminDetails()
        {
            string directoryPath = "Resources/Images/";

            List<Admin> admins = new List<Admin>();

            string tableName = MyAWSConfigs.AdminDBTableName;

            try
            {
                AmazonDynamoDBClient client;
                using (client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion))
                {
                    DynamoDBContext context = new DynamoDBContext(client);
                    IEnumerable<Admin> adminData = context.Scan<Admin>();
                    admins = adminData.ToList();

                    foreach (Admin admin in admins)
                    {

                        if (!File.Exists(directoryPath + admin.aPropic))
                        {
                            S3Bucket.DownloadFile(admin.aPropic, MyAWSConfigs.AdminS3BucketName);
                        }

                        string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

                        Uri fileUri = new Uri(exeDirectory + directoryPath + admin.aPropic);

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
