using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GemScopeWPF.WebcamFacade;
using System.Windows.Threading;
using System.Media;
using System.IO;
namespace GemScopeWPF.UI
{
    public class CaptureMovieProcessFlow
    {
        private static CaptureMovieProcessFlow Singelton { get; set; }
        public TextBlock TextBlock { get; set; }
        public Capture CaptureFacede { get; set; }
        public string TempFilename { get; set; }

        private CaptureMovieProcessFlowStates _state;

        public CaptureMovieProcessFlowStates State {
            get { return _state; }
            set
            {
                _state = value;
                this.Notify(this);
            }
        }
        public CaptureMovieProcessFlowStage Stage { get; set; }

        private bool _isRecording;

        public bool IsRecording
        {
            get { return _isRecording; }
            set {
                _isRecording = value;
                this.Notify(this);
            }
        }

        private bool _isAfterInitialCount;

        public bool IsAfterInitialCount
        {
            get { return _isAfterInitialCount; }
            set { _isAfterInitialCount = value; }
        }
        
        


        public DispatcherTimer RecordingTimer { get; set; }
        public DispatcherTimer PreRecordingTimer { get; set; }

        private int Seconds { get; set; }

        private List<IObserverCaptureFlow<CaptureMovieProcessFlow>> observers;

        public static CaptureMovieProcessFlow GetInstance()
        {
            if (Singelton == null)
            {
                Singelton = new CaptureMovieProcessFlow();
                Singelton.IsRecording = false;
                return Singelton;
            }
            else
            {
                return Singelton;
            }
        }

        #region Observable Methods

        protected void Notify(CaptureMovieProcessFlow obj)
        {
            foreach (IObserverCaptureFlow<CaptureMovieProcessFlow> observer in observers)
            {
                observer.CameraStateChange(obj);
            }
        }

        public IDisposable Subscribe(IObserverCaptureFlow<CaptureMovieProcessFlow> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserverCaptureFlow<CaptureMovieProcessFlow>> observers;
            private IObserverCaptureFlow<CaptureMovieProcessFlow>observer;

            public Unsubscriber(List<IObserverCaptureFlow<CaptureMovieProcessFlow>> observers, IObserverCaptureFlow<CaptureMovieProcessFlow> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                if (observer != null && observers.Contains(observer))
                {
                    observers.Remove(observer);
                }
            }
        }
        #endregion

        public CaptureMovieProcessFlow()
        {
            Seconds = 0;
           // RecordingTimer = new DispatcherTimer();
            //PreRecordingTimer = new DispatcherTimer();
            observers = new List<IObserverCaptureFlow<CaptureMovieProcessFlow>>();
            State = CaptureMovieProcessFlowStates.Stop;
            Stage = CaptureMovieProcessFlowStage.Stopped;
        }

