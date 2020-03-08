using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class DeleteWebsiteRequest
    {
        public int web_owner_id { get; set; }
        public int web_id { get; set; }
    }
}
