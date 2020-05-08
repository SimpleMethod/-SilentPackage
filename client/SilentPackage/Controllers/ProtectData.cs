/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation.Host;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace SilentPackage.Controllers
{
    internal class EncryptDataHandler : EncryptData
    {
        private readonly string _pathToKey;
        private readonly FileManagement _fileManagement = new FileManagement();
        public EncryptDataHandler(string pathToKey)
        {
            _pathToKey = pathToKey;
        }


        public void LoadPair(string path, ref byte[] key, ref byte[] IV)
        {
            try
            {
                key = _fileManagement.ReadBinaryFile(path + @"\key_part_1.bin");
                IV = _fileManagement.ReadBinaryFile(path + @"\key_part_2.bin");
            }
            catch (DirectoryNotFoundException e)
            {
               // Console.WriteLine(e);
                //throw;
            }
        }

        public bool EncryptFileData(string data, string path, string filename, bool force)
        {
            byte[] key = null; byte[] IV = null;
            LoadPair(_pathToKey, ref key, ref IV);

            if (key == null || IV == null)
            {
                return false;
            }

            byte[] encryptAes = EncryptAes(data, key, IV);
            _fileManagement.CreateFile(path + @"\", filename, encryptAes, force);
            return File.Exists(path + @"\" + filename);
        }

        public byte[] EncryptText(string data)
        {
            byte[] key = null; byte[] IV = null;
            LoadPair(_pathToKey, ref key, ref IV);

            if (key == null || IV == null)
            {
                return null;
            }

            return EncryptAes(data, key, IV);
        }

        public bool CreatePair(string path, bool force)
        {
            _fileManagement.CreateFile(path, "key_part_1.bin", CreateKeyBytes(), force);
            _fileManagement.CreateFile(path, "key_part_2.bin", CreateIVBytes(), force);

            if (path == null)
            {
                return File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\key_part_1.bin") && !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\key_part_2.bin");
            }
            return File.Exists(path + @"\key_part_1.bin") && File.Exists(path + @"\key_part_2.bin");
        }
    }

    internal class DecryptDataHandler : DecryptData
    {
        private readonly string _pathToKey;
        private readonly FileManagement _fileManagement = new FileManagement();
        public DecryptDataHandler(string pathToKey)
        {
            _pathToKey = pathToKey;
        }



        public void LoadPair(string path, ref byte[] key, ref byte[] IV)
        {
            key = _fileManagement.ReadBinaryFile(path + @"\key_part_1.bin");
            IV = _fileManagement.ReadBinaryFile(path + @"\key_part_2.bin");
        }

        public string DecryptFileData(string path, string filename)
        {
            byte[] key = null; byte[] IV = null;
            LoadPair(_pathToKey, ref key, ref IV);
            if (key == null || IV == null)
            {
                return null;
            }
            return DecryptAes(_fileManagement.ReadBinaryFile(path + @"\"+ filename), key, IV);
        }

        public string DecryptText(byte[] data)
        {
            byte[] key = null; byte[] IV = null;
            LoadPair(_pathToKey, ref key, ref IV);
            if (key == null || IV == null)
            {
                return null;
            }
            return DecryptAes(data, key, IV);
        }
    }

    internal class EncryptData
    {
        protected byte[] CreateKeyBytes()
        {
            AesManaged aes = new AesManaged { BlockSize = 128, KeySize = 256 };
            aes.GenerateKey();
            return aes.Key;
        }

        protected byte[] CreateIVBytes()
        {
            AesManaged aes = new AesManaged { BlockSize = 128, KeySize = 256 };
            aes.GenerateIV();
            return aes.IV;
        }

        protected byte[] EncryptAes(string data, byte[] key, byte[] IV)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, aes.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                using (var sw = new StreamWriter(cs)) sw.Write(data);
                encrypted = ms.ToArray();
            }

            return encrypted;
        }
    }


    internal class DecryptData
    {
        protected string DecryptAes(byte[] dataBytes, byte[] key, byte[] IV)
        {
            string returnText = null;
            using (var aes = new AesManaged())
            {
                using var ms = new MemoryStream(dataBytes);
                using var cs = new CryptoStream(ms, aes.CreateDecryptor(key, IV), CryptoStreamMode.Read);
                using var reader = new StreamReader(cs);
                returnText = reader.ReadToEnd();
            }
            return returnText;
        }
    }

    public class FileManagement
    {
        public void CreateFile(string path, string name, byte[] data, bool force)
        {
            if (path == null)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\data\";
            }

            if (!Directory.Exists(path))
            {
                var di = Directory.CreateDirectory(path);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            path += name;
            if (force)
            {
                File.Delete(path);
            }
            else
            {
                if (File.Exists(path))
                {
                    return;
                }
            }
            try
            {
                using FileStream fs = File.Create(path);
                fs.Write(data, 0, data.Length);
            }
            catch (UnauthorizedAccessException ae)
            {

            }
            catch (ArgumentNullException ne)
            {

            }
        }

        public byte[] ReadBinaryFile(string path)
        {
            try
            {
              return  File.ReadAllBytes(path);
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
        } 
        public string[] ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }

        public string Base64Encode(byte[] data) => Convert.ToBase64String(data);

        public byte[] Base64Decode(string data) => Convert.FromBase64String(data);

    }
}