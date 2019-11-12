using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Activator.Views;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Caliburn.Micro;

namespace Activator.ViewModels
{
    class CamerasPageViewModel: Screen
    {
        
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        public static String[] GetItem(string _id)
        {
            string[] camtabledata = { "", "", "" };

            try
            {
                var client = new AmazonDynamoDBClient(Models.MyAWSConfigs.dynamodbRegion);
                var table = Table.LoadTable(client, "camers");
                var item = table.GetItem(_id);

                camtabledata[0] = item["camId"];
                camtabledata[1] = item["location"];
                camtabledata[2] = item["quality"];

                return camtabledata;
            }
            catch (AmazonDynamoDBException ex)
            {
                MessageBox.Show("Message : Server Error", ex.Message);
                return camtabledata;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message : Unknown Error", ex.Message);
                return camtabledata;
            }
        }
    }
}
