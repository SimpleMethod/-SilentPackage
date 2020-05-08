/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace SilentPackage.Models
{
    public class FileDirectoryList
    {
        public List<FileDirectory> FileDirectories { get; set; }
        public long Timestamp { get; set; }
    }
    public class FileDirectory
    {
        public FileDirectory(string fullName, string creationTimeUtc, string lastAccessTimeUtc, string lastWriteTimeUtc)
        {
            FullName = fullName;
            CreationTimeUtc = creationTimeUtc;
            LastAccessTimeUtc = lastAccessTimeUtc;
            LastWriteTimeUtc = lastWriteTimeUtc;
        }

        public string FullName { get; set; }
        public string CreationTimeUtc { get; set; }
        public string LastAccessTimeUtc { get; set; }
        public string LastWriteTimeUtc { get; set; }
    }
}
