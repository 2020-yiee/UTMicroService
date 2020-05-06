using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.RequestModels
{
    public class inviteUserRequest
    {
        public string email { get; set; }
        public int organizationID { get; set; }
        public int roleID { get; set; }
    }
}
