/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace SilentPackage.Controllers
{

    /// <summary>
    /// Class managing Windows power options.
    /// </summary>
    internal class WindowsManagement
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPrivLuid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        public struct ProcessesList
        {
            private readonly string _name;
            private readonly int _id;
            private readonly string _startTime;


            public ProcessesList(string name, int id, string startTime)
            {
                _name = name;
                _id = id;
                _startTime = startTime;
            }

            public string GetName()
            {
                return _name;
            }

            public int GetId()
            {
                return _id;
            }

            public string GetStartTime()
            {
                return _startTime;
            }

        }


        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr h, string m, string c, long type);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPrivLuid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool ExitWindowsEx(int flg, int rea);

        /// <summary>
        /// Returns a list of active processes.
        /// </summary>
        /// <returns> List of active processes</returns>
        public List<ProcessesList> GetProcesses()
        {
            List<ProcessesList> processes = new List<ProcessesList>();

            foreach (var preprocess in Process.GetProcesses())
            {
                try
                {
                    var processName = preprocess.ProcessName;
                    var processId = preprocess.Id;
                    var startTime = "";
                    try
                    {
                        startTime = preprocess.StartTime.ToString();
                    }
                    catch (Win32Exception e)
                    {
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }
                    processes.Add(new ProcessesList(processName, processId, startTime));
                }
                catch (Exception e)
                {
                    // Debug.WriteLine(e);
                    // throw;
                }
            }
            return processes;
        }



        /// <summary>
        ///  Getting process lists.
        /// </summary>
        /// <param name="unique">If true returns only unique process.</param>
        /// <returns> List of active processes</returns>
        [Obsolete]
        public List<ProcessesList> GetProcessListPs(bool unique)
        {
            List<ProcessesList> processes = new List<ProcessesList>();
            using var ps = PowerShell.Create();
            ps.AddScript(unique ? "ps | select -unique" : "ps");
            Collection<PSObject> results = ps.Invoke();
     
            foreach (var result in results)
            {
                var baseObj = result.BaseObject;
                if (baseObj is Process preprocess)
                {
                    try
                    {
                        var processName = preprocess.ProcessName;
                        var processId = preprocess.Id;
                        var startTime = "Access denied";
                        try
                        {
                            startTime = preprocess.StartTime.ToString(CultureInfo.InvariantCulture);
                        }
                        catch (Win32Exception e)
                        {
                           
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                          
                        }
                        processes.Add(new ProcessesList(processName, processId, startTime));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                       
                        return processes;
                    }
                    catch (InvalidOperationException e)
                    {
                      
                        return processes;
                    }

                }

            }
            return processes;
        }

        /// <summary>
        ///Displaying notification in messages box with info icon.
        /// </summary>
        /// <param name="message">Information text.</param>
        /// <param name="title">Title of the message box.</param>
        /// <returns>Messages box status</returns>
        public int DisplayInformationMessageBox(string message, string title)
        {
            return MessageBox((IntPtr)0, message, title, 0x00000040L);
        }
        /// <summary>
        ///Displaying notification in messages box with warning icon.
        /// </summary>
        /// <param name="message">Information text.</param>
        /// <param name="title">Title of the message box.</param>
        /// <returns>Messages box status</returns>
        public int DisplayWarningMessageBox(string message, string title)
        {
            return MessageBox((IntPtr)0, message, title, 0x00000030L);
        }
        /// <summary>
        ///Displaying notification in messages box with error icon. 
        /// </summary>
        /// <param name="message">Information text.</param>
        /// <param name="title">Title of the message box.</param>
        /// <returns>Messages box status</returns>
        public int DisplayErrorMessageBox(string message, string title)
        {
            return MessageBox((IntPtr)0, message, title, 0x00000010L);
        }

        ///
        ///

        /// <summary>
        /// A method to manage the process of powering the operating system.
        /// </summary>
        /// <param name="flag">Power option selection flag.</param>
        protected void WindowsEXT(int flag)
        {
            TokPrivLuid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            OpenProcessToken(hproc, 0x00000020 | 0x00000008, ref htok);
            tp.Count = 1; tp.Luid = 0; tp.Attr = 0x00000002;
            LookupPrivilegeValue(null, "SeShutdownPrivilege", ref tp.Luid);
            AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            ExitWindowsEx(flag | 0x00000004 | 0x00000010, 0);
        }

        /// <summary>
        /// User logout.
        /// </summary>
        public void Logout()
        {
            WindowsEXT(0x00000000);
        }

        /// <summary>
        /// Operating system shutdown.
        /// </summary>
        public void Shutdown()
        {
            WindowsEXT(0x00000001);

        }

        /// <summary>
        /// Turning off computer power.
        /// </summary>
        public void PowerOff()
        {
            WindowsEXT(0x00000008);
        }

        /// <summary>
        /// Reboot operating system.
        /// </summary>
        public void Reboot()
        {
            WindowsEXT(0x00000002);

        }
    }
}

