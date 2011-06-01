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
using System.IO;
using GemScopeWPF.Repository;
using GemScopeWPF.UI;
using GemScopeWPF.WebcamFacade;
using System.Text.RegularExpressions;
using CAVEditLib;

namespace GemScopeWPF
{
    /// <summary>
    /// Interaction logic for NewStomeMovie.xaml
    /// </summary>
    public partial class NewStomeMovie : Window
    {

        public string TempFileName { get; set; }
        public bool EditMode { get; set; }
        public Stone CurrentStone { get; set; }
        public string FolderUponCaptureEvent { get; set; }
        public NewStomeMovie()
        {
            InitializeComponent();
            EditMode = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (CurrentStone != null)
            {
                foreach (StackPanel panel in this.InfoPartsInputsContainer.Children)
                {

                    if (panel.Children.OfType<TextBox>().SingleOrDefault() != null)
                    {
                        string tag = (string)panel.Children.OfType<TextBox>().SingleOrDefault().Tag;
                        var value = CurrentStone.InfoList.Where(m => m.Title == tag).SingleOrDefault();
                        if (value != null)
                        {
                            panel.Children.OfType<TextBox>().SingleOrDefault().Text = value.Value;
                        }

                    }
                    else if (panel.Children.OfType<ComboBox>().SingleOrDefault() != null)
                    {
                        string tag = (string)panel.Children.OfType<ComboBox>().SingleOrDefault().Tag;
                        var value = CurrentStone.InfoList.Where(m => m.Title == tag).SingleOrDefault();

                        if (value != null)
                        {
                            ComboBox combo = panel.Children.OfType<ComboBox>().SingleOrDefault();

                            combo.SelectedItem = combo.Items.OfType<ComboBoxItem>().Where(m => (string)m.Tag == value.Value).Single();
                            //  = combo.Items.Count - 1;
                            //  combo.Items.OfType<ComboBoxItem>().Where(m => m.Tag == value.Value).Single().IsSelected = true;
                            //  combo.SelectedIndex = combo.Items.IndexOf(combo.Items.OfType<ComboBoxItem>().Where(m => m.Tag == value.Value).Single());
                        }



                    }

                    // string tag = (string)panel.Children.OfType<TextBox>().SingleOrDefault().Tag;

                    //  var value = CurrentStone.InfoList.Where(m=> m.Title == tag).SingleOrDefault() ;

                    //  if (value != null)
                    // {
                    //     panel.Children.OfType<TextBox>().SingleOrDefault().Text = value.Value;
                    //}




                }

                //load iamge

                this.mediaPlayer.Source = new Uri(CurrentStone.FullFilePath, UriKind.Absolute);
                this.mediaPlayer.LoadedBehavior = WPFMediaKit.DirectShow.MediaPlayers.MediaState.Play;
              


            }
            else
            {
                this.mediaPlayer.Source = new Uri(this.TempFileName, UriKind.Absolute);
                this.mediaPlayer.LoadedBehavior = WPFMediaKit.DirectShow.MediaPlayers.MediaState.Play;
            }



        }
        private const string CAVEDIT_LIB_NAME = "CAVEditLib.dll";
        private string mLogPath = "C:\\Log\\Log.txt";
        private const string LIBAV_PATH = "\\LibAV";

        private void SaveDiamond_Click(object sender, RoutedEventArgs e)
        {
            StonesRepository rep = new StonesRepository(FolderUponCaptureEvent);

            //Create the infoparts from input controls

            if (this.ValidateUserInput() == false)
            {
                MessageBox.Show("Filename,Stonetype,Stone Weight,Clarity and color are mendatory fields...");
                return;
            }

            string filename = Path.Combine(FolderUponCaptureEvent, this.Filename.Text);
            filename = Path.ChangeExtension(filename, "mp4");

            if (rep.IsStoneExists(filename) && this.EditMode == false)
            {
                MessageBoxResult result = MessageBox.Show("The filename already exists, do you want to override the existing file", "File exists", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (!String.IsNullOrWhiteSpace(TempFileName))
            {
                ICAVConverter converter = new CAVConverter();

                converter.SetLicenseKey("Demo", "Demo");

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                string libPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + CAVEDIT_LIB_NAME;
                string regsvr32Path = Environment.SystemDirectory + "\\regsvr32.exe";

                startInfo.Arguments = "\"" + libPath + "\"" + " /s";
                startInfo.FileName = regsvr32Path;

                System.Diagnostics.Process regProcess = System.Diagnostics.Process.Start(startInfo);
                regProcess.WaitForExit();

                converter.LoadAVLib(AppDomain.CurrentDomain.BaseDirectory + LIBAV_PATH);

                converter.OutputOptions = new COutputOptions();

                converter.OutputOptions.FrameSize = "640x480";
                //Set output video bitrate
                converter.OutputOptions.VideoBitrate = 400000;



                // converter//.LogPath = mLogPath;

                //  converter.LogLevel = CLogLevel.cllInfo;

                converter.AddTask(TempFileName, filename);

                converter.StartAndWait();
                
               // File.Move(TempFileName, filename);
            }
            

            List<StoneInfoPart> infoparts = new List<StoneInfoPart>();

            foreach (StackPanel panel in this.InfoPartsInputsContainer.Children)
            {
                StoneInfoPart infopart = new StoneInfoPart();
                if (panel.Children.OfType<TextBox>().SingleOrDefault() != null)
                {
                    infopart.Title = (string)panel.Children.OfType<TextBox>().SingleOrDefault().Tag;
                    infopart.Value = panel.Children.OfType<TextBox>().SingleOrDefault().Text;
                }
                else if (panel.Children.OfType<ComboBox>().SingleOrDefault() != null)
                {
                    infopart.Title = (string)panel.Children.OfType<ComboBox>().SingleOrDefault().Tag;
                    infopart.Value = (string)((ComboBoxItem)panel.Children.OfType<ComboBox>().SingleOrDefault().SelectedItem).Tag;
                }


                infoparts.Add(infopart);

            }
            
            if (this.EditMode)
            {
                rep.UpdateStone(filename, infoparts);
            }
            else
            {
                rep.CreateANewStoneMovie(filename, infoparts);
            }

            //WebCam webcam = WebCam.GetInstance();
            // webcam.Start();

            StonesView.RefreshView();

            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.Stop();
            if (!String.IsNullOrWhiteSpace(this.TempFileName))
            {
                File.Delete(this.TempFileName);
            }
            
            this.Close();
        }

        private bool ValidateUserInput()
        {
            bool isValid = true;

            string filename = this.Filename.Text;
            int istonetype = this.StoneType.SelectedIndex;

            string caratweight = this.CaratWeight.Text;

            int icolor = this.StoneColor.SelectedIndex;
            int iclarity = this.StoneClarity.SelectedIndex;

            //filename required

            if (String.IsNullOrWhiteSpace(filename))
            {
                isValid = false;
            }

            Regex r = new Regex(@"[^/?*:;{}\\]+");
            isValid = r.IsMatch(filename);

            if (istonetype == 1)
            {
                isValid = false;
            }

            r = new Regex(@"^\d{0,8}(\.\d{1,8})?$");
            isValid = r.IsMatch(caratweight);

            if (icolor == 0)
            {
                isValid = false;
            }

            if (iclarity == 0)
            {
                isValid = false;
            }

            return isValid;


        }
    }
}
