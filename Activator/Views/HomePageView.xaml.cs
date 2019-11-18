using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Amazon.Rekognition;
using Amazon.Rekognition.Model;


namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {

        public HomePageView()
        {
            InitializeComponent();            
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                StreamProcessor sp = Models.Starter.DescribeStreamProcessor();

                if (sp.Status == StreamProcessorStatus.STOPPED)
                {
                    Models.Starter.StartStreamProcessor();
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                StreamProcessor sp = Models.Starter.DescribeStreamProcessor();

                if (sp.Status == StreamProcessorStatus.RUNNING)
                {
                    Models.Starter.StopStreamProcessor();
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                StreamProcessor sp = Models.Starter.DescribeStreamProcessor();

                Console.WriteLine(sp.Name);
                Console.WriteLine(sp.Status);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void CheckAndCreateStreamProcessor()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!Models.Starter.ListStreamProcessors().Contains(Models.MyAWSConfigs.streamProcessorName))
                {
                    Models.Starter.CreateStreamProcessor();
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            CheckAndCreateStreamProcessor();
        }

        private void BtnList_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<string> temp = Models.Starter.ListStreamProcessors();
                foreach (string item in temp)
                {
                    Console.WriteLine(item);
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Models.Starter.DeleteStreamProcessor();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
