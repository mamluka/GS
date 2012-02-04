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
using GemScopeWPF.WebcamFacade;
using GemScopeWPF.UI;
using GemScopeWPF.Repository;
using GemScopeWPF.Sharing;
using GemScopeWPF.Utils;
using GemScopeWPF.ViewModel;
using System.Media;
using NLog;
using WPFMediaKit.DirectShow.Controls;

namespace GemScopeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        //private CaptureDirectShowV1 cap;
        
        public MainWindow()
        {

           

            InitializeComponent();

            MainWindowView view = MainWindowView.GetInstrance();

            DataContext = view;

     
           

            
           
          
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                logger.Info("logging v2");
                ActivationManager am = new ActivationManager();
                am.PresistActivation();
                // am.IsExipred();
                logger.Info("get capture instance");
                Capture capture = Capture.GetInstance();
                logger.Info("set the device to the wpf");
                capture.SetCaptureDeviceWPFControl(videoElement);

                //folder view
                logger.Info("open the categories");
                StonesView.ViewControl = ViewControl;
                DirectoryManager dm = DirectoryManager.GetInstance();

                string path = dm.GetHomeFolder();

                FolderBrowser fb = FolderBrowser.GetInstance();
                logger.Info("set working folder");
                fb.SetFolderControl(this.FolderBrowserTree);
                fb.AddItemClickEvents();
                fb.Init(path);

                StonesView.StoneInfoDisplayControl = StoneInfo;

                StonesView.InitStonesView();
                logger.Info("Init stone view");
                StonesView.LoadImagesToImageView(path);
                logger.Info("loading images");
                if (SettingsManager.ReadBoolSetting("RunFirstTime"))
                {
                    //TODO add here things to run the first time
                    //Options options = new Options();
                    //options.Owner = this;
                    //options.Show();

                    SettingsManager.WriteBoolSetting("RunFirstTime", false);

                }



                CaptureMovieProcessFlow flow = CaptureMovieProcessFlow.GetInstance();
                flow.TextBlock = this.MovieFlowTimerText;
                flow.CaptureFacede = capture;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);

                throw;
            }

        }

        private void CaptureNewStone_Click(object sender, RoutedEventArgs e)
        {
           // ActivationManager am = ActivationManager.GetInstance();

           // if (am.ActivationStatus())
          //  {

                NewStone newstone = new NewStone();

                Capture capture = Capture.GetInstance();

            foreach (var videoInputDevice in MultimediaUtil.VideoInputDevices)
            {
                logger.Info(videoInputDevice.DevicePath);
                logger.Info(videoInputDevice.Name);
            }


                newstone.CapturedImage.Source = capture.CaptureImageToImageSource();

                newstone.FolderUponCaptureEvent = StonesView.CurrentPath;

                newstone.Owner = this;

                newstone.Show();
         //   }
           // else
           // {
              //  MessageBox.Show("Only registered users can capture images...");
          //      am.PresistActivation();
          //  }

            

        }

        

       

        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            SharingUtils sharing = new SharingUtils();

            List<string> files = StonesView.GetCurrentSelectedStoneFilenames();
            if (files != null)
            {
                SharingUtils.OpenNewEmailWithOutlook(files);
            }
            else
            {
                MessageBox.Show("No stones selected");
            }
        }

        private void ShowOptionsMenu_Click(object sender, RoutedEventArgs e)
        {
            Options options = new Options();
            options.Owner = this;
            options.Show();
        }

        private void ViewEditMenu_Click(object sender, RoutedEventArgs e)
        {

            StonesView.EditStoneItem();
                
        }
        private void ViewRenameMenu_Click(object sender, RoutedEventArgs e)
        {
            string file = StonesView.GetCurrentSelectedStoneFilename();

            if (!String.IsNullOrEmpty(file))
            {
                StonesView.OpenRenameDialogAndPreformRename(file);
            }
          
        }
        private void ViewDeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            string file = StonesView.GetCurrentSelectedStoneFilename();
            //TODO add the stone file name
            if (!String.IsNullOrEmpty(file) && ( MessageBox.Show("Are you sure you want to delete this stone","Delete",MessageBoxButton.YesNo) == MessageBoxResult.Yes ) )
            {
                StonesView.PreformDeleteOfStone(file);
            }
        }
        private void ViewSaveAsMenu_Click(object sender, RoutedEventArgs e)
        {
            string file = StonesView.GetCurrentSelectedStoneFilename();
            if (!String.IsNullOrEmpty(file))
            {
                StonesView.PreformSaveAsOfStone(file);  
            }
            

        }
        private void ViewEmailMenu_Click(object sender, RoutedEventArgs e)
        {
            SharingUtils sharing = new SharingUtils();

            List<string> files = StonesView.GetCurrentSelectedStoneFilenames();
            if (files != null)
            {
                SharingUtils.OpenNewEmailWithOutlook(files);
            }
            else
            {
                MessageBox.Show("No stones selected");
            }
        }
        private void ViewPrintMenu_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Not worknig yet...");
            if (StonesView.GetCurrentSelectedStones().Count > 0)
            {
                Print print = new Print();

                print.ShowDialog();
            }
            else
            {
                MessageBox.Show("No stones selected");
            }
            


        }

        private void CreateSubFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowser fb = FolderBrowser.GetInstance();
            fb.CreateNewSubFolderUnderCurrentFolder();


        }
        private void OpenSkype_Click(object sender, RoutedEventArgs e)
        {
            SharingUtils.OpenSkype();


        }

        private void StartStopCamera_Click(object sender, RoutedEventArgs e)
        {
            //WebCam webcam = WebCam.GetInstance();
            Capture capture = Capture.GetInstance();
            //videoElement.IsPlaying
            if (capture.IsPlaying)
            {
                capture.Stop();
            }
            else 
            {
                capture.Play();
            }
        }

        private void CaptureMovie_Click(object sender, RoutedEventArgs e)
        {

            //Capture capture = Capture.GetInstance();

            //if (capture.IsCapturingVideo)
            //{
            //    capture.StopCapturingVideoToFile();
            //}
            //else
            //{
            //    var filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), new Random().Next().ToString() + ".wmv");
            //    capture.StartCapturingVideoToFile(filename);
            ////}

            //var filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), new Random().Next().ToString() + ".wmv");
            //Capture capture = Capture.GetInstance();
            //capture.StartCapturingVideoToFile(filename);

          //  ActivationManager am = ActivationManager.GetInstance();

           // if (am.ActivationStatus())
          //  {

                CaptureMovieProcessFlow flow = CaptureMovieProcessFlow.GetInstance();

                flow.ToggleStartPause();
          //  }
         //   else
         //   {
       //         MessageBox.Show("Only registered users can record video");
        //        am.PresistActivation();
          //  }



           

        }

        

        private void ViewOrderBy(object sender, RoutedEventArgs e)
        {
            //TODO fix the radio button beheivier
            string tag = (string)((MenuItem)sender).Tag;

            switch (tag)
            {
                case "name":
                    StonesView.SortProperty = 1;
                    break;
                case "date":
                    StonesView.SortProperty = 2;
                    break;
                case "type":
                    StonesView.SortProperty = 3;
                    break;
                case "size":
                    StonesView.SortProperty = 4;
                    break;
                case "weight":
                    StonesView.SortProperty = 5;
                    break;
                default:
                    break;
            }

            StonesView.RefreshView();


        }

        private void ViewOrderByDirection(object sender, RoutedEventArgs e)
        {
            string tag = (string)((MenuItem)sender).Tag;
            if (tag == "asc")
            {
                StonesView.SortPropertyDirection = 1;
            }
            else
            {
                StonesView.SortPropertyDirection = 2;
            }

            StonesView.RefreshView();
        }

        private void CaptureMovieCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Are you sure you want to cancel current recording","Recording",MessageBoxButton.YesNo))
            {
                CaptureMovieProcessFlow flow = CaptureMovieProcessFlow.GetInstance();
                flow.Cancel();
                
            }
        }

        private void CaptureMovieEnd_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Are you sure you want to end the current recording before hand", "Recording", MessageBoxButton.YesNo))
            {
                CaptureMovieProcessFlow flow = CaptureMovieProcessFlow.GetInstance();
                flow.End();

            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            License lic = new License();
            lic.ShowDialog();
        }

        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void Licence_Click(object sender, RoutedEventArgs e)
        {
            License lic = new License();
            lic.ShowDialog();
        }

        private void CreatePDF_Click(object sender, RoutedEventArgs e)
        {
            PDFExport pdf = new PDFExport();

            Stone stone = StonesView.GetCurrentSelectedStone();

            if (stone.MediaType == 2)
            {
                MessageBox.Show("You can't create PDF for a video");

                return;
            }

            if (stone != null) 
            {
                pdf.CreatePDFFile(stone);
            }
       


        }

        private void EmailPDF_Click(object sender, RoutedEventArgs e)
        {
            PDFExport pdf = new PDFExport();
            string filename;

            Stone stone = StonesView.GetCurrentSelectedStone();

            if (stone.MediaType == 2)
            {
                MessageBox.Show("You can't create PDF for a video");

                return;
            }

            if (stone != null)
            {
                filename = pdf.CreateRawPDFinTemp(stone);
                if (!String.IsNullOrWhiteSpace(filename))
                {
                    List<string> files = new List<string>();
                    files.Add(stone.FullFilePath);
                    files.Add(filename);
                    SharingUtils.OpenNewEmailWithOutlook(files);
                }
            }


        }

        


        private void Adjustments_Click(object sender, RoutedEventArgs e)
        {
            var capture = Capture.GetInstance();
            capture.Format();
        }
    }


    

}
