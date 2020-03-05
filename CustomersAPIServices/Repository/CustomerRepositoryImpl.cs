using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Repository
{
    public class WebOwnerRepositoryImpl : IWebOwnerRepository
    {
        private DBUTContext context = new DBUTContext();
        public Object createWebOwner(CreateWebOwnerRequest webOwner)
        {
            WebOwner addWebOwner = new WebOwner();
            addWebOwner.Email = webOwner.Email;
            addWebOwner.FullName = webOwner.FullName;
            addWebOwner.Username = webOwner.Username;
            addWebOwner.Password = Hashing.HashPassword(webOwner.Password);
            addWebOwner.Role = "WebOwner";
            try
            {
                context.WebOwner.Add(addWebOwner);
                context.SaveChanges();
                WebOwner temp = context.WebOwner
                    .Where(s => s.Username == addWebOwner.Username)
                    .FirstOrDefault();
                WebsiteResponse website = createWebsite(new CreateWebsiteRequest(temp.WebOwnerId, webOwner.WebUrl));
                return new
                {
                    webOwner = new WebOwnerResponse(temp.WebOwnerId, temp.Username, temp.FullName, temp.Email, temp.Role),
                    website = website
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public bool deleteWebOwner(int webOwnerId)
        {
            WebOwner webOwner = context.WebOwner.Where(s => s.WebOwnerId == webOwnerId)
                .FirstOrDefault();
            try
            {
                webOwner.IsRemoved = true;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public IEnumerable<WebOwnerResponse> getAllWebOwners()
        {
            List<WebOwnerResponse> result = new List<WebOwnerResponse>();
            List<WebOwner> webOwners = context.WebOwner.Where(s => s.IsRemoved == false).ToList();
            foreach (var webOwner in webOwners)
            {
                result.Add(new WebOwnerResponse(webOwner.WebOwnerId, webOwner.Username, webOwner.FullName, webOwner.Email, webOwner.Role));
            }
            return result;
        }

        public WebOwnerResponse getWebOwner(GetWebOwnerRequest request)
        {
            WebOwner webOwner = context.WebOwner.Where(s => s.WebOwnerId == request.WebOwnerId)
                .Where(s => s.Email == request.Email).Where(s => s.IsRemoved == false).FirstOrDefault();

            return new WebOwnerResponse(webOwner.WebOwnerId, webOwner.Username, webOwner.FullName, webOwner.Email, webOwner.Role);
        }

        public bool updateWebOwner(UpdateWebOwnerRequest request)
        {
            WebOwner webOwner = context.WebOwner.Where(s => s.WebOwnerId == request.WebOwnerId)
                .FirstOrDefault();
            webOwner.FullName = request.FullName;
            webOwner.Email = request.Email;
            webOwner.Role = request.Role;
            try
            {
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public IEnumerable<WebsiteResponse> getWebsites(int webOwnerId)
        {
            try
            {
                IEnumerable<Website> websites = context.Website
                    .Where(s => s.WebOwnerId == webOwnerId).ToList();
                List<WebsiteResponse> results = new List<WebsiteResponse>();
                foreach (Website website in websites)
                {
                    results.Add(new WebsiteResponse(website.WebId, website.WebOwnerId, website.WebUrl, website.IsRemoved));
                }
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public bool deleteWebsite(int webOwnerId, int webId)
        {
            try
            {
                Website website = context.Website
                    .Where(s => s.WebOwnerId == webOwnerId)
                    .Where(s => s.WebId == webId)
                    .FirstOrDefault();
                if (website != null)
                {
                    website.IsRemoved = true;
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

        public WebsiteResponse createWebsite(CreateWebsiteRequest request)
        {
            Website website = new Website();
            website.WebOwnerId = request.webOwnerId;
            website.WebUrl = request.webUrl;
            website.IsRemoved = false;
            try
            {
                context.Website.Add(website);
                context.SaveChanges();
                Website temp = context.Website.Where(s => s.WebOwnerId == website.WebOwnerId)
                    .Where(s => s.WebUrl == website.WebUrl).FirstOrDefault();
                return new WebsiteResponse(temp.WebId, temp.WebOwnerId, temp.WebUrl, temp.IsRemoved);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }
    }
}
