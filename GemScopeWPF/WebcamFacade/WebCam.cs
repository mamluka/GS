using System;
using System.IO;
using System.Linq;
using System.Text;
using WebCam_Capture;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using GemScopeWPF.ViewModel;


namespace GemScopeWPF.WebcamFacade
{
    //Design by Pongsakorn Poosankam
    public class WebCam
    {
        private WebCamCapture webcam;
        private System.Windows.Controls.Image _FrameImage;
        private int FrameNumber = 30;
        public bool WebCamOpen { get; set; }

        public static WebCam Singelton { get; set; }

        public static WebCam GetInstance() 
        {
            if (Singelton == null)
            {
                Singelton = new WebCam();
                return Singelton;
            } 
            else {
                return Singelton;
            }
        }

        public void InitializeWebCam(ref System.Windows.Controls.Image ImageControl)
        {
            webcam = new WebCamCapture();
            webcam.FrameNumber = ((ulong)(0ul));
            webcam.TimeToCapture_milliseconds = FrameNumber;
            
            //webcam.DrawToBitmap(
            webcam.ImageCaptured += new WebCamCapture.WebCamEventHandler(webcam_ImageCaptured);
            _FrameImage = ImageControl;

            
        }

        void webcam_ImageCaptured(object source, WebcamEventArgs e)
        {
            _FrameImage.Source = Helper.LoadBitmap((System.Drawing.Bitmap)e.WebCamImage);
        }

        public void Start()
        {
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.Start(0);
            MainWindowView view = MainWindowView.GetInstrance();
            view.CameraStartStopButtonText = "Stop";
            WebCamOpen = true;
           
            
        }

        public void Stop()
        {
            //Decouple this using events and observer pattern
            webcam.Stop();
            MainWindowView view = MainWindowView.GetInstrance();
            view.CameraStartStopButtonText = "Start";
            WebCamOpen = false;
        }

        public void Continue()
        {
            // change the capture time frame
            webcam.TimeToCapture_milliseconds = FrameNumber;

            // resume the video capture from the stop
            webcam.Start(this.webcam.FrameNumber);
        }

        public void ResolutionSetting()
        {
            webcam.Config();
        }

        public void AdvanceSetting()
        {
            webcam.Config2();
        }
        public Image GetTheCurrentImageFromCamera()
        {
            Image image = new Image();
            image = _FrameImage;

            return image;
            
        }
        


    }
}
