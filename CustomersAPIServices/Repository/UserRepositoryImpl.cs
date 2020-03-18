using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
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
                List<int> organizationIds = context.Access.Where(s => s.UserId == user.UserId).Select(s => s.OrganizationId).ToList();
                var websites = context.Website
                    .Where(s => s.Removed == false)
                    .Where(s => organizationIds.Contains(s.OrganizationId) == true)
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

        public object getStatisticData(int webID, int trackingInfoID, int from, int to, int userId)
        {
            GetStatisicHeatMap response = new GetStatisicHeatMap();
            try
            {
                if (!checkAuthencation(webID, userId)) return null;
                TrackingHeatmapInfo trackingHeatmapInfo = context.TrackingHeatmapInfo.Where(s => s.WebId == webID)
                .Where(s => s.TrackingHeatmapInfoId == trackingInfoID)
                .FirstOrDefault();
                response.imageUrl = trackingHeatmapInfo.ImageUrl;
                foreach (var eventType in EVENT_TYPE_LIST)
                {
                    List<int> trackedHeatmapDataIds = context.TrackedHeatmapData.Where(s => s.WebId == webID)
                    .Where(s => s.TrackingUrl == trackingHeatmapInfo.TrackingUrl)
                    .Where(s => s.CreatedAt >= from)
                    .Where(s => s.CreatedAt <= to)
                    .Where(s => s.EventType == eventType)
                    .Select(s => s.TrackedHeatmapDataId).ToList();
                    List<StatisticHeatmap> statisticHeatmaps = context.StatisticHeatmap
                        .Where(s => trackedHeatmapDataIds.Contains(s.TrackedHeatmapDataId) == true)
                    .ToList();
                    List<StatisticData> statisticDatas = new List<StatisticData>();
                    foreach (var item in statisticHeatmaps)
                    {
                        statisticDatas.AddRange(JsonConvert.DeserializeObject<List<StatisticData>>(item.StatisticData));
                    }
                    if(eventType == 0)response.click = JsonConvert.SerializeObject(statisticDatas);
                    if(eventType == 1)response.hover = JsonConvert.SerializeObject(statisticDatas);
                    

                }
                
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
                throw;
            }
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
            return context.Website.ToList();
        }
    }
}
