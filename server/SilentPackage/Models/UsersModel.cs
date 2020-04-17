using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilentPackage.Models
{
    public class UsersModel
    {
        public UsersModel()
        {

        }
        public UsersModel(int id, string license, string deviceId)
        {
            Id = id;
            License = license;
            DeviceId = deviceId;
        }

        public int Id { get; set; }
        public string License { get; set; }
        public string DeviceId { get; set; }
    }

    public class UpdateModel
    {
        public UpdateModel()
        {

        }
        public UpdateModel(string secureKey, string license, string deviceId)
        {
            SecureKey = secureKey;
            License = license;
            DeviceId = deviceId;
        }
        public string SecureKey { get; set; }
        public string License { get; set; }
        public string DeviceId { get; set; }
    }

    public class CreateUser
    {
        public CreateUser(string secureKey, int id, string license, string deviceId)
        {
            SecureKey = secureKey;
            Id = id;
            License = license;
            DeviceId = deviceId;
        }

        public CreateUser()
        {

        }

        public string SecureKey { get; set; }
        public int Id { get; set; }
        public string License { get; set; } 
        public string DeviceId { get; set; }
    }

    public class DeleteUser
    {
        public DeleteUser(string secureKey, string license)
        {
            SecureKey = secureKey;
            License = license;
        }

        public DeleteUser()
        {

        }
        public string SecureKey { get; set; }
        public string License { get; set; }
    }
}
