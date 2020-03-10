using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class OrganizationResponse
    {
        public int organizationID { get; set; }
        public string organizationName { get; set; }
        public List<WebsiteResponse> websites { get; set; }

        public OrganizationResponse(int organizationID, string organizationName, List<WebsiteResponse> websites)
        {
            this.organizationID = organizationID;
            this.organizationName = organizationName;
            this.websites = websites;
        }
    }
}
