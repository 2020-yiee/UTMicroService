using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class WebOwnerResponse
    {
        public int WebOwnerId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public WebOwnerResponse(int customerId,string username, string fullname, string email, string role)
        {
            this.WebOwnerId = customerId;
            this.Username = username;
            this.FullName = fullname;
            this.Email = email;
            this.Role = role;
        }
    }
}
