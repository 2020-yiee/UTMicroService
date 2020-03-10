using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class GetCustomerRequest
    {
        public int customerId { get; set; }
        public string email { get; set; }
    }
}
