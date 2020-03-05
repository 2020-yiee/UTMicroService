using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class GetWebOwnerRequest
    {
        public int WebOwnerId { get; set; }
        public string Email { get; set; }
    }
}
