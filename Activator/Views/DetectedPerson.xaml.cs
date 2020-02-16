using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for DetectedPerson.xaml
    /// </summary>
    public partial class DetectedPerson : UserControl
    {
        private string pId;
        private string pName;
        private string pDescription;
        private string objectType;
        private ImageSource pImageSource;

        private MainView mv;
        private Object parent;

        Dictionary<string, Models.Camera> cameras = new Dictionary<string, Models.Camera>();

        public DetectedPerson()
        {
            InitializeComponent();
            GetAllCameras().ConfigureAwait(false);
        }

        private async Task GetAllCameras()
        {
            progressBar.Visibility = Visibility.Visible;
            try
            {
                IEnumerable<Models.Camera> temp = await Task.Run(() => Models.Camera.GetAllCamers());

                cameras.Clear();

                foreach (Models.Camera camera in temp)
                {
                    cameras.Add(camera.id, camera);
                }
            }
            finally
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        public void LoadPerson(string pId, string pName, string pDescription, ImageSource pImageSource, MainView mv, Object parent, string objectType)
        {
            this.pId = pId;
            this.pName = pName;
            this.pDescription = pDescription;
            this.pImageSource = pImageSource;

            this.objectType = objectType;
            this.mv = mv;
            this.parent = parent;

            personName.Text = pName;
            personDescription.Text = pDescription;
            personImage.Source = pImageSource;

            switch (objectType)
            {
                case "home":
                    BtnEdit.IsEnabled = false;
                    BtnDelete.IsEnabled = false;
                    break;
                default:
                    BtnEdit.IsEnabled = true;
                    BtnDelete.IsEnabled = true;
                    break;
            }

            LoadHistory().ConfigureAwait(false);
        }

        private async Task LoadHistory()
        {
            IEnumerable<Document> result = await Task.Run(() => Models.Dynamodb.GetAllDocumentsWithFilter
            (
                Models.MyAWSConfigs.HistoryDBtableName,
                "id",
                pId
            ));

            List<Document> docs = new List<Document>(result);

            foreach (var doc in docs)
            {
                Console.WriteLine("Detected camera Id" + doc["cameraId"]);

                string location = cameras[doc["cameraId"]].location;
                DateTime localTime = doc["timestamp"].AsDateTime().ToLocalTime();
                string time = $"{localTime.ToShortDateString()}  {localTime.ToShortTimeString()}";

                Label lblTime = new Label() { Content = $"{time}" };
                Label lblLocation = new Label() { Content = $"{location}" };

                                
                Ellipse ellipse = new Ellipse() { Height=50, Width=50,
                    VerticalAlignment =VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Stroke = Brushes.White,
                    Fill = new ImageBrush() { Stretch=Stretch.UniformToFill, ImageSource= new BitmapImage(new Uri("../../Resources/images/CamImage.jpg", UriKind.Relative)) },
                };

                this.timeList.Items.Add(lblTime);
                this.iconList.Items.Add(ellipse);
                this.locationList.Items.Add(lblLocation);
            }
                
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            switch (objectType)
            {
                case "home":
                    mv.MenuPage.Content = parent as HomePageView;
                    mv.lblTitle.Content = "HOME";
                    (parent as HomePageView).GetAllCameras().ConfigureAwait(false);
                    break;
                case "ref":
                    mv.MenuPage.Content = parent as AllPeoplePageView;
                    mv.lblTitle.Content = "REFERENCE";
                    (parent as AllPeoplePageView).LoadPersonsData().ConfigureAwait(false);
                    break;
            }
        }

        public void BackToRef()
        {
            mv.MenuPage.Content = parent as AllPeoplePageView;
            mv.lblTitle.Content = "REFERENCE";
            (parent as AllPeoplePageView).LoadPersonsData().ConfigureAwait(false);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            AddNewRef addNewRef = new AddNewRef(this.pId, this.pName, this.pDescription, this.pImageSource, this, null, false);
            addNewRef.ShowDialog();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            mv.DeleteRefPerson(pId, parent as AllPeoplePageView);
            mv.MenuPage.Content = parent as AllPeoplePageView;
            mv.lblTitle.Content = "REFERENCE";
        }
    }
}
