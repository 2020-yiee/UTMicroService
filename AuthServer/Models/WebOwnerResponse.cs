using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.ResponseModels
{
    public class WebOwnerResponse
    {
        public int webOwnerId { get; set; }
        public string username { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string role { get; set; }

        public WebOwnerResponse(int customerId,string username, string fullname, string email, string role)
        {
            this.webOwnerId = customerId;
            this.username = username;
            this.fullName = fullname;
            this.email = email;
            this.role = role;
        }
    }
}
