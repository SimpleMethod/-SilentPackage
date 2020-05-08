/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentPackage.Models;


namespace SilentPackage.Controllers
{


    public class UsersNewController : Controller
    {
        private const string ApiKey = "abc2137";
        readonly DatabaseManagement _databaseManagement = DatabaseManagement.GetInstance("main.db", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        /// <summary>
        /// Getting a list of users.
        /// </summary>
        /// <returns>list of users.</returns>
        [Route("api/1.1/users/")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }
            List<UsersModel> usersModel = _databaseManagement.GetUsers();
            var jsonString = JsonSerializer.Serialize(usersModel);
            return Ok(jsonString);
        }
        /// <summary>
        /// Getting user information based on license ID.
        /// </summary>
        /// <param name="license">license ID.</param>
        /// <returns>JSON with data about user.</returns>
        [Route("api/1.1/users/{license}")]
        [HttpGet]
        public IActionResult GetUserData(string license)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }
            UsersModel usersModel = _databaseManagement.GetUser(license);
            var jsonString = JsonSerializer.Serialize(usersModel);
            if (usersModel.Id == 0)
            {
                return StatusCode(404);
            }
            return Ok(jsonString);
        }
        /// <summary>
        /// Change of machine identifier.
        /// </summary>
        /// <param name="license">license ID.</param>
        /// <param name="deviceid">Device ID.</param>
        /// <returns></returns>
        [Route("api/1.1/users/{license}/{deviceid}")]
        [HttpPost]
        public IActionResult UpdateDeviceId(string license, string deviceid)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }

            UsersModel model = _databaseManagement.GetUser(license);
            if (model.Id != 0)
            {
                _databaseManagement.UpdateDeviceIdUser(license, deviceid);
                return Ok();
            }
            return StatusCode(404);
        }
        /// <summary>
        /// Creating a new user.
        /// </summary>
        /// <param name="license">license ID.</param>
        /// <returns> HTTP status</returns>
        [Route("api/1.1/users/{license}")]
        [HttpPut]
        public IActionResult CreateUser(string license)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }
            UsersModel model = _databaseManagement.GetUser(license);
            if (model.Id != 0)
            {
                return StatusCode(409);
            }
            _databaseManagement.CreateUser(license);
            return Ok();
        }
        /// <summary>
        ///  Deleting user from database.
        /// </summary>
        /// <param name="license"> License ID.</param>
        /// <returns>HTTP status.</returns>
        [Route("api/1.1/users/{license}")]
        [HttpDelete]
        public IActionResult DeleteUser(string license)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }

            UsersModel model = _databaseManagement.GetUser(license);
            if (model.Id == 0)
            {
                return StatusCode(404);
            }
            _databaseManagement.DeleteUser(license);
            return Ok();

        }

        /// <summary>
        /// Getting all user reports.
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        [Route("api/1.1/reports/{deviceid}")]
        [HttpGet]
        public async Task<IActionResult> GetReportFiles(string deviceid)
        {
            string value = Request.Headers["API"];
            string timestamp = Request.Headers["timestamp"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }
            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\", @"reports\" + deviceid);
            var list = new List<string>();
            string[] extension = { ".7z" };
            List<Files> files = FileDirectory.ScanDirectory(target, extension);

            foreach (var e in files)
            {
                if (timestamp != null && (timestamp.Equals("true")))
                {
                    string[] time = Path.GetFileName(e.FullName).Split(".");
                    list.Add(time[0]);
                }
                else
                {
                    list.Add(Path.GetFileName(e.FullName));
                }
            }
            if (list.Count.Equals(0))
            {
                return NotFound();
            }

            return Ok(list);

        }
        /// <summary>
        /// Remove reports.
        /// </summary>
        /// <param name="deviceid">Device ID.</param>
        /// <param name="filename">Reports filename.</param>
        /// <returns>HTTP status.</returns>
        [Route("api/1.1/reports/{deviceid}/{filename}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveReport(string deviceid, string filename)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }

            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\", @"reports\" + deviceid);
            if (Directory.Exists(target) && System.IO.File.Exists(target + @"\" + filename + ".7z"))
            {

                System.IO.File.Delete(target + @"\" + filename + ".7z");
                if (!System.IO.File.Exists(target + @"\" + filename + ".7z"))
                {
                    return Ok();
                }
                return StatusCode(416);
            }
            return NotFound();
        }

        /// <summary>
        ///  Remove all reports.
        /// </summary>
        /// <param name="deviceid">Device ID.</param>
        /// <returns>HTTP status.</returns>
        [Route("api/1.1/reports/{deviceid}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveReports(string deviceid)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }

            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\", @"reports\" + deviceid);
            var list = new List<string>();
            if (Directory.Exists(target))
            {
                string[] extension = { ".7z" };
                List<Files> files = FileDirectory.ScanDirectory(target, extension);

                foreach (var e in files)
                {
                    System.IO.File.Delete(e.FullName);
                }

                return Ok(list);
            }
            return NotFound();
        }

        /// <summary>
        /// Getting report.
        /// </summary>
        /// <param name="deviceid">Device ID.</param>
        /// <param name="filename">Filename.</param>
        /// <returns>File.</returns>
        [Route("api/1.1/reports/{deviceid}/{filename}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DownloadReports(string deviceid, string filename)
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\reports\" + deviceid + @"\" + filename;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            Stream stream = System.IO.File.OpenRead(filePath);
            if (stream == null) return NotFound();
            return File(stream, "application/octet-stream"); // returns a FileStreamResult
        }

        /// <summary>
        /// Getting a list of reports between dates
        /// </summary>
        /// <param name="deviceid">Device ID.</param>
        /// <param name="starrange">Starting date range.</param>
        /// <param name="endrange">Final range.</param>
        /// <returns></returns>
        [Route("api/1.1/reports/{deviceid}/{starrange}/{endrange}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetReportsBetweenDate(string deviceid, string starrange, string endrange)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }
            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\", @"reports\" + deviceid);
            var list = new List<string>();
            string[] extension = { ".7z" };
            List<Files> files = FileDirectory.ScanDirectory(target, extension);

            foreach (var e in files)
            {
                string[] time = Path.GetFileName(e.FullName).Split(".");

                if (Int32.Parse(time[0]) >= Int32.Parse(starrange) && Int32.Parse(time[0]) <= Int32.Parse(endrange))
                {
                    list.Add(time[0]);
                }

            }
            if (list.Count.Equals(0))
            {
                return NotFound();
            }

            return Ok(list);
        }



    }




    [Route("api/1.0/users")]
    public class UsersController : Controller
    {
        private readonly string _secretKey = "kdoumpo3";
        readonly DatabaseManagement _databaseManagement = DatabaseManagement.GetInstance("main.db", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));


        [HttpGet("{license}")]
        public IActionResult GetUserData(string license)
        {
            UsersModel usersModel = _databaseManagement.GetUser(license);
            var jsonString = JsonSerializer.Serialize(usersModel);
            if (usersModel.Id == 0)
            {
                return StatusCode(404);
            }
            return Ok(jsonString);
        }

        [HttpPost]
        public IActionResult UpdateDeviceId([FromBody]UpdateModel updateModel)
        {
            if (updateModel.SecureKey == _secretKey)
            {
                UsersModel model = _databaseManagement.GetUser(updateModel.License);
                if (model.Id != 0)
                {
                    _databaseManagement.UpdateDeviceIdUser(updateModel.License, updateModel.DeviceId);
                    return Ok();
                }

                return StatusCode(404);
            }
            return StatusCode(403);
        }

        [HttpPut]
        public IActionResult CreateUser([FromBody]CreateUser createUser)
        {
            if (createUser.SecureKey == _secretKey)
            {
                UsersModel model = _databaseManagement.GetUser(createUser.License);
                if (model.Id != 0)
                {
                    return StatusCode(409);
                }
                _databaseManagement.CreateUser(createUser.License);
                return Ok();
            }
            return StatusCode(403);
        }

        [HttpDelete]
        public IActionResult DeleteUser([FromBody]DeleteUser deleteUser)
        {
            if (deleteUser.SecureKey == _secretKey)
            {
                UsersModel model = _databaseManagement.GetUser(deleteUser.License);
                if (model.Id == 0)
                {
                    return StatusCode(404);
                }
                _databaseManagement.DeleteUser(deleteUser.License);
                return Ok();
            }
            return StatusCode(403);
        }


    }


    public class ReportsController : Controller
    {
        private const string ApiKey = "abc2137";
        private readonly string _secretKey = "kdoumpo3";
        readonly DatabaseManagement _databaseManagement = DatabaseManagement.GetInstance("main.db", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        [Route("api/1.0/reports/{license}/{deviceid}")]
        [HttpPut]
        public async Task<IActionResult> SaveReports(List<IFormFile> files, string license, string deviceid)
        {
            Console.WriteLine("Report from:" + deviceid);

            UsersModel usersModel = _databaseManagement.GetUser(license);
            if (usersModel.Id != 0)
            {
                long size = files.Sum(f => f.Length);

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\reports\" + deviceid + @"\";

                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }

                        Console.WriteLine(filePath);
                        using (var stream = System.IO.File.Create(filePath + formFile.FileName))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }

                return Ok(new { count = files.Count, size });
            }

            return NotFound();

        }

        [Route("api/1.0/reports/{license}/{deviceid}/{filename}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DownloadReports(string license, string deviceid, string filename)
        {
            UsersModel usersModel = _databaseManagement.GetUser(license);
            if (usersModel.Id != 0 && usersModel.DeviceId == deviceid)
            {
                var filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\reports\" + deviceid + @"\" + filename;
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }
                Stream stream = System.IO.File.OpenRead(filePath);
                if (stream == null) return NotFound();
                return File(stream, "application/octet-stream"); // returns a FileStreamResult
            }
            return NotFound();
        }

        [Route("api/1.0/reports/{license}/{deviceid}/{filename}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveReports(string license, string deviceid, string filename)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }


            UsersModel usersModel = _databaseManagement.GetUser(license);
            if (usersModel.Id != 0 && usersModel.DeviceId == deviceid)
            {
                var filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\reports\" + deviceid + @"\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return Ok();
            }
            return NotFound();
        }

        [Route("api/1.0/reports/{license}/{deviceid}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveReport(string license, string deviceid)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }

            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\", @"reports\" + deviceid);
            var list = new List<string>();
            UsersModel usersModel = _databaseManagement.GetUser(license);
            if (usersModel.Id != 0 && Directory.Exists(target))
            {
                string[] extension = { ".7z" };
                List<Files> files = FileDirectory.ScanDirectory(target, extension);

                foreach (var e in files)
                {
                    System.IO.File.Delete(e.FullName);
                }

                return Ok(list);
            }
            return NotFound();
        }


        [Route("api/1.0/reports/{license}/{deviceid}")]
        [HttpGet]
        public async Task<IActionResult> GetReportFiles(string license, string deviceid)
        {
            string value = Request.Headers["API"];
            if (value == null || (!value.Equals(ApiKey)))
            {
                return StatusCode(403);
            }

            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\", @"reports\" + deviceid);
            var list = new List<string>();
            UsersModel usersModel = _databaseManagement.GetUser(license);
            if (usersModel.Id != 0 && Directory.Exists(target))
            {
                string[] extension = { ".7z" };
                List<Files> files = FileDirectory.ScanDirectory(target, extension);

                foreach (var e in files)
                {
                    list.Add(Path.GetFileName(e.FullName));
                }

                return Ok(list);
            }
            return NotFound();
        }



        [Obsolete]
        public void SaveFile(List<IFormFile> files, string deviceid)
        {
            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP_server\", @"reports\" + deviceid);
            Directory.CreateDirectory(target);

            files.ForEach(async file =>
            {
                if (file.Length <= 0) return;

                try
                {
                    var filePath = Path.Combine(target, file.FileName);
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Flush();
                    stream.Close();
                    stream.DisposeAsync();
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
                catch (ObjectDisposedException e)
                {
                    Console.Write("Wyjatek:");
                    Console.WriteLine(e);
                }
            });
        }
    }
}