        public void Cancel()
        {
            if (Stage == CaptureMovieProcessFlowStage.Recording)
            {
                Stage = CaptureMovieProcessFlowStage.Stopped;
                State = CaptureMovieProcessFlowStates.Stop;

                PreRecordingTimer.Stop(); ;
                RecordingTimer.Stop();

                PreRecordingTimer = null;
                RecordingTimer = null;

                CaptureFacede.StopCapturingVideoToFile();

                //TODO handle files left after cancel
                //File.Delete(TempFilename);

                TextBlock.Visibility = System.Windows.Visibility.Collapsed;

                IsRecording = false;
            }
            else
            {
                this.Clean();
            }


        }
        public void ToggleStartPause()
        {
            if (Stage == CaptureMovieProcessFlowStage.Stopped || Stage == CaptureMovieProcessFlowStage.CountingDownTillRecording)
            {
                if (PreRecordingTimer == null)
                {
                    PreRecordingTimer = new DispatcherTimer();
                    PreRecordingTimer.Tick += new EventHandler(StartMovieDelay_Timer);
                    PreRecordingTimer.Interval = new TimeSpan(0, 0, 1);
                    PreRecordingTimer.Start();
                    State = CaptureMovieProcessFlowStates.CountingDownTillRecording;
                    Stage = CaptureMovieProcessFlowStage.CountingDownTillRecording;
                    Seconds = 3;
                    TextBlock.Text = "You have " + Seconds.ToString() + " seconds before recording starts...";
                    TextBlock.Visibility = System.Windows.Visibility.Visible;
                    IsRecording = true;
                }
                else
                {
                    if (PreRecordingTimer.IsEnabled)
                    {
                        PreRecordingTimer.Stop();
                        IsRecording = false;
                        State = CaptureMovieProcessFlowStates.PausedCountingDownTillRecording;
                    }
                    else
                    {
                        PreRecordingTimer.Start();
                        State = CaptureMovieProcessFlowStates.CountingDownTillRecording;
                        IsRecording = true;
                    }

                }
            }
            else
            {
                if (RecordingTimer != null)
                {
                    if (RecordingTimer.IsEnabled)
                    {
                        RecordingTimer.Stop();
                        State = CaptureMovieProcessFlowStates.PausedRecording;
                        CaptureFacede.PauseCapturingVideoToFile();
                        IsRecording = false;
                    }
                    else
                    {
                        RecordingTimer.Start();
                        State = CaptureMovieProcessFlowStates.Recording;
                        CaptureFacede.ResumeCapturingVideoToFile();
                        IsRecording = true;
                    }

                }
            }

        }
        public void End()
        {
            if (Stage == CaptureMovieProcessFlowStage.Recording)
            {
                Stage = CaptureMovieProcessFlowStage.Stopped;
                State = CaptureMovieProcessFlowStates.Stop;

                PreRecordingTimer.Stop(); ;
                RecordingTimer.Stop();

                PreRecordingTimer = null;
                RecordingTimer = null;

                this.StopRecording();
            }
            else
            {
                this.Clean();
            }


        }
        public void Clean()
        {
            if (PreRecordingTimer != null)
            {
                PreRecordingTimer.Stop();
                PreRecordingTimer = null;
            }

            if (RecordingTimer != null)
            {
                RecordingTimer.Stop();
                RecordingTimer = null;
            }

            Stage = CaptureMovieProcessFlowStage.Stopped;
            State = CaptureMovieProcessFlowStates.Stop;

            TextBlock.Visibility = Visibility.Collapsed;

            this.IsRecording = false;

         }
        public void StartMovieTimeCountDown()
        {

                RecordingTimer = new DispatcherTimer();
                RecordingTimer.Tick += new EventHandler(MovieCountDown_Timer);
                RecordingTimer.Interval = new TimeSpan(0, 0, 1);
                RecordingTimer.Start();
                State = CaptureMovieProcessFlowStates.Recording;
                Stage = CaptureMovieProcessFlowStage.Recording;
                var filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), new Random().Next().ToString() + ".avi");

                CaptureFacede.StartCapturingVideoToFile(filename);

                this.IsRecording = true;
               

                TempFilename = filename;

            

            

        }
        public void PauseRecording()
        {
            RecordingTimer.Stop();
            CaptureFacede.PauseCapturingVideoToFile();

            this.IsRecording = false;
            State = CaptureMovieProcessFlowStates.PausedRecording;
            Stage = CaptureMovieProcessFlowStage.Stopped;
            
        }
        public void StopRecording()
        {
            CaptureFacede.StopCapturingVideoToFile();
            State = CaptureMovieProcessFlowStates.Stop;
            Stage = CaptureMovieProcessFlowStage.Stopped;

            PreRecordingTimer = null;
            RecordingTimer = null;

            IsRecording = false;

            TextBlock.Visibility = System.Windows.Visibility.Collapsed;
            NewStomeMovie window = new NewStomeMovie();
            window.TempFileName = TempFilename;
            window.FolderUponCaptureEvent =  StonesView.CurrentPath;
            window.ShowDialog();
        }
        void StartMovieDelay_Timer(object sender, EventArgs e)
        {
            if (Seconds > 0)
            {
                TextBlock.Text = "You have " + Seconds.ToString() + " seconds before recording starts...";
                Seconds--;
                SystemSounds.Beep.Play();

            }
            else
            {
                
                ((DispatcherTimer)sender).Stop();
               
                Seconds = 30;

                this.StartMovieTimeCountDown();
            }
        }

        void MovieCountDown_Timer(object sender, EventArgs e)
        {
            if (Seconds > 0)
            {
                TextBlock.Text = "You have " + Seconds.ToString() + " left to record...";
                Seconds--;

            }
            else
            {
                Seconds = 0;
                ((DispatcherTimer)sender).Stop();
                this.StopRecording();
            }
        }

       
    }
    public enum CaptureMovieProcessFlowStates
    {
        CountingDownTillRecording=1,Recording=2,PausedCountingDownTillRecording=3,PausedRecording=4,Stop=5
    }
    public enum CaptureMovieProcessFlowStage
    {
        CountingDownTillRecording = 1, Recording = 2,Stopped=3
    }
}
