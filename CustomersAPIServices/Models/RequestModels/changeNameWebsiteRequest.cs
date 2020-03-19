using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class changeNameWebsiteRequest
    {
        public int webID { get; set; }
        public string newName { get; set; }
    }
}
