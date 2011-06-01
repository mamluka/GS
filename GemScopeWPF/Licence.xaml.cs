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
using System.Windows.Shapes;
using GemScopeWPF.Utils;
using GemScopeActivation;
namespace GemScopeWPF
{
    /// <summary>
    /// Interaction logic for Licence.xaml
    /// </summary>
    public partial class Licence : Window
    {
        public Licence()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Activation activation = new Activation();
            this.ActivationCode.Text = activation.GetActivationCode();
            
        }

        private void TryLater_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void VerifyKey_Click(object sender, RoutedEventArgs e)
        {
            
            Activation activation = new Activation();

            string correctkey = activation.ThisComputerKey();

            string key = this.Key.Text;

            if (key == correctkey)
            {
                SettingsManager.UpdateSetting("ActivationKey", key);
                MessageBox.Show("Activation Successful");
                this.Close();
            } 
            else 
            {
                MessageBox.Show("The key is wrong, try again...");
            }

            
        }
    }
}
