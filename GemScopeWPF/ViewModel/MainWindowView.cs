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
        public string CameraStartStoprVideoRecordingButtonText
        {
            get { return _cameraStartStoprVideoRecordingButtonText; }
            set
            {

                _cameraStartStoprVideoRecordingButtonText = value;
                RaisePropertyChanged("CameraStartStoprVideoRecordingButtonText");
            }
        }
        
        
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowView()
        {
            CameraStartStopButtonText = "Start Camera";
            CameraStartStoprVideoRecordingButtonText = "Start Recording";

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
            if (value.IsRecording)
            {
                CameraStartStoprVideoRecordingButtonText = "Pause Recording";
                
            }
            else
            {
                CameraStartStoprVideoRecordingButtonText = "Start Recording";
                
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
