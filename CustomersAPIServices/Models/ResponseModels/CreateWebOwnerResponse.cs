using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class CreateWebOwnerResponse
    {
        public string  type { get; set; }
        public string message { get; set; }
        public WebOwnerResponse webOwner { get; set; }
        public WebsiteResponse website { get; set; }

        public CreateWebOwnerResponse(string type, string message, WebOwnerResponse webOwner, WebsiteResponse website)
        {
            this.type = type;
            this.message = message;
            this.webOwner = webOwner;
            this.website = website;
        }
    }
}
