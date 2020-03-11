using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class WebsiteResponse
    {
        public int webID { get; set; }
        public string webUrl { get; set; }
        public bool removed { get; set; }
        public int organizationID { get; set; }
        public bool verified { get; set; }

        public WebsiteResponse( int webID, string webUrl, bool isRemoved,int organizationID,bool verrified)
        {
            this.webID = webID;
            this.webUrl = webUrl;
            this.removed = isRemoved;
            this.organizationID = organizationID;
            this.verified = verified;
        }
    }
}
