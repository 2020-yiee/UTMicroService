using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.ResponseModels
{
    public class WebsiteResponse
    {
        public int webID { get; set; }
        public int userID { get; set; }
        public string webUrl { get; set; }
        public bool removed { get; set; }
        public int organizationID { get; set; }

        public WebsiteResponse(int websiteId, int userId, string webUrl, bool isRemoved, int organizationID)
        {
            this.webID = websiteId;
            this.userID = userId;
            this.webUrl = webUrl;
            this.removed = isRemoved;
            this.organizationID = organizationID;
        }
    }
}
