using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.RequestModels
{
    public class CreateWebsiteRequest
    {
        public int organizationID { get; set; }
        public string domainUrl { get; set; }

        public CreateWebsiteRequest(int organizationID, string domainUrl)
        {
            this.organizationID = organizationID;
            this.domainUrl = domainUrl;
        }
    }
}
