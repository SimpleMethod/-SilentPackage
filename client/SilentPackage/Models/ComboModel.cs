using System;
using System.Collections.Generic;
using System.Text;
using SilentPackage.Controllers;

namespace SilentPackage.Models
{
    public class ComboModel
    {
        public List<BrowsingHistoryLists> BrowsingHistories { get; set; }
        public List<FileDirectoryList> DirectoryLists { get; set; }
        public List<ProcessList> ProcessLists { get; set; }
    }
}
