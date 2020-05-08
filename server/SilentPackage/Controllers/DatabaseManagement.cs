/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using SilentPackage.Models;

namespace SilentPackage.Controllers
{
    public sealed class DatabaseManagement
    {

        private static DatabaseManagement _mOInstance = null;
        private static Object _mutex = new Object();
        public SqliteConnection _sqliteConnection;
        public static DatabaseManagement GetInstance(string name, string path)
        {

            if (_mOInstance == null)
            {
                lock (_mutex)
                {
                    if (_mOInstance == null)
                    {
                        _mOInstance = new DatabaseManagement(name, path);
                    }
                }
            }

            return _mOInstance;
        }

        ~DatabaseManagement()
        {
            CloseDatabase();
        }
        private DatabaseManagement(string name, string path)
        {
            _sqliteConnection = OpenDatabase(name, path);

        }

        private SqliteConnection OpenDatabase(string name, string path)
        {
#if RELEASE
            try
            {
                var temp = Process.GetCurrentProcess().MainModule;
                if (temp != null)
                {
                    path = Path.GetDirectoryName(temp.FileName);
                }
                else
                {
                    return  null;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            Console.WriteLine(path);
#endif
            var sqliteConnection = new SqliteConnection("Data Source=" + @path + @"\" + @name);
            try
            {
                sqliteConnection.Open();
            }
            catch (SqliteException e)
            {
                Console.WriteLine(e.ToString());
                ; throw;
            }
            return sqliteConnection;
        }

        /// <summary>
        /// Closing connection to the database.
        /// </summary>
        private void CloseDatabase()
        {
            _sqliteConnection.Close();
            _sqliteConnection.Dispose();
        }


        public UsersModel GetUser(string license)
        {
            string saltedLicense = HashData(ProtectLicenseKey(license));
            UsersModel usersModel = new UsersModel();
            StringBuilder sqlQueryBuilder = new StringBuilder("SELECT * FROM users WHERE users.license=\"$\" LIMIT 1;");
            sqlQueryBuilder.Replace("$", saltedLicense);
            var dbCommand = new SqliteCommand(sqlQueryBuilder.ToString(), _sqliteConnection);
            SqliteDataReader dataReader = dbCommand.ExecuteReader();
            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1))
                {
                    usersModel.Id = dataReader.GetInt32(0);
                    usersModel.License = dataReader.GetString(1);
                }

                try
                {
                    usersModel.DeviceId = !dataReader.IsDBNull(2) ? dataReader.GetString(2) : "-1";
                }
                catch (InvalidOperationException e)
                {
                    usersModel.DeviceId = "-1";
                }
            }
            dbCommand.Dispose();
            return usersModel;
        }

        public List<UsersModel> GetUsers()
        {
            List<UsersModel> usersModels = new List<UsersModel>();
        
            StringBuilder sqlQueryBuilder = new StringBuilder("SELECT * FROM users;");
            var dbCommand = new SqliteCommand(sqlQueryBuilder.ToString(), _sqliteConnection);
            SqliteDataReader dataReader = dbCommand.ExecuteReader();
            while (dataReader.Read())
            {
                UsersModel usersModel = new UsersModel();
                if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1))
                {
                    usersModel.Id = dataReader.GetInt32(0);
                    usersModel.License = dataReader.GetString(1);
                }
                try
                {
                    usersModel.DeviceId = !dataReader.IsDBNull(2) ? dataReader.GetString(2) : null;
                }
                catch (InvalidOperationException)
                {
                    usersModel.DeviceId = null;
                }
                usersModels.Add(usersModel);
            }
            dbCommand.Dispose();
            return usersModels;
        }



        public void CreateUser(string license)
        {
            string saltedLicense = HashData(ProtectLicenseKey(license));
            UsersModel usersModel = new UsersModel();
            StringBuilder sqlQueryBuilder = new StringBuilder("INSERT INTO users(license) VALUES (\"$\")");
            sqlQueryBuilder.Replace("$", saltedLicense);
            var dbCommand = new SqliteCommand(sqlQueryBuilder.ToString(), _sqliteConnection);
            SqliteDataReader dataReader = dbCommand.ExecuteReader();
            dbCommand.Dispose();
        }

        public void UpdateDeviceIdUser(string license, string deviceId)
        {
            string saltedLicense = HashData(ProtectLicenseKey(license));
            UsersModel usersModel = new UsersModel();
            StringBuilder sqlQueryBuilder = new StringBuilder("UPDATE users SET deviceid = \"&\" WHERE license= \"$\";");
            sqlQueryBuilder.Replace("$", saltedLicense);
            sqlQueryBuilder.Replace("&", ProtectLicenseKey(deviceId));
            var dbCommand = new SqliteCommand(sqlQueryBuilder.ToString(), _sqliteConnection);
            SqliteDataReader dataReader = dbCommand.ExecuteReader();
            dbCommand.Dispose();
        }

        public void DeleteUser(string license)
        {
            string saltedLicense = HashData(ProtectLicenseKey(license));
            UsersModel usersModel = new UsersModel();
            StringBuilder sqlQueryBuilder = new StringBuilder("DELETE FROM users WHERE users.license=\"$\" ");
            sqlQueryBuilder.Replace("$", saltedLicense);
            var dbCommand = new SqliteCommand(sqlQueryBuilder.ToString(), _sqliteConnection);
            SqliteDataReader dataReader = dbCommand.ExecuteReader();
            dbCommand.Dispose();
        }

        public string HashData(string data)
        {
            using var sha256 = SHA256.Create();
            var saltedData = $"{"ct3sw5zj"}{data}";
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedData)));
        }

        public string ProtectLicenseKey(string data)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9=]+");
            data = rgx.Replace(data, "");
            return data;
        }
    }
}
