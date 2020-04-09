using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.RequestModels
{
    public class GetWebOwnerRequest
    {
        public int userID { get; set; }
        public string email { get; set; }
    }
}
