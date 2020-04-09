using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.RequestModels
{
    public class RemoveMemberRequest
    {
        public int organizationID { get; set; }
        public int userID { get; set; }

    }
}
