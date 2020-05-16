/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.Win32;
using SilentPackage.Controllers;
using SilentPackage.Models;

namespace SilentPackage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct Settings
        {
            public bool ShutDownEnable;
            public bool ListProcessesEnable;
            public bool WebHistoryEnable;
            public bool PrtScrnEnable;
            public bool RemovableDevicesEnable;
            public bool OfflineMode;
            public int ProgramBlockList;
            public int FileDirectoryList;

            public Settings(bool shutDownEnable, bool listProcessesEnable, bool webHistoryEnable, bool prtScrnEnable, bool removableDevicesEnable, bool offlineMode, int programBlockList, int fileDirectoryList)
            {
                ShutDownEnable = shutDownEnable;
                ListProcessesEnable = listProcessesEnable;
                WebHistoryEnable = webHistoryEnable;
                PrtScrnEnable = prtScrnEnable;
                RemovableDevicesEnable = removableDevicesEnable;
                OfflineMode = offlineMode;
                ProgramBlockList = programBlockList;
                FileDirectoryList = fileDirectoryList;
            }
        }
        Settings _settings = new Settings(false, false, false, false, false, false, 0, 0);
        List<String> _programList = new List<string>();
        List<String> _scanDirectory = new List<string>();
        string _pathToWebBrowser;
        public MainWindow()
        {
            InitializeComponent();
            windowsManagement_ShutDownOption_ComboBox.IsEnabled = false;
            windowsManagement_ShutDownTime_TextBox.IsEnabled = false;



            browsingHistory_AddPath_Exec.IsEnabled = false;
            windowsMonitoring_WebHistory_QueryLimit_TextBox.IsEnabled = false;


            windowsMonitoring_PrtScrn_Offset_TextBox.IsEnabled = false;
            windowsMonitoring_PrtScrn_Quality_TextBox.IsEnabled = false;

            var ShutDownOption = new[] {
                new { Text = "Wylogowanie użytkownika", Value = "0" },
                new { Text = "Zamknięcie systemu", Value = "1" },
                new { Text = "Ponowne uruchomienie", Value = "2" }
            };
            windowsManagement_ShutDownOption_ComboBox.ItemsSource = ShutDownOption;
            windowsManagement_ShutDownOption_ComboBox.DisplayMemberPath = "Text";
            windowsManagement_ShutDownOption_ComboBox.SelectedItem = new { Text = "Wylogowanie użytkownika", Value = "0" };


            var prtScrnQuality = new[] {
                new { Text = "Bardzo niska", Value = "0" },
                new { Text = "Niska", Value = "1" },
                new { Text = "Średnia", Value = "2" },
                new { Text = "Dobra", Value = "3" },
                new { Text = "Bardzo dobra", Value = "4" }
            };
            windowsMonitoring_PrtScrn_Quality_TextBox.ItemsSource = prtScrnQuality;
            windowsMonitoring_PrtScrn_Quality_TextBox.DisplayMemberPath = "Text";
            windowsMonitoring_PrtScrn_Quality_TextBox.SelectedItem = new { Text = "Bardzo niska", Value = "0" };
        }




        private void blockProgram_AddProgram_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Executable file (*.exe)|*.exe",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() != true) return;



            if (!_programList.Exists(element => element.Equals(openFileDialog.SafeFileName)))
            {
                _programList.Clear();
                blockProgram_List.Items.Add(openFileDialog.SafeFileName);
                foreach (var foreachItem in blockProgram_List.Items)
                {
                    _programList.Add(foreachItem.ToString());
                }
            }
        }

        private void blockProgram_RemoveProgram_Click(object sender, RoutedEventArgs e)
        {
            blockProgram_List.Items.Refresh();
            for (int i = 0; i < blockProgram_List.SelectedItems.Count; i++)
                blockProgram_List.Items.Remove(blockProgram_List.SelectedItems[i]);
        }

        private void windowsManagement_ShutDownTime_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void windowsManagement_ShutDownTime_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void browsingHistory_AddPath_Click(object sender, RoutedEventArgs e)
        {
            _pathToWebBrowser = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default";
            if (!Directory.Exists(_pathToWebBrowser))
            {
                _pathToWebBrowser = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            }

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _pathToWebBrowser,
                FileName = "History",
                Multiselect = false
            };
            try
            {
                if (openFileDialog.ShowDialog() != true) return;
            }
            catch (ArgumentException exception)
            {
            }

        }

        private void windowsManagement_ShutDown_Enable_Exec_Click(object sender, RoutedEventArgs e)
        {
            if (windowsManagement_ShutDownOption_ComboBox.IsEnabled)
            {
                windowsManagement_ShutDownOption_ComboBox.IsEnabled = false;
                windowsManagement_ShutDownTime_TextBox.IsEnabled = false;
                _settings.ShutDownEnable = false;
            }
            else
            {
                windowsManagement_ShutDownOption_ComboBox.IsEnabled = true;
                windowsManagement_ShutDownTime_TextBox.IsEnabled = true;
                _settings.ShutDownEnable = true;
            }
        }

        private void windowsMonitoring_WebHistory_Enable_Exec_Click(object sender, RoutedEventArgs e)
        {
            if (browsingHistory_AddPath_Exec.IsEnabled)
            {
                browsingHistory_AddPath_Exec.IsEnabled = false;
                windowsMonitoring_WebHistory_QueryLimit_TextBox.IsEnabled = false;
                _settings.WebHistoryEnable = false;
            }
            else
            {
                browsingHistory_AddPath_Exec.IsEnabled = true;
                windowsMonitoring_WebHistory_QueryLimit_TextBox.IsEnabled = true;
                _settings.WebHistoryEnable = true;
            }
        }

        private void windowsMonitoring_PrtScrn_Enable_Exec_Click(object sender, RoutedEventArgs e)
        {
            if (windowsMonitoring_PrtScrn_Offset_TextBox.IsEnabled)
            {
                windowsMonitoring_PrtScrn_Offset_TextBox.IsEnabled = false;
                windowsMonitoring_PrtScrn_Quality_TextBox.IsEnabled = false;
                _settings.PrtScrnEnable = false;
            }
            else
            {
                windowsMonitoring_PrtScrn_Offset_TextBox.IsEnabled = true;
                windowsMonitoring_PrtScrn_Quality_TextBox.IsEnabled = true;
                _settings.PrtScrnEnable = true;
            }
        }

        private void windowsManagement_ListProcesses__Enable_Exec_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)windowsManagement_ListProcesses__Enable_Exec.IsChecked)
            {
                _settings.ListProcessesEnable = true;
            }
            else
            {
                _settings.ListProcessesEnable = false;
            }
        }

        private void windowsMonitoring_FileDirectory_RemovableDevices_Exec_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)windowsMonitoring_FileDirectory_RemovableDevices_Exec.IsChecked)
            {
                _settings.RemovableDevicesEnable = true;
            }
            else
            {
                _settings.RemovableDevicesEnable = false;
            }
        }

        private void settingsPage_TP_OfflineMode_Exec_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)settingsPage_TP_OfflineMode_Exec.IsChecked)
            {
                _settings.OfflineMode = true;
            }
            else
            {
                _settings.OfflineMode = false;
            }
        }

        private void TextBox_MinimalLength(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text.Length < 1 || ((TextBox)sender).Text.Equals("0"))
            {
                MessageBox.Show("Wartość musi być większa od zera");
                ((TextBox)sender).Text = "10";
            }
        }

        private void PasswordBox_MinimalLength(object sender, RoutedEventArgs e)
        {
            if (((PasswordBox)sender).Password.Length < 1 || ((PasswordBox)sender).Password.Equals("0"))
            {
                MessageBox.Show("Wartość musi być większa od zera");
                ((PasswordBox)sender).Password = "10";
            }
        }

        private void TextBox_CheckURLAddress(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text.Length < 1 || ((TextBox)sender).Text.Equals("0"))
            {
                MessageBox.Show("Wartość musi być większa od zera");
                ((TextBox)sender).Text = "10";
            }
            try
            {
                Uri tempValue;
                if (!Uri.TryCreate(settingsPage_CC_URL_TextBox.Text, UriKind.Absolute, out tempValue))
                {
                    MessageBox.Show("Niepoprawny adres URL");
                    ((TextBox)sender).Text = "";
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }

        }

        private void windowsMonitoring_FileDirectory_AddPath_Exec_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var openFileDialog = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Wybierz folder docelowy"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                folderPath = Path.GetDirectoryName(openFileDialog.FileName);
            }

            if (!_scanDirectory.Exists(element => element.Equals(folderPath)))
            {
                _scanDirectory.Clear();
                windowsMonitoring_FileDirectory_ListView.Items.Add(folderPath);
                foreach (var foreachItem in windowsMonitoring_FileDirectory_ListView.Items)
                {
                    _scanDirectory.Add(foreachItem.ToString());
                }
            }
        }

        private void windowsMonitoring_FileDirectory_RemovePath_Exec_Click(object sender, RoutedEventArgs e)
        {
            windowsMonitoring_FileDirectory_ListView.Items.Refresh();
            for (int i = 0; i < windowsMonitoring_FileDirectory_ListView.SelectedItems.Count; i++)
                windowsMonitoring_FileDirectory_ListView.Items.Remove(windowsMonitoring_FileDirectory_ListView.SelectedItems[i]);
        }


        private void TextBox_CheckPatter(object sender, RoutedEventArgs e)
        {
            const string patternSpace = @"\s+";
            const string filenameExtension = @"\.[a-zA-Z0-9]+";
            var filenameExtensionRegex = new Regex(filenameExtension);
            var spaceRegex = new Regex(patternSpace);
            char[] separator = { ',', ';' };

            String[] strlist = ((TextBox)sender).Text.Split(separator);

            foreach (String s in strlist)
            {
                if (spaceRegex.Matches(s).Count() != 0 || !filenameExtensionRegex.Matches(s).Any())
                {
                    MessageBox.Show("Niepoprawne rozszerzenie pliku");
                }
            }
        }

        private void settingsPage_Save_Exec_Click(object sender, RoutedEventArgs e)
        {
            var configModel = new ConfigModel();
            var dataHandler = new EncryptDataHandler(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data");
            var fileManagement = new FileManagement();


            if (_settings.ShutDownEnable)
            {
                configModel.ShutDownEnable = _settings.ShutDownEnable;
                configModel.ShutDownOption = windowsManagement_ShutDownOption_ComboBox.SelectedItem.GetType().GetProperty("Value")?.GetValue(windowsManagement_ShutDownOption_ComboBox.SelectedItem, null).ToString();
                configModel.ShutDownTime = int.Parse(windowsManagement_ShutDownTime_TextBox.Text);
            }

            if (_settings.ListProcessesEnable)
            {
                configModel.ListProcessesEnable = _settings.ListProcessesEnable;
            }

            if (_programList.Count != 0)
            {
                configModel.ProgramBlockList = _programList;
            }

            if (_settings.WebHistoryEnable)
            {
                configModel.WebHistoryEnable = _settings.WebHistoryEnable;
                configModel.WebHistoryQueryLimit = int.Parse(windowsMonitoring_WebHistory_QueryLimit_TextBox.Text);
                configModel.WebHistoryPath = _pathToWebBrowser;
            }

            if (_settings.PrtScrnEnable)
            {
                configModel.PrtScrnEnable = _settings.PrtScrnEnable;
                configModel.PrtScrnQualityOption = windowsMonitoring_PrtScrn_Quality_TextBox.SelectedItem.GetType().GetProperty("Value")?.GetValue(windowsMonitoring_PrtScrn_Quality_TextBox.SelectedItem, null).ToString();
                configModel.PrtScrInterval = int.Parse(windowsMonitoring_PrtScrn_Offset_TextBox.Text);
            }

            if (_scanDirectory.Count != 0)
            {
                configModel.FileDirectoryList = _scanDirectory;
                configModel.FileDirectoryExtension = windowsMonitoring_FileDirectory_EXT_TextBox.Text;
                configModel.RemovableDevicesEnable = _settings.RemovableDevicesEnable;
            }

            if (ProtectLicenseKey(settingsPage_CC_Key_TextBox.Password).Length == 0 || settingsPage_CC_URL_TextBox.Text.Length==0 || settingsPage_ID_ID_TextBox.Text.Length == 0 || settingsPage_TP_Offset_TexBox.Text.Length==0)
            {
                MessageBox.Show("Brak wszystkich wymaganych danych!");
                return;
            }
            configModel.OfflineMode = _settings.OfflineMode;
            configModel.AddressCc = settingsPage_CC_URL_TextBox.Text;
            configModel.License = fileManagement.Base64Encode(dataHandler.EncryptText(ProtectLicenseKey(settingsPage_CC_Key_TextBox.Password)));
            configModel.IntervalTime = int.Parse(settingsPage_TP_Offset_TexBox.Text);
            fileManagement.CreateFile(null,"config.bin", Encoding.ASCII.GetBytes(JsonSerializer.Serialize(configModel)), true);
            HttpClient client = new HttpClient();
            if (_settings.OfflineMode.Equals(false))
            {
                var identification = new UserIdentification();
                StringBuilder urlBuilder = new StringBuilder(settingsPage_CC_URL_TextBox.Text+"/api/1.1/users/" + settingsPage_CC_Key_TextBox.Password+"/"+ settingsPage_ID_ID_TextBox.Text);
                client.SetDeviceID(urlBuilder.ToString());
            }
            using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\debug.bin"))
            {
                sw.WriteLine(settingsPage_ID_ID_TextBox.Text);
            }
            MessageBox.Show("Zapisano ustawienia, uruchom ponownie program!");
        }

        private void settingsPage_ID_GetID_Exec_Click(object sender, RoutedEventArgs e)
        {
            var identification = new UserIdentification();
            settingsPage_ID_ID_TextBox.Text = identification.GetMachineID();
        }

        private void settingsPage_CC_Testing_Exec_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();

            Uri tempValue;
            if (!Uri.TryCreate(settingsPage_CC_URL_TextBox.Text, UriKind.Absolute, out tempValue))
            {
                MessageBox.Show("Niepoprawny adres URL");
                ((TextBox)sender).Text = "";
            }
            var urlBuilder = new StringBuilder(settingsPage_CC_URL_TextBox.Text + "/api/1.0/users/" + ProtectLicenseKey(settingsPage_CC_Key_TextBox.Password));
            MessageBox.Show(client.MakeWebRequest(urlBuilder.ToString(), "GET", true).Equals("200")
                ? "Połączenie nawiązane"
                : "Nieudane połączenie lub błędne dane");
        }

        public string ProtectLicenseKey(string data)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9=]+");
            data = rgx.Replace(data, "");
            return data;
        }
    }
}
