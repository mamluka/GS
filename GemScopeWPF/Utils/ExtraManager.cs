using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace GemScopeWPF.Utils
{
    public class ExtraManager
    {
        public void EditImageWithDefaultEditor(string filename)
        {
            string apppath = SettingsManager.ReadSetting("ImageEditorPath");
            if (String.IsNullOrWhiteSpace(apppath))
            {
                apppath = GetDefaultImageEditor();
            }
            

            var startInfo = new System.Diagnostics.ProcessStartInfo();

          
            startInfo.Arguments = "\"" + filename + "\"";
            startInfo.FileName = apppath;

            try
            {
                System.Diagnostics.Process regProcess = System.Diagnostics.Process.Start(startInfo);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error with opening editor");
              
            }

        }
        /// <summary>
        /// First detect photoshop-->gimp-->ms paint
        /// </summary>
        /// <returns></returns>
        public string GetDefaultImageEditor()
        {
            //check for photoshop
            //TODO find a way to locate photoshop and gimp
            //var softwareList = GetinstalledsoftwareByNameAndKnownExePath("photoshop",)
            //check gimp
           // var softwareList = new List<string>();
           //  softwareList =  GetinstalledsoftwareByNameAndKnownExePath("gimp", );

            string paintpath = Path.Combine(Environment.SystemDirectory, "mspaint.exe");

            return paintpath;
        }

        /// <summary>
        /// Gets a list of installed software and, if known, the software's install path.
        /// </summary>
        /// <returns></returns>
        public List<string> GetinstalledsoftwareByNameAndKnownExePath(string name, string path)
        {
            //Declare the string to hold the list:
            List<string> softwareList = new List<string>();

            //The registry key:
            const string softwareKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(softwareKey))
            {
                //Let's go through the registry keys and get the info we need:
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            //If the key has value, continue, if not, skip it:
                            const string displayname = "DisplayName";
                            if (!(sk.GetValue(displayname) == null) && ((string)sk.GetValue(displayname)).ToLower().Contains(name))
                            {
                                //Is the install location known?
                                 var installLocation = sk.GetValue("InstallLocation");
                                if (installLocation != null)
                                {
                                    string apppath = Path.Combine((string) installLocation, path);

                                    if (File.Exists(apppath))
                                    {
                                        softwareList.Add(apppath);
                                    }
                                    
                                   
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //No, that exception is not getting away... :P
                        }
                    }

                    return softwareList;
                }
            }

            return softwareList;

        }

    }
}
