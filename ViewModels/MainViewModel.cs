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
    public partial class MainViewModel : Screen
    {
        public MainViewModel()
        {
            HomePageView setpage = new HomePageView();
            CurrentPage = setpage;
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

        //set Home Page to frame
        static readonly HomePageView setpage = new HomePageView();
        private Page _currentpage = setpage;

        //show Home Page when no menu is selected
        public Page CurrentPage
        {
            get
            {
                return _currentpage;
            }
            set
            {
                _currentpage = value;
                NotifyOfPropertyChange(() => CurrentPage);
            }
        }

        //navigate to cameras page
        public void ButtonMenuCameras()
        {
            //Console.WriteLine("EVGWEGWEGWEG");
            CamerasPageView camp = new CamerasPageView();
            CurrentPage.Content = camp;
        }
    }
}
