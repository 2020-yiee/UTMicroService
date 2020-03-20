using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CustomersAPIServices.Repository
{
    public class UserRepositoryImpl : IUserRepository
    {

        private List<int> EVENT_TYPE_LIST = new List<int>();

        public UserRepositoryImpl()
        {
            EVENT_TYPE_LIST.Add(1);
            EVENT_TYPE_LIST.Add(0);
        }

        private DBUTContext context = new DBUTContext();
        public Object createUser(CreateUserRequest request)
        {
            var existed = context.User.Where(u => u.Email.Equals(request.email)).Count() > 0;
            if (existed)
            {
                return null;
            }
            User addUser = new User();
            addUser.Email = request.email;
            addUser.FullName = request.fullName;
            addUser.Password = Hashing.HashPassword(request.password);
            addUser.Actived = true;
            try
            {
                context.User.Add(addUser);
                context.SaveChanges();

                Organization organization = (Organization)createOrganization(new OrganizationRequest(request.organizationName), addUser.UserId);

                List<WebsiteResponse> websites = new List<WebsiteResponse>();
                    websites.Add(createWebsite(new CreateWebsiteRequest(organization.OrganizationId,request.domainUrl), addUser.UserId));

                List<OrganizationResponse> organizationResponses = new List<OrganizationResponse>();
                organizationResponses.Add(new OrganizationResponse(organization.OrganizationId, organization.Name,1,websites));
                return new
                {
                    
                    token = GenerateJwtToken(addUser.Email, addUser, "user"),
                    organizations = organizationResponses,
                    user = new UserResponse(addUser.UserId, addUser.FullName, addUser.Email)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public object GenerateJwtToken(string email, User user, string Role)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LgS9RPkJ0bf9PgdkzzHZM7hJdGCdW_gfGVHDNT1P3FvDdDpAWzan2JOzDtAuYHoP"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(7));

            var token = new JwtSecurityToken(
                "https://dev-tkt68df2.auth0.com/",
                "fHkCJ1ingY7v9zWpbrKwDqeGlr3zIIlm",
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public bool deleteUser(int webId)
        {
            User webOwner = context.User.Where(s => s.UserId == webId)
                .FirstOrDefault();
            try
            {
                webOwner.Actived = false;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public Object getUser(int userId)
        {
            User temp = context.User
                    .Where(s => s.UserId == userId)
                    .Where(s => s.Actived ==true)
                    .FirstOrDefault();
            if(temp != null)
            {
                List<int> organizationIds = context.Access.Where(s => s.UserId == temp.UserId).Select(s => s.OrganizationId).ToList();
                if(organizationIds==null || organizationIds.Count == 0)
                {
                    return new
                    {
                        organizations = new List<Object>(),
                        user = new UserResponse(temp.UserId, temp.FullName, temp.Email)
                       
                    };
                }
                List<Organization> organizations = context.Organization
                    .Where(s => organizationIds.Contains(s.OrganizationId) == true)
                    .Where(s => s.Removed == false)
                    .ToList();

                List<OrganizationResponse> organizationResponses = new List<OrganizationResponse>();
                foreach (var organization in organizations)
                {
                    var websites = context.Website
                            .Where(x => x.OrganizationId == organization.OrganizationId)
                            .Where(x => x.Removed == false)
                            .ToList()
                            .Select(x => new WebsiteResponse(x.WebId, x.DomainUrl, x.Removed, x.OrganizationId
                            ,x.Verified,x.CreatedAt,context.User.Where(s => s.UserId == x.AuthorId).FirstOrDefault().FullName))
                            .ToList();
                    var access = context.Access.Where(s => s.UserId == userId && s.OrganizationId == organization.OrganizationId).FirstOrDefault();

                    organizationResponses.Add(new OrganizationResponse(organization.OrganizationId, organization.Name,access.Role, websites));
                }

                return new
                {
                    organizations = organizationResponses,
                    user = new UserResponse(temp.UserId, temp.FullName, temp.Email)

                };
            }
            else
            {
                return null;
            }
        }

        public bool updateUser(UpdateUserRequest request,int userId)
        {
            var webOwner = context.User.Where(s => s.UserId == userId)
                .FirstOrDefault();
            if (webOwner != null)
                try
                {
                    webOwner.FullName = request.fullName;
                    webOwner.Email = request.email;
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            return false;
        }

        public IEnumerable<WebsiteResponse> getWebsites(int userId)
        {
            try
            {
                User user = context.User
                    .Where(s => s.UserId ==userId)
                    .Where(s => s.Actived == true).FirstOrDefault();
                if (user == null) return null;
                List<int> organizationIds = context.Access.Where(s => s.UserId == user.UserId).Select(s => s.OrganizationId).ToList();
                var websites = context.Website
                    .Where(s => s.Removed == false)
                    .Where(s => organizationIds.Contains(s.OrganizationId) == true)
                    .Select(x => new WebsiteResponse(x.WebId,x.DomainUrl, x.Removed,x.OrganizationId
                    ,x.Verified,x.CreatedAt,user.FullName)).ToList();
                return websites;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public bool deleteWebsite(int userId, int webId)
        {
            try
            {
                
                User user = context.User
                    .Where(s => s.UserId == userId)
                    .Where(s => s.Actived == true).FirstOrDefault();
                if (user == null) return false;
                List<int> organizationIds = context.Access.Where(s => s.UserId == user.UserId).Select(s => s.OrganizationId).ToList();

                Website website = context.Website
                    //.Where(s => s.WebOwnerId == webOwnerId)
                    //.Where(s => s.WebId == webId)
                    .FirstOrDefault(x => x.WebId == webId && organizationIds.Contains(x.OrganizationId) == true);
                if (website != null)
                {
                    website.Removed = true;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }

        public WebsiteResponse createWebsite(CreateWebsiteRequest request,int userId)
        {
            User user = context.User
                    .Where(s => s.UserId == userId)
                    .Where(s => s.Actived == true).FirstOrDefault();
            if (user == null) return null;
            Website website = new Website();
            website.DomainUrl = request.domainUrl;
            website.Removed = false;
            website.OrganizationId = request.organizationID;
            website.Verified = false;
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            website.CreatedAt = (long)timeSpan.TotalSeconds;
            website.AuthorId = userId;
            try
            {
                context.Website.Add(website);
                context.SaveChanges();
                //Website temp = context.Website.Where(s => s.WebOwnerId == website.WebOwnerId)
                //    .Where(s => s.WebUrl == website.WebUrl).FirstOrDefault();
                return new WebsiteResponse(website.WebId,website.DomainUrl, website.Removed
                    ,website.OrganizationId,website.Verified,website.CreatedAt,user.FullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public object checkUserEmail( string email)
        {
            User user = context.User.Where(s => s.Email == email).FirstOrDefault();
            if (user != null) return new
            {
                type = "ERROR",
                emailMessage = "Email has been existed"
            };
            return new { type = "SUCCESS" };

        }

        public object createOrganization(OrganizationRequest request, int userId)
        {
            try
            {
                Organization organization = new Organization();
                organization.Name = request.organirzationName;
                organization.Removed = false;
                context.Organization.Add(organization);
                context.SaveChanges();

                Access access = new Access();
                access.OrganizationId = organization.OrganizationId;
                access.UserId = userId;
                access.Role = 1;
                context.Access.Add(access);
                context.SaveChanges();

                return organization;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public object updateOrganization(UpdateOrganizationRequest request, int userId)
        {
            List<int> organizationIds = context.Access
                .Where(s => s.UserId == userId).Select(s => s.OrganizationId).ToList();

            Organization organization = context.Organization
                .FirstOrDefault(s => s.OrganizationId == request.organizationID
                && organizationIds.Contains(s.OrganizationId) == true
                && s.Removed == false);
            if (organization == null) return null;
            organization.Name = request.organizationName;
            context.SaveChanges();
            return organization;
        }

        public object DeleteOrganization(int organizationID, int userId)
        {
            List<int> organizationIds = context.Access
                .Where(s => s.UserId == userId).Select(s => s.OrganizationId).ToList();

            Organization organization = context.Organization
                .FirstOrDefault(s => s.OrganizationId == organizationID
                && organizationIds.Contains(s.OrganizationId) == true
                && s.Removed == false);
            if (organization == null) return null;
            organization.Removed = true; ;
            context.SaveChanges();
            return organization;
        }

        public bool verifyWebsite(verifiedRequest request)
        {
            Website website = context.Website.FirstOrDefault(s => s.WebId == request.webID && s.DomainUrl == request.domainUrl);
            if (website != null)
            {
                website.Verified = true;
                context.SaveChanges();
                return true;
            }
            else return false;
        }
        private Boolean checkAuthencation(int websiteId, int userId)
        {
            List<int> orgIds = context.Access.Where(s => s.UserId == userId).Select(s => s.OrganizationId).ToList();
            if (orgIds == null || orgIds.Count == 0) return false;
            Website website = context.Website.FirstOrDefault(s => s.WebId == websiteId);
            if (website == null) return false;
            return orgIds.Contains(website.OrganizationId);
        }
        //=============================================admin==================================================
        public IEnumerable<User> getAllUsers()
        {
            try
            {
                return context.User.ToList();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public User lockUser(LockRequest request)
        {
            try
            {
                User user = context.User.FirstOrDefault(s => s.UserId == request.ID);
                if (user != null)
                {
                    user.Actived = request.locked;
                    context.SaveChanges();
                }
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Object getAllUserOrganizationAndWebsites(int userID)
        {
            try
            {
                return getUser(userID);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Website lockWebsite(LockRequest request)
        {
            try
            {
                Website website = context.Website.FirstOrDefault(s => s.WebId == request.ID);
                if (website != null)
                {
                    website.Removed = request.locked;
                    context.SaveChanges();
                }
                return website;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object getAllWebSite()
        {
            List<Website> websites =  context.Website.ToList();
            List<WebsiteResponse> responses = new List<WebsiteResponse>();
            foreach (var website in websites)
            {
                User user = context.User.Where(s => s.UserId == website.AuthorId).FirstOrDefault();
                if (user == null) return null;
                WebsiteResponse response = new WebsiteResponse(website.WebId, website.DomainUrl, website.Removed
                    , website.OrganizationId, website.Verified, website.CreatedAt, user.FullName);
                responses.Add(response);
            }
            return responses;
        }

        public IActionResult getAllMemberOfOrganization(int organizationID,int userID)
        {
            List<MemberResponse> memberResponses = new List<MemberResponse>();
            
            List<int> userIDs = context.Access.Where(s => s.OrganizationId == organizationID).Select(s => s.UserId).ToList();
            foreach (var ID in userIDs)
            {
                User user = context.User.Where(s => s.UserId == ID).FirstOrDefault();
                if (user == null) return new BadRequestResult();
                Access userAccess = context.Access.Where(s => s.UserId == ID && s.OrganizationId == organizationID).FirstOrDefault();
                if (userAccess == null) return new BadRequestResult();
                MemberResponse memberResponse = new MemberResponse(user.UserId,user.Email, user.FullName, userAccess.Role,userAccess.DayJoin);
                memberResponses.Add(memberResponse);
            }
            return new OkObjectResult(memberResponses);
        }

        public IActionResult removeOrganzationMember(RemoveMemberRequest request, int ownerID)
        {
            Access access = context.Access.Where(s => s.OrganizationId == request.organizationID && s.UserId == ownerID).FirstOrDefault();
            if (access == null) return new BadRequestResult();
            if (access.Role != 1) return new UnauthorizedResult();
            access = context.Access.Where(s => s.UserId == request.userID && s.OrganizationId == request.organizationID).FirstOrDefault();
            if (access == null) return new BadRequestResult();
            context.Access.Remove(access);
            context.SaveChanges();
            return new OkResult();
        }
    }
}
