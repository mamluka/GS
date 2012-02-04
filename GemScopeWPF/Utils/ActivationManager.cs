using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemScopeActivation;
using Microsoft.Win32;
using System.Security.AccessControl;
namespace GemScopeWPF.Utils
{
    public class ActivationManager
    {
        private static ActivationManager Singelton { get; set; }

        public static ActivationManager GetInstance()
        {
            if (Singelton == null)
            {
                Singelton = new ActivationManager();
                return Singelton;
            }
            else
            {
                return Singelton;
            }
        }

        public void PresistActivation()
        {
            if (!this.ActivationStatus())
            {
                License lic = new License();
                lic.ShowDialog();

            }
        }
        public bool ActivationStatus()
        {

            Activation activation = new Activation();

            string key = SettingsManager.ReadSetting("ActivationKey");

            string code = activation.GetActivationCode();



            string correctkey = activation.ThisComputerKey();

            if (correctkey == key)
            {
                return true;
            }
            else
            {
                return false;
            }


            
            
        }
        public string GetKey()
        {
            Activation activation = new Activation();

            return SettingsManager.ReadSetting("ActivationKey");


        }
        public bool IsExipred()
        {

            int days= CountDays();

            if (days > 30)
            {
                return true;
            }
           

            return false;
        }
        public int CountDays() {


        

            DateTime daystart = DateTime.Now;
            RegistryKey days = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\GEMSCOPE");
           
           


            if (days != null)
            {
                string dl = (string)days.GetValue("dl");

                if (dl != null)
                {
                    daystart = DateTime.Parse(dl);
                }
                else
                {
                    days.SetValue("dl", DateTime.UtcNow);
                }

                days.Close();

            }

           // daystart = DateTime.Now.AddDays(-45);

            

            Registry.LocalMachine.Flush();
           
            DateTime newDate = DateTime.Now;

            // Difference in days, hours, and minutes.
            TimeSpan ts = newDate - daystart;
            // Difference in days.
            int differenceInDays = ts.Days;

            return differenceInDays;
            
        
        }
    }
}
