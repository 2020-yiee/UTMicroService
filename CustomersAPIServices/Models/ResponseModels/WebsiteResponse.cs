using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class WebsiteResponse
    {
        public int website_id { get; set; }
        public int web_owner_id { get; set; }
        public string web_url { get; set; }
        public bool is_removed { get; set; }

        public WebsiteResponse(int websiteId, int webOwnerId, string webUrl, bool isRemoved)
        {
            website_id = websiteId;
            web_owner_id = webOwnerId;
            web_url = webUrl;
            this.is_removed = isRemoved;
        }
    }
}
