using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.RequestModels
{
    public class changeNameWebsiteRequest
    {
        public int webID { get; set; }
        public string newName { get; set; }
    }
}
