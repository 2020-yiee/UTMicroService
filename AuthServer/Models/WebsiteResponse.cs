using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.ResponseModels
{
    public class WebsiteResponse
    {
        public int websiteId { get; set; }
        public int webOwnerId { get; set; }
        public string webUrl { get; set; }
        public bool isRemoved { get; set; }

        public WebsiteResponse(int websiteId, int webOwnerId, string webUrl, bool isRemoved)
        {
            this.websiteId = websiteId;
            this.webOwnerId = webOwnerId;
            this.webUrl = webUrl;
            this.isRemoved = isRemoved;
        }
    }
}
