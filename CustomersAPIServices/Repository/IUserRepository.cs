//using CustomersAPIServices.EFModels;
using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Repository
{
    public interface IUserRepository
    {
        Object createUser(CreateUserRequest webOwner);
        IEnumerable<User> getAllUsers();
        Object getUser(int webOwnerId);

        Boolean deleteUser(int WebOwnerId);
        Boolean updateUser(UpdateUserRequest request,int userId);
        IEnumerable<WebsiteResponse> getWebsites(int webOwnerId);
        bool deleteWebsite(int webOwnerId, int webId);
        WebsiteResponse createWebsite(CreateWebsiteRequest request,int userId);
        object checkUserEmail(string email);
        object createOrganization(OrganizationRequest request, int v);
        object updateOrganization(UpdateOrganizationRequest request, int userId);
        object DeleteOrganization(int organizationID, int v);
        bool verifyWebsite(verifiedRequest request);
        object getStatisticData(int webID, int trackingInfoID, int from, int to, int userId);
        User lockUser(int userID);
        object getAllUserOrganizationAndWebsites(int userID);
        Website lockWebsite(int websiteID);
    }
}
