using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.Interop;
namespace CameraPauseTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           // videoElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[0];
            videoElement.EnableSampleGrabbing = false;
            
           videoElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[0];




          
            


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
          //  videoElement.VideoCaptureDevice = null;
           videoElement.SetNextFileName(@"c:\1.wmv");
           
           videoElement.StartCapture();

            //videoElement.
           
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            videoElement.PauseCapture();
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            videoElement.ResumeCapture();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            videoElement.StopCapture();
        }
    }
}
