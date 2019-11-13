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
using Activator.Models;
using Amazon.DynamoDBv2.Model;

namespace Activator.ViewModels
{
    
    class CamerasPageViewModel: Screen, INotifyPropertyChangedEx
    {
        private IObservableCollection<CamerasPageModel> cameras;
        private AmazonDynamoDBClient client;

        public IObservableCollection<CamerasPageModel> Cameras
        {
            get { return this.cameras; }
            set
            {
                if (this.cameras != value)
                {
                    this.cameras = value;
                    this.OnPropertyChanged("Cameras");
                }
            }
        }
        public CamerasPageViewModel()
        {
            try
            {
                this.client = new AmazonDynamoDBClient();
                this.Cameras = new IObservableCollection<CamerasPageModel>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: failed to create a DynamoDB client; " + ex.Message);
            }
            
            //GetItem();
            //this.CamerasPageModels = new IObservableCollection<CamerasPageView>();
            //this.CreateCustomersTable();
            //this.AddCustomers();
            //CamerasPageModels = new IObservableCollection<CamerasPageModel>();
            //this.LoadData(null);
        }

        

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        

        public static String[] GetItem(string _id)
        {
            string[] camtabledata = { "", "", "" };

            try
            {
                var client = new AmazonDynamoDBClient(MyAWSConfigs.dynamodbRegion);
                var table = Table.LoadTable(client, "cameras");
                //var item = table.GetItem();

                //camtabledata[0] = item["camId"];
                //camtabledata[1] = item["location"];
                //camtabledata[2] = item["quality"];

                //this.LoadData(null);

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

        // MainWindowViewModel.cs 
        private void LoadData(object obj)
        {
            var client = new AmazonDynamoDBClient(MyAWSConfigs.dynamodbRegion);
            var table = Table.LoadTable(client, "cameras");
            var search = table.Scan(new Amazon.DynamoDBv2.DocumentModel.Expression());

            var documentList = new List<Document>();
            do
            {
                documentList.AddRange(search.GetNextSet());

            } while (!search.IsDone);

            //var cameras = new IObservableCollection<CamerasPageModel>();
            foreach (var doc in documentList)
            {
                var camera = new CamerasPageModel();
                foreach (var attribute in doc.GetAttributeNames())
                {
                    var value = doc[attribute];
                    if (attribute == "camId")
                    {
                        camera.camId = value.AsPrimitive().Value.ToString();
                    }
                    else if (attribute == "location")
                    {
                        camera.location = value.AsPrimitive().Value.ToString();
                    }
                    else if (attribute == "quality")
                    {
                        camera.quality = value.AsPrimitive().Value.ToString();
                    }
                }

                cameras.Add(camera);
            }

            //this.CamerasPageModels = cameras;
        }

        private static void RetrieveBook(Table productCatalog)
        {
            Console.WriteLine("\n*** Executing RetrieveBook() ***");
            // Optional configuration.
            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "camId", "location", "quality"},
                ConsistentRead = true
            };
            //Document document = productCatalog.GetItem(sampleBookId, config);
            Console.WriteLine("RetrieveBook: Printing book retrieved...");
            //PrintDocument(document);
        }


        #region INotifyPropertyChanged Members

        public event DependencyPropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
    

