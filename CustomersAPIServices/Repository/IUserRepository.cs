//using CustomersAPIServices.EFModels;
using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Repository
{
    public interface IUserRepository
    {
        IActionResult createUser(CreateUserRequest webOwner);
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
        User lockUser(LockRequest request);
        IActionResult getAllUserOrganizationAndWebsites(int userID);
        IActionResult getAllWebSite();
        Website lockWebsite(LockRequest request);
        IActionResult getAllMemberOfOrganization(int organizationIDs,int v);
        IActionResult removeOrganzationMember(RemoveMemberRequest request, int v);
    }
}
