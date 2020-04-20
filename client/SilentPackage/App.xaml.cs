using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using SilentPackage.Controllers;

namespace SilentPackage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
          
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\"))
            {
                DirectoryInfo di = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\");
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\key_part_1.bin") && !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\key_part_2.bin"))
            {
                EncryptDataHandler dataHandler = new EncryptDataHandler(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data");
                dataHandler.CreatePair(null, false);
            }
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\config.bin"))
            {
               
                ConfigurationManagement configManagement = ConfigurationManagement.GetInstance();
                //DataCollection management = DataCollection.GetInstance();
                GeneralPurposeTimer generalPurposeTimer = GeneralPurposeTimer.GetInstance();
            }
            else
            {
                StartupUri = new Uri("Views/MainWindow.xaml", UriKind.Relative);
            }
        }
    }
}
