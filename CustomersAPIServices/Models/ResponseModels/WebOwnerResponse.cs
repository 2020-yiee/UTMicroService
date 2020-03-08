using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class WebOwnerResponse
    {
        public int web_owner_id { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public string role { get; set; }

        public WebOwnerResponse(int customerId,string username, string fullname, string email, string role)
        {
            this.web_owner_id = customerId;
            this.username = username;
            this.full_name = fullname;
            this.email = email;
            this.role = role;
        }
    }
}
