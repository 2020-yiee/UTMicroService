using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class LoginRequestModel
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
