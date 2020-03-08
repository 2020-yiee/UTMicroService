using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class GetWebOwnerRequest
    {
        public int webOwnerId { get; set; }
        public string email { get; set; }
    }
}
