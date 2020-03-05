using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class CreateWebsiteRequest
    {
        public int webOwnerId { get; set; }
        public string webUrl { get; set; }

        public CreateWebsiteRequest(int webOwnerId, string webUrl)
        {
            this.webOwnerId = webOwnerId;
            this.webUrl = webUrl;
        }
    }
}
