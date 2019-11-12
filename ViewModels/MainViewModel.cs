using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Activator.Views;
using System.Windows;

namespace Activator.ViewModels
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainViewModel : Conductor<object>
    {
        public MainViewModel()
        {
            LoginView login = new LoginView();
            login.Close();

            ButtonMenuHome();
        }

        IWindowManager manager = new WindowManager();

        //close button
        public void ButtonCloseApplication()
        {
            manager.ShowWindow(new CloseConfirmViewModel(), null, null);
        }
            
            
            
            //=> Application.Current.Shutdown();

        //visibility of the MENU button
        private Visibility _myPropertyo;
        public Visibility MyPropertyO
        {
            get { return _myPropertyo; }
            set { _myPropertyo = value; NotifyOfPropertyChange(() => MyPropertyO); }
        }

        private Visibility _myPropertyc;
        public Visibility MyPropertyC
        {
            get { return _myPropertyc; }
            set { _myPropertyc = value; NotifyOfPropertyChange(() => MyPropertyC); }
        }

        public void ButtonOpenMenu()
        {
            MyPropertyO = Visibility.Hidden;
            MyPropertyC = Visibility.Visible;
            //buttonFunction(MyPropertyO);
            //ButtonOpenMenu.Visibility = Visibility.Collapsed;
            // ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        public void ButtonCloseMenu()
        {
            MyPropertyC = Visibility.Hidden;
            MyPropertyO = Visibility.Visible;
            //buttonFunction(MyPropertyC);
            //ButtonOpenMenu.Visibility = Visibility.Visible;
            //ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        //navigation through menu
        //navigate to Home Page
        public void ButtonMenuHome()
        {
            this.ActivateItem(new HomePageViewModel());
        }
        //navigate to PeopleIn Page
        public void ButtonMenuPeopleIn()
        {
            this.ActivateItem(new PeopleInPageViewModel());
        }
        //navigate to AllPeople Page
        public void ButtonMenuAllPeople()
        {
            this.ActivateItem(new AllPeoplePageViewModel());
        }
        //navigate to Readers Page
        public void ButtonMenuReaders()
        {
            this.ActivateItem(new ReadersViewModel());
        }
        //navigate to Cameras Page
        public void ButtonMenuCameras()
        {
            this.ActivateItem(new CamerasPageViewModel());
            ButtonOpenMenu();
        }
    }
}
