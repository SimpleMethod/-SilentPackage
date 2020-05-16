/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using SilentPackage.Models;

namespace SilentPackage.Controllers
{
    class ConfigurationManagement
    {
        private readonly FileManagement _fileManagement = new FileManagement();
        ConfigModel _configModel;
        private static ConfigurationManagement _mOInstance = null;
        private static Object _mutex = new Object();
        private static string _deviceId = null;
        public static ConfigurationManagement GetInstance()
        {

            if (_mOInstance == null)
            {
                lock (_mutex)
                {
                    if (_mOInstance == null)
                    {
                        _mOInstance = new ConfigurationManagement();


                        bool fileExist = File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\debug.bin");
                        _deviceId = fileExist ? File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\debug.bin") : new UserIdentification().GetMachineID(); 

                    }
                }
            }
            return _mOInstance;
        }
        private ConfigurationManagement() => _configModel = JsonSerializer.Deserialize<ConfigModel>(_fileManagement.ReadBinaryFile(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\config.bin"));

        public ConfigModel GetConfigModel()
        {
            return _configModel;
        }

        public string GetDeviceId()
        {
            return _deviceId;
        }
    }
}
