using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.ResponseModels
{
    public class WebsiteResponse
    {
        public int webID { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public bool removed { get; set; }
        public int organizationID { get; set; }
        public bool verified { get; set; }
        public long createdAt { get; set; }
        public string authorName { get; set; }

        public WebsiteResponse(int webID, string name, string webUrl, bool removed, int organizationID, bool verified, long createdAt, string authorName)
        {
            this.webID = webID;
            this.name = name;
            this.webUrl = webUrl;
            this.removed = removed;
            this.organizationID = organizationID;
            this.verified = verified;
            this.createdAt = createdAt;
            this.authorName = authorName;
        }
    }
}
