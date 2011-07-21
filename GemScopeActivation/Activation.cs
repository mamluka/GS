using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Management;
using System.Windows;

namespace GemScopeActivation
{
    public class Activation
    {


        public string GetActivationCode()
        {

            string code = String.Empty;

            List<HardDrive> list = this.GetHardDriveList();

            //use the first harddrirve for a key

            HardDrive hd = list[0];

            string hdserial8 = hd.SerialNo.ToUpper();

            if (hdserial8.Length<8)
            {
                hdserial8 = hdserial8 + "ABCDEFGH";
            }


            string hdserial = hdserial8.Trim().Replace("-", String.Empty).Substring(0, 8);

            string _BaseString = Encryption.Boring(hdserial);

            //Regex.Match(

            return _BaseString;
        }

        public string ThisComputerKey()
        {
            string code = this.GetActivationCode();
            return Encryption.MakePassword(code, "852");
        }

        public string GenerateKeyFromCode(string code)
        {
            return Encryption.MakePassword(code, "852");
        }
       


        public List<HardDrive> GetHardDriveList()
        {
            List<HardDrive> hdCollection = new List<HardDrive>();

            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                HardDrive hd = new HardDrive();
                hd.Model = wmi_HD["Model"].ToString();
                hd.Type = wmi_HD["InterfaceType"].ToString();

                hdCollection.Add(hd);
            }

            searcher = new
                ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

            int i = 0;
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                // get the hard drive from collection

                if (((string)wmi_HD["Tag"]).IndexOf("PHYSICALDRIVE0") == -1)
                {
                    continue;
                }

                // using index
                HardDrive hd = (HardDrive)hdCollection[i];

                // get the hardware serial no.
                if (wmi_HD["SerialNumber"] == null)
                    hd.SerialNo = "None";
                else
                    hd.SerialNo = wmi_HD["SerialNumber"].ToString();

                ++i;
            }

            return hdCollection;
            // Display available hard drives
        }
            
    }


    public class HardDrive
    {
        private string model = null;
        private string type = null;
        private string serialNo = null;

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }
    }

  
}
