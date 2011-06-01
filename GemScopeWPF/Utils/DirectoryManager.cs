using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace GemScopeWPF.Utils
{
    class DirectoryManager
    {
        private static DirectoryManager Singelton { get; set; }
        public string HomeFolder { get; set; }
        public string DefaultHomeFolder { get; set; }

        public static DirectoryManager GetInstance()
        {
            if (Singelton == null)
            {
                Singelton = new DirectoryManager();
                return Singelton;
            }
            else
            {
                return Singelton;
            }
        }
        public DirectoryManager()
        {
            DefaultHomeFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DJV");
        }
        public bool CreateDefaultFolder()
        {



            try
            {
                
                Directory.CreateDirectory(DefaultHomeFolder);

                SettingsManager.UpdateSetting("StoneRepRootFolder", DefaultHomeFolder);

                return true;
            }
            catch (Exception)
            {
                return false;
                
            }
            
        }
        public bool IsDefaultFolderExists()
        {
            
            return Directory.Exists(DefaultHomeFolder);
            
        }

        public string GetHomeFolderFromSettings()
        {
            string homepath = SettingsManager.ReadSetting("StoneRepRootFolder");
            return homepath;
        }
        public string GetHomeFolder()
        {
            string homepath = this.GetHomeFolderFromSettings();
            if (String.IsNullOrWhiteSpace(homepath))
            {
                if (this.IsDefaultFolderExists())
                {
                    return DefaultHomeFolder;
                }
                else
                {
                    if (this.CreateDefaultFolder())
                    {
                        return DefaultHomeFolder;
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }
            else
            {
                if (Directory.Exists(homepath))
                {
                    return homepath;
                }
                else
                {
                    MessageBox.Show("The stone image folder was not found, please select a new folder");
                    Options options = new Options(0);
                    options.ShowDialog();
                    return this.GetHomeFolder();

                    
                }
            }
        }
        public void SaveHomeFolder(string path)
        {
            SettingsManager.UpdateSetting("StoneRepRootFolder", path);
            this.HomeFolder = path;
        }
            
    }
}
