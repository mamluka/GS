using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemScopeActivation;
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
                Licence lic = new Licence();
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
    }
}
