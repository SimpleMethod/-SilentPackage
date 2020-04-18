
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation.Language;
using System.ServiceModel.Channels;
using System.Text;
using System.Timers;
using System.Windows;
using SilentPackage.Controllers;
using SilentPackage.Models;
using FileDirectory = SilentPackage.Models.FileDirectory;

namespace SilentPackage.Controllers
{



    public class DataCollection
    {
        // readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();
        //GeneralPurposeTimer generalPurposeTimer = GeneralPurposeTimer.GetInstance();
        private static DataCollection _mOInstance = null;
        private static Object _mutex = new Object();
        public static DataCollection GetInstance()
        {

            if (_mOInstance == null)
            {
                lock (_mutex)
                {
                    if (_mOInstance == null)
                    {
                        _mOInstance = new DataCollection();
                    }
                }
            }
            return _mOInstance;
        }

        private DataCollection()
        {

        }

        //public ComboModel GetComboModel()
        //{
        //    return generalPurposeTimer.GetComboModel();
        //}
    }

    public class PrintScrFileManagement
    {
        private readonly string[] _strlist = { ".jpg" };

        public List<Models.FileDirectory> GetPrintScrFile()
        {
            List<Models.FileDirectory> fileDirectories = new List<Models.FileDirectory>();
            foreach (var e in FileDirectory.ScanDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\screenshot\", _strlist))
            {
                fileDirectories.Add(new Models.FileDirectory(e.FullName.ToString(), e.CreationTimeUtc.ToString(), e.LastAccessTimeUtc.ToString(), e.LastWriteTimeUtc.ToString()));
            }
            return fileDirectories;
        }

        public void DeleteFile(string name)
        {
            try
            {
                File.Delete(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        public void ClearDirectory()
        {
            List<Models.FileDirectory> fileDirectories = GetPrintScrFile();
            foreach (var eDirectory in fileDirectories)
            {
                DeleteFile(eDirectory.FullName);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class PrintScrManagementTimer
    {
        readonly PrintScrManagement _printScrManagement = new PrintScrManagement();
        public void StartTimer(int minutes)
        {
            Timer oTimer = new Timer();
            oTimer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            oTimer.Interval = TimeSpan.FromMinutes(minutes).TotalMilliseconds;
            oTimer.Enabled = true;
        }


        private void OnTimeEvent(object oSource, ElapsedEventArgs oElapsedEventArgs)
        {
            _printScrManagement.GetPrintScreen();
        }
    }
    /// <summary>
    /// Class to start main timer.
    /// </summary>
    public class GeneralPurposeTimer
    {
        ComboModel _comboModel = new ComboModel();
        private int _iteration = 0;
        private int _interval = 0;
        private bool _smallState = false;
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();

        private Stack<ProcessList> processLists = new Stack<ProcessList>();
        private  Stack<BrowsingHistoryLists> browsingHistory = new Stack<BrowsingHistoryLists>();
        private Stack<FileDirectoryList> fileDirectory = new Stack<FileDirectoryList>();
        private DocumentTableGeneration documentTableGeneration = new DocumentTableGeneration();


        private static GeneralPurposeTimer _mOInstance = null;
        private static Object _mutex = new Object();

        public static GeneralPurposeTimer GetInstance()
        {

            if (_mOInstance == null)
            {
                lock (_mutex)
                {
                    if (_mOInstance == null)
                    {
                        _mOInstance = new GeneralPurposeTimer();
                    }
                }
            }
            return _mOInstance;
        }

        private GeneralPurposeTimer()
        {
            StartTimer();

            if (_configurationManagement.GetConfigModel().PrtScrnEnable)
            {
                var printScrManagementTimer = new PrintScrManagementTimer();
                printScrManagementTimer.StartTimer(_configurationManagement.GetConfigModel().PrtScrInterval);
            }

            if (_configurationManagement.GetConfigModel().ShutDownEnable)
            {
                var shutDownTimer = new ShutDownTimer();
                shutDownTimer.StartTimer(_configurationManagement.GetConfigModel().ShutDownTime);
            }

            if (_configurationManagement.GetConfigModel().ProgramBlockList != null)
            {
                var blockingProgram = new BlockingProgramManagement();
            }
        }

        public ComboModel GetComboModel()
        {
            return _comboModel;
        }

        public void ClearStack()
        {
            _comboModel.ProcessLists.Clear();
            _comboModel.BrowsingHistories.Clear();
            _comboModel.BrowsingHistories.Clear();
        }

        private void StartTimer()
        {
            int interval = _configurationManagement.GetConfigModel().IntervalTime;
            int masterInterval = interval;
            _smallState = false;
            if (interval <= 10)
            {
                _interval = interval;
                _smallState = true;
            }
            else
            {
                _interval = (interval / 10);
            }
            var oTimer = new Timer();
            oTimer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            //oTimer.Interval = TimeSpan.FromMinutes(interval).TotalMilliseconds;
            oTimer.Interval = 10000;
            oTimer.Enabled = true;
        }

        private void OnTimeEvent(object oSource, ElapsedEventArgs oElapsedEventArgs)
        {
            bool processEnable = false;
            bool historyEnable = false;
            bool fileDirectoryEnable = false;
            if (_configurationManagement.GetConfigModel().ListProcessesEnable)
            {
                ProcessListManagement listManagement = new ProcessListManagement();
                processLists.Push(listManagement.GetProcessListReports());
                processEnable = true;
            }

            if (_configurationManagement.GetConfigModel().WebHistoryEnable)
            {
                BrowsingHistoryManagement historyManagement = new BrowsingHistoryManagement();
                browsingHistory.Push(historyManagement.GetBrowsingHistory());
                historyEnable = true;
            }

            if (_configurationManagement.GetConfigModel().FileDirectoryList != null)
            {
                ScanDirectoryManagement scanDirectoryManagement = new ScanDirectoryManagement();
                fileDirectory.Push(scanDirectoryManagement.GetFileDirectoryReports());
                fileDirectoryEnable = true;
            }
            _iteration++;

            if (_smallState)
            {
               
                if (_iteration == _interval)
                {
                    MessageBox.Show(fileDirectory.Count.ToString());     
                    if (processEnable)
                    {
                        _comboModel.ProcessLists = processLists;
                    }

                    if (historyEnable)
                    {
                        _comboModel.BrowsingHistories = browsingHistory;
                    }

                    if (fileDirectoryEnable)
                    {
                        _comboModel.DirectoryLists = fileDirectory;
                    }



                  
                    //MessageBox.Show(_comboModel.ProcessLists.Count.ToString());
                    //MessageBox.Show(documentTableGeneration.GenerateTable(_comboModel.ProcessLists, 0));
                    //MessageBox.Show(documentTableGeneration.GenerateTable(_comboModel.BrowsingHistories, 1));
                    MessageBox.Show(documentTableGeneration.GenerateTable(_comboModel.DirectoryLists, 2));
                    _iteration = 0;
                }

            }

            /*
            if (_iteration == 10)
            {
                if (processEnable)
                {
                    _comboModel.ProcessLists = processLists;
                }

                if (historyEnable)
                {
                    _comboModel.BrowsingHistories = browsingHistory;
                }

                if (FileDirectoryEnable)
                {
                    _comboModel.DirectoryLists = fileDirectory;
                }
                _iteration = 0;
            }
            */
        }
    }

    /// <summary>
    /// Class for session time limitation
    /// </summary>
    public class ShutDownTimer
    {
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();
        readonly WindowsManagement _management = new WindowsManagement();
        public void StartTimer(int minutes)
        {
            Timer oTimer = new Timer();
            oTimer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            oTimer.Interval = TimeSpan.FromMinutes(minutes).TotalMilliseconds;
            oTimer.Enabled = true;
            //_management.DisplayInformationMessageBox("Na tym komputerze uruchomiono ograniczenie czasu pracy. Pozostało:"+ minutes + " minut do zakończenia sesji.", "Ograniczenie czasu pracy!");
        }


        private void OnTimeEvent(object oSource, ElapsedEventArgs oElapsedEventArgs)
        {
            int status = int.Parse(_configurationManagement.GetConfigModel().ShutDownOption);
            if (status == 0)
            {
                _management.Logout();
            }
            else if (status == 1)
            {
                _management.Shutdown();
                _management.PowerOff();
            }
            else if (status == 2)
            {
                _management.Reboot();
            }
        }
    }

    /// <summary>
    /// Class to handle program blocking.
    /// </summary>
    public class BlockingProgramManagement
    {
        readonly BlockingPrograms _blocking = BlockingPrograms.GetInstance(ConfigurationManagement.GetInstance().GetConfigModel().ProgramBlockList);
        public BlockingProgramManagement()
        {
            _blocking.EnablePolicy();
        }

    }

    /// <summary>
    /// Class for viewing user history.
    /// </summary>
    public class BrowsingHistoryManagement
    {
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();

        public BrowsingHistoryLists GetBrowsingHistory()
        {
            var configModel = _configurationManagement.GetConfigModel();
            var browsing = BrowsingHistory.GetInstance("Default", configModel.WebHistoryPath);
            var browsingHistoryLists = new BrowsingHistoryLists();
            List<BrowsingHistoryTab> browsingHistoryTabs = new List<BrowsingHistoryTab>();

            foreach (var f in browsing.GetHistoryFromDatabase(configModel.WebHistoryQueryLimit))
            {
                browsingHistoryTabs.Add(new BrowsingHistoryTab(f.GetUrl(), f.GetTitle(),
                    f.GetLastVisitTime(), f.GetDurationTime()));
            }
            browsingHistoryLists.BrowsingHistoryList = browsingHistoryTabs;
            browsingHistoryLists.Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            return browsingHistoryLists;
        }
    }

    /// <summary>
    /// Class for creating screenshots.
    /// </summary>
    public class PrintScrManagement
    {
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();

        public PrintScrManagement()
        {
            try
            {
                var di = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\screenshot\");
            }
            catch (IOException e)
            {
            }
        }
        public void GetPrintScreen()
        {
            var configModel = _configurationManagement.GetConfigModel();
            var screenshot = new PrintScreenManagement();
            //_configurationManagement.GetDeviceId() + " - " + 
            var filename = new StringBuilder(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + ".jpg");
            var quality = int.Parse(configModel.PrtScrnQualityOption);
            PrintScreenManagement.JpegQuality jpeg;
            switch (quality)
            {
                case 0:
                    {
                        jpeg = PrintScreenManagement.JpegQuality.VLOW;
                        break;
                    }
                case 1:
                    {
                        jpeg = PrintScreenManagement.JpegQuality.LOW;
                        break;
                    }
                case 2:
                    {
                        jpeg = PrintScreenManagement.JpegQuality.MEDIUM;
                        break;
                    }
                case 3:
                    {
                        jpeg = PrintScreenManagement.JpegQuality.HIGH;
                        break;
                    }
                default:
                    {
                        jpeg = PrintScreenManagement.JpegQuality.BEST;
                        break;
                    }
            }
            screenshot.MakePrintScreen(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\screenshot\", filename.ToString(), jpeg);
        }
    }

    /// <summary>
    /// Class for directory scanning.
    /// </summary>
    public class ScanDirectoryManagement
    {
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();
        public FileDirectoryList GetFileDirectoryReports()
        {
            ConfigModel configModel = _configurationManagement.GetConfigModel();
            List<string> directory = configModel.FileDirectoryList;
            FileDirectoryList fileDirectory = new FileDirectoryList();
            List<Models.FileDirectory> fileDirectories = new List<Models.FileDirectory>();
            char[] separator = { ',', ';' };

            String[] strlist = configModel.FileDirectoryExtension.Split(separator);

            foreach (var f in directory)
            {
                foreach (var e in FileDirectory.ScanDirectory(f, strlist))
                {
                    fileDirectories.Add(new Models.FileDirectory(e.FullName.ToString(),
                        e.CreationTimeUtc.ToString(), e.LastAccessTimeUtc.ToString(), e.LastWriteTimeUtc.ToString()));
                }
            }

            if (configModel.RemovableDevicesEnable)
            {
                foreach (var e in FileDirectory.ScanRemovableDrives(strlist))
                {
                    fileDirectories.Add(new Models.FileDirectory(e.FullName.ToString(),
                        e.CreationTimeUtc.ToString(), e.LastAccessTimeUtc.ToString(), e.LastWriteTimeUtc.ToString()));
                }
            }

            fileDirectory.FileDirectories = fileDirectories;
            fileDirectory.Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            return fileDirectory;
        }
    }

    /// <summary>
    /// Class using to getting the process list.
    /// </summary>
    public class ProcessListManagement
    {
        private readonly WindowsManagement _windows = new WindowsManagement();

        public ProcessList GetProcessListReports()
        {
            ProcessList processList = new ProcessList();
            List<Process> processes = new List<Process>();
            foreach (var f in _windows.GetProcessListPs(true))
            {
                processes.Add(new Process(f.GetName(), f.GetId(), f.GetStartTime()));
            }
            processList.ProcessObjectList = processes;
            processList.Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            return processList;
        }
    }
}