using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class CreateWebsiteRequest
    {
        public int web_owner_id { get; set; }
        public string web_url { get; set; }

        public CreateWebsiteRequest(int webOwnerId, string webUrl)
        {
            this.web_owner_id = webOwnerId;
            this.web_url = webUrl;
        }
    }
}
