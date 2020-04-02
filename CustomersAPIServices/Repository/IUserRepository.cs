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
        IActionResult getUser(int webOwnerId);

        Boolean deleteUser(int WebOwnerId);
        Boolean updateUser(UpdateUserRequest request,int userId);
        IEnumerable<WebsiteResponse> getWebsites(int webOwnerId);
        bool deleteWebsite(int webOwnerId, int webId);
        WebsiteResponse createWebsite(CreateWebsiteRequest request,int userId);
        IActionResult checkUserEmail(string email);
        object createOrganization(OrganizationRequest request, int v);
        object updateOrganization(UpdateOrganizationRequest request, int userId);
        object DeleteOrganization(int organizationID, int v);
        bool verifyWebsite(verifiedRequest request);
        IActionResult lockUser(LockRequest request);
        IActionResult getAllUserOrganizationAndWebsites(int userID);
        IActionResult getAllWebSite();
        IActionResult lockWebsite(LockRequest request);
        IActionResult getAllMemberOfOrganization(int organizationIDs,int v);
        IActionResult inviteUser(int userID,string email,int organizationID,int roleID);
        IActionResult uninviteUser(int v, string email, int organizationID);
        IActionResult changeRole(int v, string email, int organizationID);
        IActionResult inviteNewUser(InviteNewUserRequest request, int v1, string value, int v2, int v3);
    }
}
