using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class WebsiteResponse
    {
        public int WebsiteId { get; set; }
        public int WebOwnerId { get; set; }
        public string WebUrl { get; set; }
        public bool isRemoved { get; set; }

        public WebsiteResponse(int websiteId, int webOwnerId, string webUrl, bool isRemoved)
        {
            WebsiteId = websiteId;
            WebOwnerId = webOwnerId;
            WebUrl = webUrl;
            this.isRemoved = isRemoved;
        }
    }
}
