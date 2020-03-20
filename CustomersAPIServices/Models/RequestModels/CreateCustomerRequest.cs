using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class CreateUserRequest
    {
        public string email { get; set; }
        public string fullName { get; set; }
        public string password { get; set; }
        public string domainUrl { get; set; }
        public string organizationName { get; set; }
    }
}
