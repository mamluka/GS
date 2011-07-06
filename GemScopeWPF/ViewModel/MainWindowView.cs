using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using GemScopeWPF.UI;
using System.Windows;
namespace GemScopeWPF.ViewModel
{
    public class MainWindowView:INotifyPropertyChanged,IObserverCaptureFlow<CaptureMovieProcessFlow>      
    {

        public static MainWindowView Singelton { get; set; }

        private string _cameraStartStopButtonText;
        private string _cameraStartStoprVideoRecordingButtonText;

        private bool _cameraControlButtonIsEnabled;

        private Visibility _capturebuttonsvis;
        private string _cameraStartStopButtonIcon;
        private string _cameraStartStoprVideoRecordingButtonIcon;

        public Visibility CaptureButtonsVisibility
        {
            get { return _capturebuttonsvis; }
            set { 
                _capturebuttonsvis = value;
                RaisePropertyChanged("CaptureButtonsVisibility");
            
            }
        }

        public bool CameraControlButtonIsEnabled
        {
            get { return _cameraControlButtonIsEnabled; }
            set
            {
                _cameraControlButtonIsEnabled = value;
                RaisePropertyChanged("CameraControlButtonIsEnabled");
            }
        }

        public string CameraStartStopButtonText
        {
            get { return _cameraStartStopButtonText; }
            set { 

                _cameraStartStopButtonText = value;
                RaisePropertyChanged("CameraStartStopButtonText");
            }
        }

        public string CameraStartStopButtonIcon
        {
            get { return _cameraStartStopButtonIcon; }
            set
            {

                _cameraStartStopButtonIcon = value;
                RaisePropertyChanged("CameraStartStopButtonIcon");
            }
        }
        public string CameraStartStoprVideoRecordingButtonText
        {
            get { return _cameraStartStoprVideoRecordingButtonText; }
            set
            {

                _cameraStartStoprVideoRecordingButtonText = value;
                RaisePropertyChanged("CameraStartStoprVideoRecordingButtonText");
            }
        }

        public string CameraStartStoprVideoRecordingButtonIcon
        {
            get { return _cameraStartStoprVideoRecordingButtonIcon; }
            set
            {

                _cameraStartStoprVideoRecordingButtonIcon = value;
                RaisePropertyChanged("CameraStartStoprVideoRecordingButtonIcon");
            }
        }
        
        
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowView()
        {
            CameraStartStopButtonText = "Start Camera";
            CameraStartStoprVideoRecordingButtonText = "Start\nRecording";

            CameraStartStopButtonIcon = "/GemScopeWPF;component/Media/Icons/freeze.png";
            CameraStartStoprVideoRecordingButtonIcon = "/GemScopeWPF;component/Media/Icons/start.png";

            CaptureButtonsVisibility = Visibility.Hidden;

            CaptureMovieProcessFlow flow = CaptureMovieProcessFlow.GetInstance();

            CameraControlButtonIsEnabled = true;

            flow.Subscribe(this);

           
        }



        public static MainWindowView GetInstrance() {
            if (Singelton == null ) {
                Singelton = new MainWindowView();
                return Singelton;
            } else {
                return Singelton;
            }
        }

        void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }





        public void CameraStateChange(CaptureMovieProcessFlow value)
        {
            if ((value.State == CaptureMovieProcessFlowStates.Recording || value.State == CaptureMovieProcessFlowStates.CountingDownTillRecording))
            {
                CameraStartStoprVideoRecordingButtonText = "Pause\nRecording";
                CameraStartStoprVideoRecordingButtonIcon = "/GemScopeWPF;component/Media/Icons/pause.png";

            }
            else if ((value.State == CaptureMovieProcessFlowStates.PausedRecording || value.State == CaptureMovieProcessFlowStates.PausedCountingDownTillRecording))
            {
                CameraStartStoprVideoRecordingButtonText = "Continue\nRecording";
                CameraStartStoprVideoRecordingButtonIcon = "/GemScopeWPF;component/Media/Icons/start.png";
            }
            else
            {
                CameraStartStoprVideoRecordingButtonText = "Start\nRecording";
                CameraStartStoprVideoRecordingButtonIcon = "/GemScopeWPF;component/Media/Icons/start.png";

            }
           
            if (value.State == CaptureMovieProcessFlowStates.Stop)
            {
                this.CaptureButtonsVisibility = Visibility.Hidden;
                CameraControlButtonIsEnabled = true;
            }
            else
            {
                this.CaptureButtonsVisibility = Visibility.Visible;
                CameraControlButtonIsEnabled = false;
            }
        }
    }
}
