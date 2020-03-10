using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class CreateWebsiteRequest
    {
        public int organizationID { get; set; }
        public string domainUrl { get; set; }

        public CreateWebsiteRequest(int organizationID, string domainUrl)
        {
            this.domainUrl = domainUrl;
            this.organizationID = organizationID;
        }
    }
}
