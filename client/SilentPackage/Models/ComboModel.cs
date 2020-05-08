/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.Text;
using SilentPackage.Controllers;

namespace SilentPackage.Models
{
    public class ComboModel
    {
        public Stack<BrowsingHistoryLists> BrowsingHistories { get; set; }
        public Stack<FileDirectoryList> DirectoryLists { get; set; }
        public Stack<ProcessList> ProcessLists { get; set; }
    }
}
