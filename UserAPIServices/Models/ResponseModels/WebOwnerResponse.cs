using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.ResponseModels
{
    public class UserResponse
    {
        public int userID { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }

        public UserResponse(int webOwnerId, string fullName, string email)
        {
            this.userID = webOwnerId;
            this.fullName = fullName;
            this.email = email;
        }
    }
}
