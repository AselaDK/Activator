using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


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

        public DetectedPerson()
        {
            InitializeComponent();
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

            switch (objectType)
            {
                case "home":
                    title.Content = $"DETECTED PERSON: {pName}";
                    break;
                case "ref":
                    title.Content = $"PERSON: {pName}";
                    break;
            }

            personName.Text = pName;
            personDescription.Text = pDescription;
            personImage.Source = pImageSource;

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
                Console.WriteLine("Detected camera Id" + doc["cameraId"]);
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
                    mv.lblTitle.Content = "ALL PEOPLE";
                    (parent as AllPeoplePageView).LoadPersonsData().ConfigureAwait(false);
                    break;
            }

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            mv.MenuPage.Content = parent as AllPeoplePageView;
            mv.lblTitle.Content = "ALL PEOPLE";
            (parent as AllPeoplePageView).RefPersonEdit();

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            mv.MenuPage.Content = parent as AllPeoplePageView;
            mv.lblTitle.Content = "ALL PEOPLE";
            (parent as AllPeoplePageView).DeleteRef();
        }
    }
}
