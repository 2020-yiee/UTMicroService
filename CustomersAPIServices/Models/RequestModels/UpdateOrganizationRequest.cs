using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class UpdateOrganizationRequest
    {
        public int organizationID { get; set; }
        public string organizationName { get; set; }
    }
}
