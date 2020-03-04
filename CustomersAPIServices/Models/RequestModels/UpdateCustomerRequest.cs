using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class UpdateCustomerRequest
    {
        public int customerId { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string role { get; set; }
    }
}
