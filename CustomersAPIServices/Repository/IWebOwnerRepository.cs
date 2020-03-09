using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Repository
{
    public interface IWebOwnerRepository
    {
        Object createWebOwner(CreateWebOwnerRequest webOwner);
        IEnumerable<WebOwnerResponse> getAllWebOwners();
        Object getWebOwner(int webOwnerId);

        Boolean deleteWebOwner(int WebOwnerId);
        Boolean updateWebOwner(UpdateWebOwnerRequest request);
        IEnumerable<WebsiteResponse> getWebsites(int webOwnerId);
        bool deleteWebsite(int webOwnerId, int webId);
        WebsiteResponse createWebsite(CreateWebsiteRequest request);
        object checkUsernnameOrEmail(string username, string email);
    }
}
