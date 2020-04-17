using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace SilentPackage.Controllers
{
    /// <summary>
    /// A class used to block programs.
    /// </summary>
    public sealed class BlockingPrograms
    {
        private static BlockingPrograms _mOInstance = null;
        private static Object _mutex = new Object();


        public static BlockingPrograms GetInstance()
        {

            if (_mOInstance == null)
            {
                lock (_mutex)
                {
                    if (_mOInstance == null)
                    {
                        _mOInstance = new BlockingPrograms();
                    }
                }
            }
            return _mOInstance;
        }


        public static BlockingPrograms GetInstance(List<string> programsList)
        {
            if (programsList == null) throw new ArgumentNullException(nameof(programsList));
            if (_mOInstance == null)
            {
                lock (_mutex)
                {
                    if (_mOInstance == null)
                    {
                        _mOInstance = new BlockingPrograms(programsList);
                    }
                }
            }
            return _mOInstance;
        }

        ~BlockingPrograms()
        {
            //DisablePolicy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programsList"></param>
        private BlockingPrograms(List<string> programsList)
        {
            if (RegNodeExist() == false)
            {
                Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\", true).SetValue("Explorer", 1, RegistryValueKind.DWord);
            }

            DisablePolicy();
            EnablePolicy();
            foreach (var variable in programsList)
            {
                AddProgram(variable);
            }
        }

        private BlockingPrograms()
        {
            DisablePolicy();
            EnablePolicy();
        }


        /// <summary>
        /// Activating a policy to block programs.
        /// </summary>
        /// <returns>Status of the method.</returns>
        public bool EnablePolicy()
        {
            if (RegNodeExist() != true || RegKeyExist() != true)
            {
                try
                {
                    RegistryKey key;
                    key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\", true);
                    key.SetValue("Explorer", 1, RegistryValueKind.DWord);
                    key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\", true);
                    key.SetValue("DisallowRun", 1, RegistryValueKind.DWord);
                    key.Close();
                    key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun", true);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Deactivation policy.
        /// </summary>
        /// <returns>Status of the method.</returns>
        public bool DisablePolicy()
        {

            if (RegNodeExist() == true)
            {
                if (RegKeyExist() == true)
                {
                    try
                    {
                        RegistryKey key;
                        key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\", true);
                        key.DeleteValue("DisallowRun");
                        key.Close();
                        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun\");
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return false;
                    }
                }
                return true;
            }
            return true;
        }



        private bool RegNodeExist()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies", true);
                if (key != null)
                {
                    Console.WriteLine(key.GetValueNames().Contains("Explorer"));
                    return (key.GetValueNames().Contains("Explorer"));
                }

                return false;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }



        /// <summary>
        /// Method for checking whether the key is exists.
        /// </summary>
        /// <returns>Status of the method.</returns>
        private bool RegKeyExist()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\", true);
                if (key != null)
                {
                    return (key.GetValueNames().Contains("DisallowRun"));
                }

                return false;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }


        /// <summary>
        /// Method for checking whether the key is exists.
        /// </summary>
        /// <param name="name"> key</param>
        /// <returns>Status of the method.</returns>
        private bool RegKeyExist(string name)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun\", true);
                if (key != null)
                {
                    return (key.GetValueNames().Contains(name));
                }
                return false;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Adding a program to the list of blocked programs.
        /// </summary>
        /// <param name="name"> Name of exe programs.</param>
        /// <returns>Status of the method.</returns>
        public bool AddProgram(string name)
        {
            try
            {
                if (RegKeyExist(name) != true)
                {
                    RegistryKey key;
                    key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun\", true);
                    key.SetValue(name, name);
                    key.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Removing a program to the list of blocked programs.
        /// </summary>
        /// <param name="name"> Name of exe programs.</param>
        /// <returns>Status of the method.</returns>
        public bool RemoveProgram(string name)
        {
            try
            {
                if (RegKeyExist(name) == true)
                {
                    RegistryKey key;
                    key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun\", true);
                    key.DeleteValue(name);
                    key.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

    }
}
