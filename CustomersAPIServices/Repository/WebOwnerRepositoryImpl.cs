using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CustomersAPIServices.Repository
{
    public class UserRepositoryImpl : IWebOwnerRepository
    {
       
        private DBUTContext context = new DBUTContext();
        public Object createUser(CreateUserRequest request)
        {
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
                organizationResponses.Add(new OrganizationResponse(organization.OrganizationId, organization.Name,websites));
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

        public IEnumerable<UserResponse> getAllWebOwners()
        {
            List<UserResponse> result = new List<UserResponse>();
            List<User> webOwners = context.User.Where(s => s.Actived == true).ToList();
            foreach (var webOwner in webOwners)
            {
                result.Add(new UserResponse(webOwner.UserId, webOwner.FullName, webOwner.Email));
            }
            return result;
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
                            .Select(x => new WebsiteResponse(x.WebId, x.DomainUrl, x.Removed, x.OrganizationId,x.Verified))
                            .ToList();

                    organizationResponses.Add(new OrganizationResponse(organization.OrganizationId, organization.Name, websites));
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
                var websites = context.Website
                    .Where(s => s.Removed == false)
                    .Select(x => new WebsiteResponse(x.WebId, x.DomainUrl, x.Removed,x.OrganizationId,x.Verified)).ToList();
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
                Website website = context.Website
                    //.Where(s => s.WebOwnerId == webOwnerId)
                    //.Where(s => s.WebId == webId)
                    .FirstOrDefault(x => x.WebId == webId);
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
            try
            {
                context.Website.Add(website);
                context.SaveChanges();
                //Website temp = context.Website.Where(s => s.WebOwnerId == website.WebOwnerId)
                //    .Where(s => s.WebUrl == website.WebUrl).FirstOrDefault();
                return new WebsiteResponse(website.WebId, website.DomainUrl, website.Removed,website.OrganizationId,website.Verified);
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
    }
}
