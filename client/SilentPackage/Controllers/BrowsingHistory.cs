using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Data.Sqlite;

namespace SilentPackage.Controllers
{
    /// <summary>
    /// A structure for saving information about the websites visited.
    /// </summary>
    public struct BrHistory
    {
        private readonly string _url;
        private readonly string _title;
        private readonly long _lastVisitTime;
        private readonly long _durationTime;

        public BrHistory(string url, string title, long lastVisitTime, long durationTime)
        {
            _lastVisitTime = lastVisitTime;
            _title = title;
            _url = url;
            _durationTime = durationTime;
        }

        public string GetUrl()
        {
            return _url;
        }

        public string GetTitle()
        {
            return _title;
        }

        public long GetLastVisitTime()
        {
            return _lastVisitTime;
        }
        public long GetDurationTime()
        {
            return _durationTime;
        }
    }

    /// <summary>
    /// Structure for saving profile location.
    /// </summary>
    struct Profile
    {
        private readonly string _name;
        private readonly string _path;

        public Profile(string name, string path)
        {
            this._name = name;
            this._path = path;
        }

        public string GetName()
        {
            return _name;
        }
        public string GePath()
        {
            return _path;
        }

    }

    /// <summary>
    /// A class for saving web browsing history.
    /// </summary>
    public sealed class BrowsingHistory
    {
        private Profile _accountProfile;
        private static BrowsingHistory _mOInstance = null;
        private static Object _mutex = new Object();

        public static BrowsingHistory GetInstance(string name, string path)
        {

            if (_mOInstance == null)
            {
                lock (_mutex)
                {
                    if (_mOInstance == null)
                    {
                        _mOInstance = new BrowsingHistory(name, path);
                    }
                }
            }
            return _mOInstance;
        }
        private BrowsingHistory(string name, string path) => _accountProfile = new Profile(name, path);



