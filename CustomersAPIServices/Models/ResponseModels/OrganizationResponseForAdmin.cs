using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class OrganizationResponseForAdmin
    {
        public int organizationID { get; set; }
        public string organizationName { get; set; }
        public int userRole { get; set; }
        public bool removed { get; set; }
        public List<WebsiteResponseForAdmin> websites { get; set; }

        public OrganizationResponseForAdmin(int organizationID, string organizationName, int userRole, bool removed, List<WebsiteResponseForAdmin> websites)
        {
            this.organizationID = organizationID;
            this.organizationName = organizationName;
            this.userRole = userRole;
            this.removed = removed;
            this.websites = websites;
        }
    }
}
