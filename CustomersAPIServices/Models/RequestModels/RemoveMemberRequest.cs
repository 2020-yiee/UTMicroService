using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class RemoveMemberRequest
    {
        public int organizationID { get; set; }
        public int userID { get; set; }

    }
}
