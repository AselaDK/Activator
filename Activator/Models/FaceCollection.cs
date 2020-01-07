using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activator.Models
{
    class FaceCollection
    {
        private static AmazonRekognitionClient rekognitionClient;
        private static RegionEndpoint collectionRegion = MyAWSConfigs.KinesisRegion;
        private static string bucket = MyAWSConfigs.RefImagesBucketName;

        public static List<String> GetFaceCollectionList()
        {
            List<String> _faceCollectionList = new List<string>();
            _faceCollectionList.Clear();

            using (rekognitionClient = new AmazonRekognitionClient(collectionRegion))
            {
                ListingCollections();
            }

            void ListingCollections()
            {
                try
                {
                    int limit = 10;

                    ListCollectionsResponse listCollectionsResponse = null;
                    String paginationToken = null;
                    do
                    {
                        if (listCollectionsResponse != null)
                            paginationToken = listCollectionsResponse.NextToken;

                        ListCollectionsRequest listCollectionsRequest = new ListCollectionsRequest()
                        {
                            MaxResults = limit,
                            NextToken = paginationToken
                        };

                        listCollectionsResponse = rekognitionClient.ListCollections(listCollectionsRequest);

                        foreach (String resultId in listCollectionsResponse.CollectionIds)
                            _faceCollectionList.Add(resultId);

                    } while (listCollectionsResponse != null && listCollectionsResponse.NextToken != null);

                }
                catch (AmazonRekognitionException e)
                {
                    Console.WriteLine("AmazonRekognitionException: " + e);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e);
                }
            }

            return _faceCollectionList;
        }

        public static string Create(string _collectionId)
        {
            if (GetFaceCollectionList().Contains(_collectionId)) return "";

            string collectionId = _collectionId;
            string collectionArn = "";
            try
            {
                using (rekognitionClient = new AmazonRekognitionClient(collectionRegion))
                {
                    CreatingCollection();
                }

                void CreatingCollection()
                {
                    Console.WriteLine("Creating collection: " + collectionId);

                    CreateCollectionRequest createCollectionRequest = new CreateCollectionRequest()
                    {
                        CollectionId = collectionId
                    };

                    CreateCollectionResponse createCollectionResponse = rekognitionClient.CreateCollection(createCollectionRequest);
                    collectionArn = createCollectionResponse.CollectionArn;

                    Console.WriteLine("Status code : " + createCollectionResponse.StatusCode);
                }         
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);
                collectionArn = "error";
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                collectionArn = "error";
            }

            return collectionArn;
        }

        public static void Delete(string _collectionId)
        {
            if (GetFaceCollectionList().Contains(_collectionId))
            {
                string collectionId = _collectionId;
                try
                {
                    using (rekognitionClient = new AmazonRekognitionClient(collectionRegion))
                    {
                        DeletingCollection();
                    }

                    void DeletingCollection()
                    {
                        Console.WriteLine("Deleting collection: " + collectionId);

                        DeleteCollectionRequest deleteCollectionRequest = new DeleteCollectionRequest()
                        {
                            CollectionId = collectionId
                        };

                        DeleteCollectionResponse deleteCollectionResponse = rekognitionClient.DeleteCollection(deleteCollectionRequest);
                        Console.WriteLine(collectionId + ": " + deleteCollectionResponse.StatusCode);                        
                    }
                }
                catch (AmazonRekognitionException e)
                {
                    Console.WriteLine("AmazonRekognitionException: " + e);                   
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e);                    
                }
            }
        }

        public static void AddFace(string _externalImageId, string _collectionId)
        {
            try
            {
                string photo = _externalImageId;
                string collectionId = _collectionId;

                using (rekognitionClient = new AmazonRekognitionClient(collectionRegion))
                {
                    AddingFace();
                }

                void AddingFace()
                {
                    Image image = new Image()
                    {
                        S3Object = new S3Object()
                        {
                            Bucket = bucket,
                            Name = photo
                        }
                    };

                    IndexFacesRequest indexFacesRequest = new IndexFacesRequest()
                    {
                        Image = image,
                        CollectionId = collectionId,
                        ExternalImageId = photo,
                        DetectionAttributes = new List<String>() { "ALL" }
                    };

                    IndexFacesResponse indexFacesResponse = rekognitionClient.IndexFaces(indexFacesRequest);

                    Console.WriteLine(photo + " added");

                    foreach (FaceRecord faceRecord in indexFacesResponse.FaceRecords)
                        Console.WriteLine("Face detected: Faceid is " +
                           faceRecord.Face.FaceId);
                }                
            }
            catch (AmazonRekognitionException e)
            {
                Console.WriteLine("AmazonRekognitionException: " + e);               
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);              
            }
        }

        public static Dictionary<string, string> GetFaceList(string _collectionId)
        {
            string collectionId = _collectionId;
            Dictionary<string, string> _faceList = new Dictionary<string, string>();
            _faceList.Clear();

            using (rekognitionClient = new AmazonRekognitionClient(collectionRegion))
            {
                GettingFaceList();
            }

            void GettingFaceList()
            {
                try
                {
                    ListFacesResponse listFacesResponse = null;
                    
                    String paginationToken = null;
                    do
                    {
                        if (listFacesResponse != null)
                            paginationToken = listFacesResponse.NextToken;

                        ListFacesRequest listFacesRequest = new ListFacesRequest()
                        {
                            CollectionId = collectionId,
                            MaxResults = 1,
                            NextToken = paginationToken
                        };

                        listFacesResponse = rekognitionClient.ListFaces(listFacesRequest);
                        foreach (Face face in listFacesResponse.Faces)
                            _faceList[face.FaceId] = face.ExternalImageId;

                    } while (listFacesResponse != null && !String.IsNullOrEmpty(listFacesResponse.NextToken));
                }
                catch (AmazonRekognitionException e)
                {
                    Console.WriteLine("AmazonRekognitionException: " + e);                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e);                    
                }
            }
            return _faceList;
        }
    }
}
