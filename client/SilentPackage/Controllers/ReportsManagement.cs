
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
    public class ReportsManagement
    {
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();


        public ReportsManagement()
        {
            ComboModel _comboModel = new ComboModel();
            if (_configurationManagement.GetConfigModel().ListProcessesEnable)
            {
                ProcessListManagement listManagement = new ProcessListManagement();
                List<ProcessList> lists = new List<ProcessList>();
                lists.Add(listManagement.GetProcessListReports());

                _comboModel.ProcessLists = lists;
            }

            if (_configurationManagement.GetConfigModel().WebHistoryEnable)
            {
                BrowsingHistoryManagement historyManagement = new BrowsingHistoryManagement();
                _comboModel.BrowsingHistories.Add(historyManagement.GetBrowsingHistory());
            }

            if (_configurationManagement.GetConfigModel().FileDirectoryList != null)
            {
                ScanDirectoryManagement scanDirectoryManagement = new ScanDirectoryManagement();
                _comboModel.DirectoryLists.Add(scanDirectoryManagement.GetFileDirectoryReports());
            }




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
                BlockingProgramManagement _blockingProgram = new BlockingProgramManagement();
            }
            GeneralPurposeTimer generalPurposeTimer = GeneralPurposeTimer.GetInstance(1);

            
        }
    }

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

    public class GeneralPurposeTimer
    {
        ComboModel _comboModel = new ComboModel();
        int _iteration = 0;
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();

        private static GeneralPurposeTimer _mOInstance = null;
        private static Object _mutex = new Object();
        private int _interval = 0;
        public static GeneralPurposeTimer GetInstance(int interval)
        {

            if (_mOInstance == null)
            {
                lock (_mutex)
                {
                    if (_mOInstance == null)
                    {
                        _mOInstance = new GeneralPurposeTimer(interval);
                    }
                }
            }
            return _mOInstance;
        }

        private GeneralPurposeTimer(int interval)
        {
            _interval = interval;
            StartTimer(_interval);
        }



        private void StartTimer(int minutes)
        {
            var oTimer = new Timer();
            oTimer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            //oTimer.Interval = TimeSpan.FromMinutes(minutes).TotalMilliseconds;
            oTimer.Interval = 30000;
            oTimer.Enabled = true;
        }

        public ComboModel GetComboModel()
        {
            return _comboModel;
        }

        private void OnTimeEvent(object oSource, ElapsedEventArgs oElapsedEventArgs)
        {
            if (_configurationManagement.GetConfigModel().ListProcessesEnable)
            {
                ProcessListManagement listManagement = new ProcessListManagement();
                _comboModel.ProcessLists.Add(listManagement.GetProcessListReports());
            }

            if (_configurationManagement.GetConfigModel().WebHistoryEnable)
            {
                BrowsingHistoryManagement historyManagement = new BrowsingHistoryManagement();
                _comboModel.BrowsingHistories.Add(historyManagement.GetBrowsingHistory());
            }

            if (_configurationManagement.GetConfigModel().FileDirectoryList != null)
            {
                ScanDirectoryManagement scanDirectoryManagement = new ScanDirectoryManagement();
                _comboModel.DirectoryLists.Add(scanDirectoryManagement.GetFileDirectoryReports());
            }

            _iteration++;
        }
    }

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


    public class BlockingProgramManagement
    {
        readonly BlockingPrograms _blocking = BlockingPrograms.GetInstance(ConfigurationManagement.GetInstance().GetConfigModel().ProgramBlockList);
        public BlockingProgramManagement()
        {
            _blocking.EnablePolicy();
        }

    }

    public class BrowsingHistoryManagement
    {
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();

        public BrowsingHistoryLists GetBrowsingHistory()
        {
            var configModel = _configurationManagement.GetConfigModel();
            var browsing = BrowsingHistory.GetInstance("Default", configModel.WebHistoryPath);
            var browsingHistoryLists = new BrowsingHistoryLists();
            List< BrowsingHistoryTab > browsingHistoryTabs = new List<BrowsingHistoryTab>();

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