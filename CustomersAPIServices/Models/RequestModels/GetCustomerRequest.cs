using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class GetWebOwnerRequest
    {
        public int web_owner_id { get; set; }
        public string email { get; set; }
    }
}
