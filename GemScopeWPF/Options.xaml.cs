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
using GemScopeWPF.Utils;
using GemScopeWPF.UI;
using GemScopeWPF.WebcamFacade;
using WPFMediaKit.DirectShow.Controls;
namespace GemScopeWPF
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();

        }

        public Options(int activetab)
        {
            InitializeComponent();

            this.tabControl.SelectedIndex = activetab;

           




        }

        private void OpenFolderDialogBox_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.txt_repositoryfolder.Text = dialog.SelectedPath;
            }
            

        }

        private void SaveOptions_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (Directory.Exists(this.txt_repositoryfolder.Text))
                {
                    DirectoryManager dm = DirectoryManager.GetInstance();
                    dm.SaveHomeFolder(this.txt_repositoryfolder.Text);

                    FolderBrowser fb = FolderBrowser.GetInstance();
                    fb.ChangeHomeFolder(this.txt_repositoryfolder.Text);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid path");
                }

                if (File.Exists(txt_imageeditor.Text))
                {
                    SettingsManager.UpdateSetting("ImageEditorPath", txt_imageeditor.Text);
                }
                else
                {
                    MessageBox.Show("Invalid editor path");
                }

                //devices

                int deviceid = this.DeviceCombo.SelectedIndex;

                Capture capture = Capture.GetInstance();

                if (deviceid >= 0)
                {

                    if (MultimediaUtil.VideoInputDevices[deviceid] != null)
                    {

                        capture.SetCaptureDevice(MultimediaUtil.VideoInputDevices[deviceid]);
                    }
                }
                //editor
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                throw;
            }
            

            
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryManager dm = DirectoryManager.GetInstance();
            this.txt_repositoryfolder.Text = dm.GetHomeFolderFromSettings();

            //test commit
            string device = SettingsManager.ReadSetting("CaptureDeviceName");

            DeviceCombo.SelectedIndex = MultimediaUtil.VideoInputDevices.ToList().FindIndex(m => m.Name == device);

            string editorPath = SettingsManager.ReadSetting("ImageEditorPath");

            if (String.IsNullOrWhiteSpace(editorPath))
            {
                var extraManager = new ExtraManager();
                editorPath = extraManager.GetDefaultImageEditor();
            }

            txt_imageeditor.Text = editorPath;

        }

        
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

        private void ShowFormat_Click(object sender, RoutedEventArgs e)
        {
            Capture capture = Capture.GetInstance();
            capture.Format();
        }

        
    }
}
