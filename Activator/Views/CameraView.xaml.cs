﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        public CameraView()
        {
            InitializeComponent();
            //Models.StreamManager.DeleteStreamProcessor("indoorcam_processor");
        }

        private void BtnAddNewCamera_Click(object sender, RoutedEventArgs e)
        {
            AddNewCameraView addNewCameraView = new AddNewCameraView();
            addNewCameraView.Show();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStopSP(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStartSP(object sender, RoutedEventArgs e)
        {

        }
    }
}
