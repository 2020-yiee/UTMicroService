//using CustomersAPIServices.EFModels;
using UserAPIServices.EFModels;
using UserAPIServices.Models;
using UserAPIServices.Models.RequestModels;
using UserAPIServices.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Repository
{
    public interface IUserRepository
    {
        IActionResult createUser(CreateUserRequest webOwner);
        IActionResult getNewInviteMember(int webOwnerId);

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
        IActionResult getAllMemberOfOrganization(int organizationIDs,int v);
        IActionResult inviteUser(int userID,string email,int organizationID,int roleID);
        IActionResult uninviteUser(int v, string email, int organizationID);
        IActionResult changeRole(int v, string email, int organizationID);
        IActionResult inviteNewUser(InviteNewUserRequest request, int v1, string value, int v2, int v3);
    }
}
