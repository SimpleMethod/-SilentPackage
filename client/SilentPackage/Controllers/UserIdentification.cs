/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace SilentPackage.Controllers
{
    /// <summary>
    /// Class for  getting device ID.
    /// </summary>
    internal class UserIdentification
    {
        /// <summary>
        /// Building a check sum.
        /// </summary>
        /// <param name="data">Data for calculating the checksum.</param>
        /// <returns>Text as a checksum.</returns>
        private string HashData(string data)
        {

            byte[] hash;
            using (MD5 md5 = MD5.Create())
            {
                md5.Initialize();
                md5.ComputeHash(Encoding.ASCII.GetBytes(data));
                hash = md5.Hash;
            }
            data = Convert.ToBase64String(hash).ToString();
            return data.Substring(0, Math.Min(data.Length, 6));

        }
        /// <summary>
        /// Generation of random numbers.
        /// </summary>
        /// <returns> Return random numbers.</returns>
        private int RandomNumbers()
        {
            Random random = new Random();
            int result = 0;
            for (int i = 0; i < 4; ++i)
            {
                result = result + random.Next(0, 9999);
            }
            return result;
        }
        /// <summary>
        /// Getting device ID.
        /// </summary>
        /// <returns>Return device ID.</returns>
        public string GetMachineID()
        {
            string Data = "";
            try
            {

                var cpu = new ManagementObjectSearcher("Select * From Win32_processor");
                var populist = cpu.Get();

                foreach (var o in populist)
                {
                    var cpuL = (ManagementObject)o;
                    Data += (string)cpuL["ProcessorID"];
                }
                var motherboardSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                using var baseMotherboardCollection = motherboardSearcher.Get();
                foreach (var o in baseMotherboardCollection)
                {
                    var baseL = (ManagementObject)o;
                    Data += (string)baseL["SerialNumber"];
                }
            }
            catch (ManagementException e)
            {
                Data = RandomNumbers().ToString();
            }
            return HashData(Data);
        }
    }
}
