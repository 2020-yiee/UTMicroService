using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class CreateCustomerRequest
    {
        public string userName { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string password { get; set; }
    }
}
