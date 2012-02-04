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
using System.Windows.Shapes;

namespace GemScopeWPF
{
    /// <summary>
    /// Interaction logic for VideoCapture.xaml
    /// </summary>
    public partial class VideoCapture : Window
    {
        public VideoCapture()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //videoElement2.file
            //videoElement2.VideoCaptureDevice = WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputDevices[1];
        }
    }
}
