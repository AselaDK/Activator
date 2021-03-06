﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string camera
        {
            get; set;
        }

        public string lastLocation
        {
            get; set;
        }

        public List<String> readerList
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
        public bool isCheckedRef
        {
            get; set;
        }

        public static IEnumerable<RefPerson> GetAllRefPersons()
        {
            try
            {
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(MyAWSConfigs.DynamodbRegion);

                DynamoDBContext context = new DynamoDBContext(client);

                IEnumerable<RefPerson> refPersonsData = context.Scan<RefPerson>();

                var tempPersons = refPersonsData.ToList<RefPerson>();

                client.Dispose();

                return tempPersons;
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine("AmazonDynamoDBException: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return null;
        }
    }
}
