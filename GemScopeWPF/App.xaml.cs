using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GemScopeWPF.Utils;
using System.IO;
using NLog;

namespace GemScopeWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args

            
           
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception


            
            logger.Fatal(e.Exception.Message);
            logger.Fatal(e.Exception.StackTrace);
            if (e.Exception.InnerException != null)
            {
                logger.Fatal(e.Exception.InnerException.Message);
                logger.Fatal(e.Exception.InnerException.StackTrace);
            }

            MessageBox.Show("Error:" + e.Exception.Message);
            

            // Prevent default unhandled exception processing
            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}
