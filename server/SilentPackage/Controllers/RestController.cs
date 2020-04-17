using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentPackage.Models;


namespace SilentPackage.Controllers
{

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
                    return StatusCode(405);
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
        private readonly string _secretKey = "kdoumpo3";
        readonly DatabaseManagement _databaseManagement = DatabaseManagement.GetInstance("main.db",Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        [Route("api/1.0/reports/{license}/{deviceid}")]
        [HttpPut]
        public IActionResult SaveReports(List<IFormFile> files, string license, string deviceid)
        {
            try
            {
               SaveFile(files);

            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }

            return Ok();
        }



        public void SaveFile(List<IFormFile> files)
        {
            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SP\", "reports");

            Directory.CreateDirectory(target);

            files.ForEach(async file =>
            {
                if (file.Length <= 0) return;
                var filePath = Path.Combine(target, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            });
        }
    }
}
