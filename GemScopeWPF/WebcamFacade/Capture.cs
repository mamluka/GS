using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.Interop;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using GemScopeWPF.ViewModel;
using GemScopeWPF.Utils;
namespace GemScopeWPF.WebcamFacade
{
    /// <summary>
    /// For now we are using the mediakit
    /// </summary>
    public class Capture
    {
        public static Capture Singelton { get; set; }
        private VideoCaptureElement Control { get; set; }

        public bool IsCapturingVideo { get; set; }

        public bool IsPlaying
        {
            get {
                return Control.IsPlaying;
            
            }
        }
        
        public static Capture GetInstance()
        {
            if (Singelton == null)
            {
                Singelton = new Capture();
                return Singelton;
            }
            else
            {
                return Singelton;
            }
        }

        public Capture()
        {
            //TODO move all the view changes to deligation in the near future before release
            IsCapturingVideo = false;
        }
        public void SetCaptureDeviceWPFControl(VideoCaptureElement control)
        {
            this.Control = control;
            this.Control.VideoCaptureDevice = this.GetCaptureDevice();
            this.Control.Volume = 0;

            MainWindowView view = MainWindowView.GetInstrance();
            view.CameraStartStopButtonText = "Freeze";
        }
        public DsDevice GetCaptureDevice()
        {

            if (SettingsManager.ReadBoolSetting("RunFirstTime"))
            {
                 SettingsManager.UpdateSetting("CaptureDeviceName", MultimediaUtil.VideoInputDevices[0].Name);
                return MultimediaUtil.VideoInputDevices[0];
            }
            else
            {
                
                string deviceid = SettingsManager.ReadSetting("CaptureDeviceName");

                if (String.IsNullOrWhiteSpace(deviceid))
                {

                    SettingsManager.UpdateSetting("CaptureDeviceName", MultimediaUtil.VideoInputDevices[0].Name);
                    return MultimediaUtil.VideoInputDevices[0];
                }
                else
                {
                    DsDevice device = MultimediaUtil.VideoInputDevices.Where(m => m.Name == deviceid).SingleOrDefault();
                    if (device != null)
                    {
                        return device;
                    }
                    else
                    {
                        if (MultimediaUtil.VideoInputDevices.Length > 0)
                        {
                            return MultimediaUtil.VideoInputDevices[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }



        }
        public void SetCaptureDevice(DsDevice device)
        {
            this.Control.VideoCaptureDevice = device;
            SettingsManager.UpdateSetting("CaptureDeviceName", device.Name);
        }
        public ImageSource CaptureImageToImageSource()
        {
            this.Control.EnableSampleGrabbing = true;

            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();
            VisualBrush elementBrush = new VisualBrush(Control);
            int w = 640;
            int h = 480;
            context.DrawRectangle(elementBrush, null, new Rect(0, 0, w, h));
            context.Close();
            RenderTargetBitmap bitmap = new RenderTargetBitmap(w, h, 0, 0, PixelFormats.Default);
            bitmap.Render(visual);
            BitmapSource bitmapSource = bitmap;

            return bitmapSource;
        }


        public void StartCapturingVideoToFile(string filename)
        {
            Control.Stop();
            Control.VideoCaptureDevice = null;
            Control.OutputFileName = filename;

            Control.VideoCaptureDevice = this.GetCaptureDevice();
            IsCapturingVideo = true;
            //Control.Pause();

            MainWindowView view = MainWindowView.GetInstrance();
            view.CameraStartStoprVideoRecordingButtonText = "Stop Recording Video";
           
           


        }
        public void StopCapturingVideoToFile()
        {
            Control.Stop();
            Control.VideoCaptureDevice = null;
            Control.OutputFileName = String.Empty;
            Control.VideoCaptureDevice = this.GetCaptureDevice();
            IsCapturingVideo = false;

            MainWindowView view = MainWindowView.GetInstrance();
            view.CameraStartStoprVideoRecordingButtonText = "Start Recording Video";
           
            //Control.Pause();


        }
        public void Stop()
        {
            Control.Stop();
            MainWindowView view = MainWindowView.GetInstrance();
            view.CameraStartStopButtonText = "Start";
            
        }
        public void Play()
        {
            Control.Play();
            MainWindowView view = MainWindowView.GetInstrance();
            view.CameraStartStopButtonText = "Freeze";
        }
        public void Pause()
        {
            Control.Pause();
        }

        public void Format()
        {
            Control.ShowPropertyPage();
        }
        

        
    }
}
