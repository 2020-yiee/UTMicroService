using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class CustomerResponse
    {
        public string name { get; set; }

        public CustomerResponse(string name)
        {
            this.name = name;
        }
    }
}
