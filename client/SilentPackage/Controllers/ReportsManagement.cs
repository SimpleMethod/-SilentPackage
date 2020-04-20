
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using SilentPackage.Controllers.DocumentGenerator;
using SilentPackage.Models;
using FileDirectory = SilentPackage.Models.FileDirectory;
using Process = SilentPackage.Models.Process;
using Timer = System.Timers.Timer;

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

        /// <summary>
        /// Method using to package reports to 7z archive.
        /// </summary>
        /// <param name="filename">Name of file.</param>
        public void PackageReports(string filename)
        {

            StringBuilder builder = new StringBuilder(" a -t7z \u0022" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\" + filename + ".7z" + "\u0022 -m0=BCJ2 -m1=LZMA2:d=1024m -aoa  \u0022" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\*" + "\u0022");

            //7za.exe a -t7z E:\Github\SilentPackage\client\SilentPackage\Files.7z -m0=BCJ2 -m1=LZMA2:d=1024m -aoa C:\Users\Pathfinder\AppData\Local\SP\UserActivityReport
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        FileName = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\7za\7za.exe",
                        Arguments = builder.ToString(),
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    // Zakonczenie tworzenia archiwum <?> 
                    //MessageBox.Show(process.StandardOutput.ReadLine());
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Method using to generate reports.
        /// </summary>
        /// <param name="processPage">String with data.</param>
        /// <param name="webHistoryPage">String with data.</param>
        /// <param name="directoryPage">String with data.</param>
        /// <param name="screenshotsPage">String with data.</param>
        /// <param name="index">String with data.</param>
        /// <param name="bootstrap">String with data.</param>
        /// <param name="style">String with data.</param>
        /// <param name="license">String with data.</param>
        public void GenerateReports(string processPage, string webHistoryPage, string directoryPage, string screenshotsPage, string index, string bootstrap, string style, string license)
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\"))
            {
                DirectoryInfo di = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport");
                di.Attributes = FileAttributes.Directory;
            }
            using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\index.html"))
            {
                byte[] dataBytes = new UTF8Encoding(true).GetBytes(index);
                fs.Write(dataBytes, 0, dataBytes.Length);
            }

            using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\processList.html"))
            {
                byte[] dataBytes = new UTF8Encoding(true).GetBytes(processPage);
                fs.Write(dataBytes, 0, dataBytes.Length);
            }
            using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\webHistoryList.html"))
            {
                byte[] dataBytes = new UTF8Encoding(true).GetBytes(webHistoryPage);
                fs.Write(dataBytes, 0, dataBytes.Length);
            }
            using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\directoryList.html"))
            {
                byte[] dataBytes = new UTF8Encoding(true).GetBytes(directoryPage);
                fs.Write(dataBytes, 0, dataBytes.Length);
            }
            using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\screenshotsList.html"))
            {
                byte[] dataBytes = new UTF8Encoding(true).GetBytes(screenshotsPage);
                fs.Write(dataBytes, 0, dataBytes.Length);
            }

            using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\bootstrap.min.css"))
            {
                byte[] dataBytes = new UTF8Encoding(true).GetBytes(bootstrap);
                fs.Write(dataBytes, 0, dataBytes.Length);
            }
            using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\style.css"))
            {
                byte[] dataBytes = new UTF8Encoding(true).GetBytes(style);
                fs.Write(dataBytes, 0, dataBytes.Length);
            }
            using (FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\LICENSE"))
            {
                byte[] dataBytes = new UTF8Encoding(true).GetBytes(license);
                fs.Write(dataBytes, 0, dataBytes.Length);
            }

          
        }
    }


    /// <summary>
    /// Class to start main timer.
    /// </summary>
    public class GeneralPurposeTimer
    {
        private readonly ComboModel _comboModel = new ComboModel();
        private readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();
        private readonly UserIdentification _userIdentification = new UserIdentification();
        private readonly Stack<ProcessList> _processLists = new Stack<ProcessList>();
        private readonly Stack<BrowsingHistoryLists> _browsingHistory = new Stack<BrowsingHistoryLists>();
        private readonly Stack<FileDirectoryList> _fileDirectory = new Stack<FileDirectoryList>();
        private readonly PrintScrFileManagement _fileManagement = new PrintScrFileManagement();

        private int _iteration = 0;
        private int _interval = 0;
        private bool _smallState = false;
        private static string _deviceId = null;
        private int masterInterval = 0;

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
            _deviceId = _userIdentification.GetMachineID();
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

        /// <summary>
        /// Return object with telemetry.
        /// </summary>
        /// <returns>Object with data</returns>
        public ComboModel GetComboModel()
        {
            return _comboModel;
        }

        /// <summary>
        /// Clear stack with data.
        /// </summary>
        public void ClearStack()
        {
            _comboModel.ProcessLists.Clear();
            _comboModel.BrowsingHistories.Clear();
            _comboModel.BrowsingHistories.Clear();
        }

        /// <summary>
        /// Start timer using to generate documents.
        /// </summary>
        private void StartTimer()
        {
            int interval = _configurationManagement.GetConfigModel().IntervalTime;
            masterInterval = interval;
            _smallState = false;
            _fileManagement.ClearDirectory();
            if (interval <= 10)
            {
                _interval = 1;
                _smallState = true;
            }
            else
            {
                _interval = (interval / 10);
            }
            var oTimer = new Timer();
            oTimer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            oTimer.Interval = TimeSpan.FromMinutes(_interval).TotalMilliseconds;
            //oTimer.Interval = 10000;
            oTimer.Enabled = true;
        }

        /// <summary>
        /// Main method using to generate documents.
        /// </summary>
        /// <param name="oSource"></param>
        /// <param name="oElapsedEventArgs"></param>
        private void OnTimeEvent(object oSource, ElapsedEventArgs oElapsedEventArgs)
        {
            DocumentTableGenerator _documentTableGeneration = new DocumentTableGenerator();
            DocumentNavGenerator _documentNavGenerator = new DocumentNavGenerator();
            DocumentGenerator.DocumentGenerator _documentGenerator = new DocumentGenerator.DocumentGenerator();
            bool processEnable = false;
            bool historyEnable = false;
            bool fileDirectoryEnable = false;
            bool executeOrder67 = false;
            if (_configurationManagement.GetConfigModel().ListProcessesEnable)
            {
                ProcessListManagement listManagement = new ProcessListManagement();
                _processLists.Push(listManagement.GetProcessListReports());
                processEnable = true;
            }

            if (_configurationManagement.GetConfigModel().WebHistoryEnable)
            {
                BrowsingHistoryManagement historyManagement = new BrowsingHistoryManagement();
                _browsingHistory.Push(historyManagement.GetBrowsingHistory());
                historyEnable = true;
            }

            if (_configurationManagement.GetConfigModel().FileDirectoryList != null)
            {
                ScanDirectoryManagement scanDirectoryManagement = new ScanDirectoryManagement();
                _fileDirectory.Push(scanDirectoryManagement.GetFileDirectoryReports());
                fileDirectoryEnable = true;
            }
            _iteration++;

            if (_smallState)
            {

                if (_iteration == masterInterval)
                {
                    executeOrder67 = true;
                }
            }
            else
            {
                if (_iteration == 10)
                {
                    executeOrder67 = true;
                }
            }

            if (executeOrder67)
            {
                string process = null;
                string history = null;
                string directory = null;
                string screen = "";
                if (processEnable)
                {
                    _comboModel.ProcessLists = _processLists;
                    string navbar = _documentNavGenerator.GenerateNav(true, historyEnable, _configurationManagement.GetConfigModel().PrtScrnEnable, fileDirectoryEnable, 0);
                    process = _documentGenerator.DocumentPageGenerator(navbar, "Raport listy procesów dla ",
                        _deviceId, _documentTableGeneration.GenerateTable(_comboModel.ProcessLists, 0));
                }

                if (historyEnable)
                {
                    _comboModel.BrowsingHistories = _browsingHistory;
                    string navbar = _documentNavGenerator.GenerateNav(processEnable, true, _configurationManagement.GetConfigModel().PrtScrnEnable, fileDirectoryEnable, 1);
                    history = _documentGenerator.DocumentPageGenerator(navbar, "Raport historii przeglądania dla ",
                        _deviceId, _documentTableGeneration.GenerateTable(_comboModel.BrowsingHistories, 1));
                }

                if (fileDirectoryEnable)
                {
                    _comboModel.DirectoryLists = _fileDirectory;
                    string navbar = _documentNavGenerator.GenerateNav(processEnable, historyEnable, _configurationManagement.GetConfigModel().PrtScrnEnable, true, 2);
                    directory = _documentGenerator.DocumentPageGenerator(navbar, "Raport skanowanie katalogów dla ",
                        _deviceId, _documentTableGeneration.GenerateTable(_comboModel.DirectoryLists, 2));
                }

                if (_configurationManagement.GetConfigModel().PrtScrnEnable)
                {
                    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\screenshots"))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\screenshots\");
                        di.Attributes = FileAttributes.Directory;
                    }
                    _fileManagement.CopyPrintScreenFile(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\screenshots\");
                    List<Models.FileDirectory> printScrList = _fileManagement.GetPrintScrFile();


                    if (printScrList.Any())
                    {
                        string navbar = _documentNavGenerator.GenerateNav(processEnable, historyEnable, _configurationManagement.GetConfigModel().PrtScrnEnable, fileDirectoryEnable, 3);
                        screen = _documentGenerator.DocumentPageGenerator(navbar, "Raport aktywności ",
                            _deviceId, _documentTableGeneration.GenerateScreenshotsTable(printScrList));
                    }

                }


                DataCollection collection = DataCollection.GetInstance();
                collection.GenerateReports(process, history, directory, screen, _documentGenerator.DocumentIndexGenerator(_documentNavGenerator.GenerateNav(processEnable, historyEnable, _configurationManagement.GetConfigModel().PrtScrnEnable, fileDirectoryEnable, 4)), _documentGenerator.DocumentBootstrapGenerator(), _documentGenerator.DocumentStyleGenerator(), _documentGenerator.DocumentLicenseGenerator());
                collection.PackageReports(_deviceId);
                
                _iteration = 0;
                ClearStack();
                _fileManagement.ClearDirectory();
            }

        }
    }



    /// <summary>
    /// Class for screenshots timer management.
    /// </summary>
    public class PrintScrManagementTimer
    {
        readonly PrintScrManagement _printScrManagement = new PrintScrManagement();
        private readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();
        private double _interval = 0;
        public void StartTimer(int minutes)
        {
            if (minutes > _configurationManagement.GetConfigModel().IntervalTime)
            {
                int interval = _configurationManagement.GetConfigModel().IntervalTime;
                if (interval <= 10)
                {
                    _interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                }
                else
                {
                    _interval = TimeSpan.FromMinutes(interval / 10.00).TotalMilliseconds;
                }
            }
            else
            {
                _interval = TimeSpan.FromMinutes(minutes).TotalMilliseconds;
            }
            Timer oTimer = new Timer();
            oTimer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            oTimer.Interval = _interval;
            oTimer.Enabled = true;
        }

        /// <summary>
        /// Timer using to getting a screenshots.
        /// </summary>
        /// <param name="oSource"></param>
        /// <param name="oElapsedEventArgs"></param>
        private void OnTimeEvent(object oSource, ElapsedEventArgs oElapsedEventArgs)
        {
            _printScrManagement.GetPrintScreen();
        }
    }

    /// <summary>
    /// Class for screenshot management.
    /// </summary>
    public class PrintScrFileManagement
    {
        private readonly string[] _strlist = { ".jpg" };

        /// <summary>
        /// Method using to get lists screenshots.
        /// </summary>
        /// <returns>Lists with screenshots.</returns>
        public List<Models.FileDirectory> GetPrintScrFile()
        {
            List<Models.FileDirectory> fileDirectories = new List<Models.FileDirectory>();
            foreach (var e in FileDirectory.ScanDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\screenshot\", _strlist))
            {
                fileDirectories.Add(new Models.FileDirectory(e.FullName.ToString(), e.CreationTimeUtc.ToString(), e.LastAccessTimeUtc.ToString(), e.LastWriteTimeUtc.ToString()));
            }
            return fileDirectories;
        }
        /// <summary>
        /// Delete file.
        /// </summary>
        /// <param name="name">Filename</param>
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
        /// <summary>
        /// Copy screenshots to new path.
        /// </summary>
        /// <param name="path">String using as new directions.</param>
        public void CopyPrintScreenFile(string path)
        {
            List<Models.FileDirectory> fileDirectories = GetPrintScrFile();
            foreach (var eDirectory in fileDirectories)
            {
                File.Copy(eDirectory.FullName, path + Path.GetFileName(eDirectory.FullName));
            }
        }

        /// <summary>
        /// Cleaning reports.
        /// </summary>
        public void RemoveReports()
        {
            string[] _strlist = { ".7z" };
            foreach (var e in FileDirectory.ScanDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\", _strlist))
            {
                DeleteFile(e.FullName);
            }
        }
        /// <summary>
        /// Cleaning of temporary folders.
        /// </summary>
        public void ClearDirectory()
        {
            DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\UserActivityReport\");
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            
            List<Models.FileDirectory> fileDirectories = GetPrintScrFile();
            foreach (var eDirectory in fileDirectories)
            {
                DeleteFile(eDirectory.FullName);
            }
        }
    }

    /// <summary>
    /// Class for session time limitation
    /// </summary>
    public class ShutDownTimer
    {
        readonly ConfigurationManagement _configurationManagement = ConfigurationManagement.GetInstance();
        readonly WindowsManagement _management = new WindowsManagement();
        /// <summary>
        /// Method using to start a timer.
        /// </summary>
        /// <param name="minutes">Minute to interval between execute timer.</param>
        public void StartTimer(int minutes)
        {
            Timer oTimer = new Timer();
            oTimer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            oTimer.Interval = TimeSpan.FromMinutes(minutes).TotalMilliseconds;
            oTimer.Enabled = true;
            //_management.DisplayInformationMessageBox("Na tym komputerze uruchomiono ograniczenie czasu pracy. Pozostało:"+ minutes + " minut do zakończenia sesji.", "Ograniczenie czasu pracy!");
        }

        /// <summary>
        /// Timer for limiting working time 
        /// </summary>
        /// <param name="oSource"></param>
        /// <param name="oElapsedEventArgs"></param>
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

        /// <summary>
        /// Getting web browsing history.
        /// </summary>
        /// <returns>Lists with browsing history.</returns>
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

        /// <summary>
        /// Taking a screenshot
        /// </summary>
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

        /// <summary>
        /// Getting information about files from directories.
        /// </summary>
        /// <returns>Lists with files information.</returns>
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
        /// <summary>
        /// Getting a process list.
        /// </summary>
        /// <returns> Lists with process.</returns>
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