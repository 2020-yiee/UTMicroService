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
        public IActionResult createUser(CreateUserRequest request)
        {
            var existed = context.User.Where(u => u.Email.Equals(request.email)).Count() > 0;
            if (existed)
            {
                return new BadRequestResult();
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
                organizationResponses.Add(new OrganizationResponse(organization.OrganizationId, organization.Name,
                    context.Access.Where(s =>s.OrganizationId == organization.OrganizationId &&
                    s.UserId == addUser.UserId).FirstOrDefault().Role,organization.Removed,websites));
                return new OkObjectResult(
                    new
                    {
                        token = GenerateJwtToken(addUser.Email, addUser, "user"),
                        organizations = organizationResponses,
                        user = new UserResponse(addUser.UserId, addUser.FullName, addUser.Email)
                    }
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new BadRequestResult();
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

        public IActionResult getUser(int userId)
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
                    return new OkObjectResult(
                        new
                        {
                            organizations = new List<Object>(),
                            user = new UserResponse(temp.UserId, temp.FullName, temp.Email)

                        }
                        );
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
                            ,x.Verified,x.CreatedAt,context.User.Where(s => s.UserId == x.AuthorId).FirstOrDefault().FullName,
                            context.User.Where(s => s.UserId == x.AuthorId).FirstOrDefault().UserId))
                            .ToList();
                    var access = context.Access.Where(s => s.UserId == userId && s.OrganizationId == organization.OrganizationId).FirstOrDefault();

                    organizationResponses.Add(new OrganizationResponse(organization.OrganizationId, organization.Name,access.Role,organization.Removed,websites));
                }

                return new OkObjectResult(new
                {
                    organizations = organizationResponses,
                    user = new UserResponse(temp.UserId, temp.FullName, temp.Email)

                });
            }
            else
            {
                return null;
            }
        }

        public bool updateUser(UpdateUserRequest request,int userId)
        {
            var user = context.User.Where(s => s.UserId == userId)
                .FirstOrDefault();
            if (user != null)
                try
                {
                    var checkPassword = BCrypt.Net.BCrypt.Verify(request.oldPassword, user.Password);
                    if (checkPassword)
                    {
                        user.FullName = request.fullName;
                        user.Email = request.email;
                        user.Password = Hashing.HashPassword(request.newPassword);
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
                    ,x.Verified,x.CreatedAt,user.FullName,user.UserId)).ToList();
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
                List<int> organizationIds = context.Access.Where(s => s.UserId == user.UserId)
                    .Where(s => s.Role == 1 || s.Role == 2).Select(s => s.OrganizationId).ToList();

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
            Organization organization = context.Organization.Where(s => s.OrganizationId == request.organizationID).FirstOrDefault();
            if (organization == null) return null;
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
                    ,website.OrganizationId,website.Verified,website.CreatedAt,user.FullName,user.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public IActionResult checkUserEmail( string email)
        {
            User user = context.User.Where(s => s.Email == email).FirstOrDefault();
            if (user != null) return new OkObjectResult(new
            {
                type = "ERROR",
                emailMessage = "Email has been existed"
            });
            return new OkObjectResult(new { type = "SUCCESS" });

        }

        public object createOrganization(OrganizationRequest request, int userId)
        {
            try
            {
                Organization organization = new Organization();
                organization.Name = request.organirzationName;
                organization.Removed = false;
                var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                organization.CreatedAt = (long)timeSpan.TotalSeconds;
                context.Organization.Add(organization);
                context.SaveChanges();

                Access access = new Access();
                access.OrganizationId = organization.OrganizationId;
                access.UserId = userId;
                access.Role = 1;
                access.DayJoin = (long)timeSpan.TotalSeconds;
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
                .Where(s => s.UserId == userId && s.Role == 1).Select(s => s.OrganizationId).ToList();

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
                .Where(s => s.UserId == userId && s.Role == 1).Select(s => s.OrganizationId).ToList();

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

        public IActionResult lockUser(LockRequest request)
        {
            try
            {
                User user = context.User.FirstOrDefault(s => s.UserId == request.ID);
                if (user != null)
                {
                    user.Actived = request.locked;
                    context.SaveChanges();
                }
                return new OkObjectResult(user);
            }
            catch (Exception)
            {
                return new UnprocessableEntityResult();
            }
        }

        public IActionResult getAllUserOrganizationAndWebsites(int userID)
        {
            User temp = context.User
                    .Where(s => s.UserId == userID)
                    .FirstOrDefault();
            if (temp != null)
            {
                List<int> organizationIds = context.Access.Where(s => s.UserId == temp.UserId).Select(s => s.OrganizationId).ToList();
                if (organizationIds == null || organizationIds.Count == 0)
                {
                    return new OkObjectResult(
                        new
                        {
                            organizations = new List<Object>(),
                            user = new UserResponse(temp.UserId, temp.FullName, temp.Email)

                        });
                }
                List<Organization> organizations = context.Organization
                    .Where(s => organizationIds.Contains(s.OrganizationId) == true)
                    .ToList();

                List<OrganizationResponseForAdmin> organizationResponses = new List<OrganizationResponseForAdmin>();
                foreach (var organization in organizations)
                {
                    var websites = context.Website
                            .Where(x => x.OrganizationId == organization.OrganizationId)
                            .ToList()
                            .Select(x => new WebsiteResponseForAdmin(x.WebId, x.DomainUrl, x.Removed, x.OrganizationId
                            , x.Verified, x.CreatedAt, context.User.Where(s => s.UserId == x.AuthorId).FirstOrDefault().FullName,
                            context.User.Where(s => s.UserId == x.AuthorId).FirstOrDefault().UserId,
                            context.User.Where(s => s.UserId == x.AuthorId).FirstOrDefault().Actived))
                            .ToList();
                    var access = context.Access.Where(s => s.UserId == userID && s.OrganizationId == organization.OrganizationId).FirstOrDefault();

                    organizationResponses.Add(new OrganizationResponseForAdmin(organization.OrganizationId, organization.Name, access.Role, organization.Removed,websites));
                }

                return new OkObjectResult(
                    new
                    {
                        organizations = organizationResponses,
                        user = new UserResponse(temp.UserId, temp.FullName, temp.Email)

                    });
            }
            else
            {
                return new BadRequestResult();
            }
        }

        public IActionResult lockWebsite(LockRequest request)
        {
            try
            {
                Website website = context.Website.FirstOrDefault(s => s.WebId == request.ID);
                if (website != null)
                {
                    website.Removed = request.locked;
                    context.SaveChanges();
                }
                return new OkObjectResult(website);
            }
            catch (Exception ex)
            {
                return new UnprocessableEntityResult();
            }
        }

        public IActionResult getAllWebSite()
        {
            List<Website> websites =  context.Website.ToList();
            List<WebsiteResponseForAdmin> responses = new List<WebsiteResponseForAdmin>();
            foreach (var website in websites)
            {
                User user = context.User.Where(s => s.UserId == website.AuthorId).FirstOrDefault();
                if (user == null) return null;
                WebsiteResponseForAdmin response = new WebsiteResponseForAdmin(website.WebId, website.DomainUrl, website.Removed
                    , website.OrganizationId, website.Verified, website.CreatedAt, user.FullName,user.UserId,user.Actived);
                responses.Add(response);
            }
            return new OkObjectResult(responses);
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
        public IActionResult inviteUser(int userID,string email,int orgID, int roleID)
        {
            if(!checkOrgAuthencation(orgID, userID)) return new UnauthorizedResult();
            var user = context.User.Where(s => s.Email == email).FirstOrDefault();
            if (user == null) return new NotFoundResult();
            Access access = new Access();
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            access.DayJoin = (long)timeSpan.TotalSeconds;
            access.OrganizationId = orgID;
            access.UserId = user.UserId;
            access.Role = roleID;
            context.Access.Add(access);
            context.SaveChanges();
            return new OkObjectResult(access);
        }

        private Boolean checkOrgAuthencation(int orgID, int userId)
        {
            Access access = context.Access.Where(s => s.UserId == userId && s.Role ==1 && s.OrganizationId == orgID).FirstOrDefault();
            if (access == null) return false;
            return true;
        }

        public IActionResult uninviteUser(int userID, string email, int orgID)
        {
            if (!checkOrgAuthencation(orgID, userID)) return new UnauthorizedResult();
            var user = context.User.Where(s => s.Email == email).FirstOrDefault();
            if (user == null) return new NotFoundResult();
            Access access = context.Access.Where(s => s.UserId == user.UserId && s.OrganizationId == orgID).
                Where(s => s.Role == 2 || s.Role == 3).FirstOrDefault();
            if (access == null) return new NotFoundResult();
            context.Access.Remove(access);
            context.SaveChanges();
            return new OkObjectResult(access);
        }

        public IActionResult changeRole(int userID, string email, int orgID)
        {
            if (!checkOrgAuthencation(orgID, userID)) return new UnauthorizedResult();
            var user = context.User.Where(s => s.Email == email).FirstOrDefault();
            if (user == null) return new NotFoundResult();
            Access access = context.Access.Where(s => s.UserId == user.UserId && s.OrganizationId == orgID).FirstOrDefault();
            if (access == null) return new NotFoundResult();
            if (access.Role == 2) access.Role = 3; else access.Role = 2;
            context.SaveChanges();
            return new OkObjectResult(access);
        }
    }
}
