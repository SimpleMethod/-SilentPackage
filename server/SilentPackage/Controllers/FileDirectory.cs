/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SilentPackage.Controllers
{
    /// <summary>
    /// Structure for storing file information.
    /// </summary>
    internal struct Files
    {
        public string FullName;
        public string CreationTimeUtc;
        public string LastAccessTimeUtc;
        public string LastWriteTimeUtc;
    }

    /// <summary>
    /// Class to list directories and files from specified locations.
    /// </summary>
    internal static class FileDirectory
    {

        /// <summary>
        /// Method for getting information about files and stored into list.
        /// </summary>
        /// <param name="directory">
        /// Windows compatible directory access path.
        /// </param>
        /// Used extensions used during a search files.
        /// <param name="extensions">
        /// </param>
        private static List<Files> GetFiles(string directory, params string[] extensions)
        {
            List<Files> files = new List<Files>();
            var file = new Files();
            try
            {
                var directoryInfo = new DirectoryInfo(directory);
                directoryInfo.GetFiles();
                foreach (var e in directoryInfo.GetFilesByExtensions(extensions))
                {
                    file.FullName = e.FullName;
                    file.CreationTimeUtc = DateToDate(e.CreationTimeUtc.ToString(CultureInfo.InvariantCulture));
                    file.LastAccessTimeUtc = DateToDate(e.LastAccessTimeUtc.ToString(CultureInfo.InvariantCulture));
                    file.LastWriteTimeUtc = DateToDate(e.LastWriteTimeUtc.ToString(CultureInfo.InvariantCulture));
                    files.Add(file);
                }
                return files;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
                return files;
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e);
                return files;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return files;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return files;
            }

        }

        /// <summary>
        /// Method for returning a list with file information.
        /// </summary>
        /// <returns>
        /// List of structures.
        /// </returns>
        [Obsolete("This method has been deprecated. Use GetFiles instead.", true)]
        public static List<Files> PrintFiles()
        {
            // return Files;
            return null;
        }

        /// <summary>
        /// Method for cleaning the list.
        /// </summary>
        [Obsolete("This method has been deprecated. Use GetFiles instead.", true)]
        public static void ClearFiles()
        {
            //  Files.Clear();
        }

        /// <summary>
        /// The method used to download subdirectories in the path.
        /// </summary>
        /// <param name="directory">
        /// Windows compatible directory access path.
        /// </param>
        /// <returns>
        /// Table with paths to subfolders
        /// </returns>
        private static string[] GetSubDirectory(string directory)
        {
            string[] error = { "-1" };
            try
            {
                return Directory.GetDirectories(directory);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e);
                return error;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return error;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return error;
            }
            finally
            {

            }
        }
        /// <summary>
        ///  Method used to list files from the directory and subdirectories in the given path. 
        /// </summary>
        /// <param name="directory">
        /// Windows compatible directory access path.
        /// </param>
        /// Used extensions used during a search files.
        /// <param name="extensions">
        /// </param>
        public static List<Files> ScanDirectory(string directory, string[] extensions)
        {
            List<Files> files = new List<Files>();
            var file = new Files();
            var subdirectories = GetSubDirectory(directory);
            if (subdirectories.Length == 0)
            {
                return GetFiles(directory, extensions);

            }
            else
            {
                files = GetFiles(directory, extensions);
                foreach (var s in subdirectories)
                {
                    try
                    {
                        foreach (var f in GetFiles(s, extensions))
                        {
                            file.FullName = f.FullName;
                            file.CreationTimeUtc = f.CreationTimeUtc;
                            file.LastAccessTimeUtc = f.LastAccessTimeUtc;
                            file.LastWriteTimeUtc = f.LastAccessTimeUtc;
                            files.Add(file);
                        }
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine(e);
                    }

                }

                return files;
            }

        }

        /// <summary>
        /// Getting information about removable drives.
        /// </summary>
        /// <returns>IEnumerable with DriveInfo object.</returns>
        private static IEnumerable<DriveInfo> GetRemovableDrives()
        {
            try
            {
                return DriveInfo.GetDrives().AsEnumerable().Where(drive => drive.DriveType == DriveType.Removable);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return null;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
                return null;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Getting information about removable drivers. 
        /// </summary>
        /// <returns>Lists with removable drivers.</returns>
        private static List<String> GetAllRemovableDrives()
        {
            var usbDevice = GetRemovableDrives();
            var driveInfos = new List<DriveInfo>();
            List<string> driveList = new List<string>();

            foreach (var usbDrive in usbDevice)
            {
                driveInfos.Add(usbDrive);

            }

            if (driveInfos.Count != 0)
            {
                foreach (var usbDrive in driveInfos)
                {
                    driveList.Add(usbDrive.Name);
                }
                return driveList;
            }
            else
            {
                driveList.Add(@"-");
                return driveList;
            }

        }

        /// <summary>
        /// Scan removable drivers with patter extensions.
        /// </summary>
        /// <param name="extensions">Array with extensions using to search file.</param>
        /// <returns>Lists with files matching with specific extensions.</returns>
        public static List<Files> ScanRemovableDrives(string[] extensions)
        {
            List<Files> files = new List<Files>();
            var file = new Files();
            foreach (var drivers in GetAllRemovableDrives())
            {
                if (drivers.First().ToString().Equals("-"))
                {
                    return files;
                }

                var subdirectories = GetSubDirectory(drivers);
                if (subdirectories.Length == 0)
                {
                    return GetFiles(drivers, extensions);

                }
                else
                {
                    files.AddRange(GetFiles(drivers, extensions));
                    foreach (var s in subdirectories)
                    {
                        try
                        {
                            foreach (var f in GetFiles(s, extensions))
                            {
                                file.FullName = f.FullName;
                                file.CreationTimeUtc = f.CreationTimeUtc;
                                file.LastAccessTimeUtc = f.LastAccessTimeUtc;
                                file.LastWriteTimeUtc = f.LastAccessTimeUtc;
                                files.Add(file);
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            Console.WriteLine(e);
                        }

                    }

                }

            }
            return files;
        }

        /// <summary>
        /// Converts date to an understandable format dd.mm.yyyy
        /// </summary>
        /// <param name="input">
        /// Date in original format 
        /// </param>
        /// <returns>
        /// Date in corrected format
        /// </returns>
        private static string DateToDate(string input)
        {
            string[] words = input.Split('/');
            return words[1] + "." + words[0] + "." + words[2];
        }
        private static IEnumerable<FileSystemInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            try
            {
                var files = dir.EnumerateFileSystemInfos();
                return files.Where(f => extensions.Contains(f.Extension));

            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
                return null;
            }

        }
    }
}
