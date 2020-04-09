using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.RequestModels
{
    public class UpdateUserRequest
    {
        public string email { get; set; }
        public string fullName { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
