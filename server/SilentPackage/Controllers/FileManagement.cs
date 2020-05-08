/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.IO;

namespace SilentPackage.Controllers
{
    public class FileManagement
    {
        /// <summary>
        /// Create main dir for server.
        /// </summary>
        public void CreateDir()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\"))
            {
                DirectoryInfo di = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\");
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }
    }
}
