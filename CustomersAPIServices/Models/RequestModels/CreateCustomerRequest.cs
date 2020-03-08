using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class CreateWebOwnerRequest
    {
        public string username { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public string password { get; set; }
        public string web_url { get; set; }
    }
}
