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
            LoginView logwin = new LoginView();
            logwin.Close();

            ButtonMenuHome();
        }

        public void ButtonMenuHome()
        {
            this.ActivateItem(new HomePageViewModel());
        }

        //close button
        public void ButtonCloseApplication() => Application.Current.Shutdown();

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

        //navigate to cameras page
        public void ButtonMenuCameras()
        {
            Console.WriteLine("EVGWEGWEGWEG");
            this.ActivateItem(new CamerasPageViewModel());
        }
    }
}
