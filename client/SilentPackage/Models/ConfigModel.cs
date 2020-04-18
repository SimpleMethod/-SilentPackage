using System;
using System.Collections.Generic;
using System.Text;

namespace SilentPackage.Models
{
    class ConfigModel
    {
        public bool ShutDownEnable { get; set; }
        public string ShutDownOption { get; set; }
        public int ShutDownTime { get; set; }
        public bool ListProcessesEnable { get; set; }
        public List<string> ProgramBlockList { get; set; }
        public bool WebHistoryEnable { get; set; }
        public int WebHistoryQueryLimit { get; set; }
        public string WebHistoryPath { get; set; }
        public bool PrtScrnEnable { get; set; }
        public string PrtScrnQualityOption { get; set; }
        public int PrtScrInterval { get; set; }
        public List<string> FileDirectoryList { get; set; }
        public string FileDirectoryExtension { get; set; }
        public bool RemovableDevicesEnable { get; set; }

        public int IntervalTime { get; set; }
        public bool OfflineMode { get; set; }
        public string AddressCc { get; set; }
        public string License { get; set; }


    }
}
