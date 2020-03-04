using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class CustomerResponse
    {
        public int customerId { get; set; }
        public string username { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string role { get; set; }

        public CustomerResponse(int customerId,string username, string fullname, string email, string role)
        {
            this.customerId = customerId;
            this.username = username;
            this.fullname = fullname;
            this.email = email;
            this.role = role;
        }
    }
}