        /// <summary>
        /// Getting a file name from the specified path.
        /// </summary>
        /// <param name="path"> path to file.</param>
        /// <returns>File name.</returns>
        private string GetFileName(string path)
        {
            try
            {
                return Path.GetFileName(path);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        /// <summary>
        /// Creating a backup of the database.
        /// </summary>
        /// <returns>Status of the operation.</returns>
        private bool GetDataBase()
        {
            return MakeDirectory() && GetCopy();
        }
        /// <summary>
        /// Deletes a copy of the database.
        /// </summary>
        /// <returns>Status of the operation.</returns>
        private bool RemoveCopy()
        {
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\" + _accountProfile.GetName() + @"\History_COPY.db");
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\" + _accountProfile.GetName());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;

            }
        }
        /// <summary>
        /// Creates a folder for the specified profile.
        /// </summary>
        /// <returns>Status of the operation.</returns>
        private bool MakeDirectory()
        {
            try
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\" + _accountProfile.GetName());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        /// <summary>
        /// Create a backup of the database.
        /// </summary>
        /// <returns>Status of the operation.</returns>
        private bool GetCopy()
        {
            if (File.Exists(_accountProfile.GePath() + @"\History.*"))
            {
                return false;
            }
            try
            {
                File.Copy(_accountProfile.GePath() + @"\History", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\" + _accountProfile.GetName() + @"\History_COPY.db", true);
                Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }

        /// <summary>
        /// Opens and executes operations on the database. 
        /// </summary>
        /// <param name="startRange">Initial search range specified in the timestamp.</param>
        /// <param name="stopRange">End of search range specified in the timestamp.</param>
        /// <returns>Status of the operation.</returns>
        public List<BrHistory> GetHistoryFromDatabase(long startRange, long stopRange)
        {
            var bhHistory = new BrHistory();
            List<BrHistory> bHistories = new List<BrHistory>();
            SqliteConnection sqliteConnection;
            SqliteCommand dbCommand;
            try
            {

                StringBuilder SQLqueryStandard = new StringBuilder("SELECT urls.url,urls.title, (visits.visit_time / 1000000 + (strftime('%s', '1601-01-01'))), visits.visit_duration FROM visits INNER JOIN urls ON visits.url=urls.id WHERE visits.visit_time  BETWEEN $ AND ^;");
                SQLqueryStandard.Replace("$", startRange.ToString());
                SQLqueryStandard.Replace("^", stopRange.ToString());
                if (GetDataBase())
                {
                    sqliteConnection = new SqliteConnection("Data Source=" +
                                                            Environment.GetFolderPath(Environment.SpecialFolder
                                                                .LocalApplicationData) + @"\SP\" +
                                                            _accountProfile.GetName() + @"\History_COPY.db");
                    sqliteConnection.Open();
                    dbCommand = new SqliteCommand(SQLqueryStandard.ToString(), sqliteConnection);
                    SqliteDataReader dataReader = dbCommand.ExecuteReader();
                    while (dataReader.Read())
                    {
                        bHistories.Add(new BrHistory(dataReader.GetString(0), dataReader.GetString(1),
                            dataReader.GetInt64(2), dataReader.GetInt64(3)));
                    }

                    sqliteConnection.Close();
                    sqliteConnection.Dispose();
                    dbCommand.Dispose();
                    return bHistories;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception f)
            {
                Console.WriteLine(f);
                throw;
            }
            finally
            {
                RemoveCopy();
            }
        }

        /// <summary>
        /// Opens and executes operations on the database. 
        /// </summary>
        /// <param name="limit">Limit of last records from the database.</param>
        /// <returns>Status of the operation.</returns>
        public List<BrHistory> GetHistoryFromDatabase(long limit)
        {

            var bhHistory = new BrHistory();
            List<BrHistory> bHistories = new List<BrHistory>();
            try
            {

                StringBuilder SQLqueryStandard = new StringBuilder("SELECT urls.url,urls.title, (visits.visit_time / 1000000 + (strftime('%s', '1601-01-01'))), visits.visit_duration FROM visits INNER JOIN urls ON visits.url=urls.id WHERE visits.visit_time  order by visits.id desc limit $;");
                SQLqueryStandard.Replace("$", limit.ToString());
                if (GetDataBase())
                {
                    var sqliteConnection = new SqliteConnection("Data Source=" +
                                                                Environment.GetFolderPath(Environment.SpecialFolder
                                                                    .LocalApplicationData) + @"\SP\" +
                                                                _accountProfile.GetName() + @"\History_COPY.db");
                    sqliteConnection.Open();
                    var dbCommand = new SqliteCommand(SQLqueryStandard.ToString(), sqliteConnection);
                    SqliteDataReader dataReader = dbCommand.ExecuteReader();
                    while (dataReader.Read())
                    {
                        bHistories.Add(new BrHistory(dataReader.GetString(0), dataReader.GetString(1),
                            dataReader.GetInt64(2), dataReader.GetInt64(3)));
                    }

                    sqliteConnection.Close();
                    sqliteConnection.Dispose();
                    dbCommand.Dispose();
                    return bHistories;
                }
                return null;
            }
            catch (Exception f)
            {
                Console.WriteLine(f);
                throw;
            }
            finally
            {
                RemoveCopy();
            }
        }

        /*
        /// <summary>
        /// Getting the browsing history and saving it in the list.
        /// </summary>
        /// <param name="startRange">Initial search range specified in the timestamp.</param>
        /// <param name="stopRange">End of search range specified in the timestamp.</param>
        /// <returns>Status of the operation.</returns>
        public bool GetHistoryFromDatabase(long startRange, long stopRange)
        {
            Console.WriteLine(_accountProfile.GetName());
            if (GetDataBase())
            {
                if (OpenDatabase(startRange, stopRange))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Getting the browsing history and saving it in the list.
        /// </summary>
        /// <param name="limit">Limit of last records from the database.</param>
        /// <returns>Status of the operation.</returns>
        public bool GetHistoryFromDatabase(long limit)
        {

            Console.WriteLine(_accountProfile.GetName());
            if (GetDataBase())
            {
                if (OpenDatabase(limit))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the browsing history as a list.
        /// </summary>
        /// <returns>Returns the browsing history as a list.</returns>
        public List<BrHistory> GetHistory()
        {
            return bHistories;
        }

        /// <summary>
        /// Clears list.
        /// </summary>
        public void ClearHistory()
        {
            bHistories.Clear();
        }
        */
    }
}
