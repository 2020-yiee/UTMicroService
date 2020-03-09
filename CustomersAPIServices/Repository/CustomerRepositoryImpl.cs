﻿using CustomersAPIServices.EFModels;
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
            addWebOwner.Email = webOwner.email;
            addWebOwner.FullName = webOwner.fullName;
            addWebOwner.Username = webOwner.username;
            addWebOwner.Password = Hashing.HashPassword(webOwner.password);
            addWebOwner.Role = "WebOwner";
            try
            {
                context.WebOwner.Add(addWebOwner);
                context.SaveChanges();
                WebOwner temp = context.WebOwner
                    .Where(s => s.Username == addWebOwner.Username)
                    .FirstOrDefault();
                WebsiteResponse website = createWebsite(new CreateWebsiteRequest(temp.WebOwnerId, webOwner.webUrl));
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

        public WebOwnerResponse getWebOwner(int webOwnerId)
        {
            WebOwner webOwner = context.WebOwner.Where(s => s.WebOwnerId == webOwnerId).FirstOrDefault();
            if(webOwner!=null)
            return new WebOwnerResponse(webOwner.WebOwnerId, webOwner.Username, webOwner.FullName, webOwner.Email, webOwner.Role);
            return null;
        }

        public bool updateWebOwner(UpdateWebOwnerRequest request)
        {
            WebOwner webOwner = context.WebOwner.Where(s => s.WebOwnerId == request.webOwnerId)
                .FirstOrDefault();
            webOwner.FullName = request.fullName;
            webOwner.Email = request.email;
            webOwner.Role = request.role;
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
                var websites = context.Website
                    .Where(s => s.WebOwnerId == webOwnerId).Select(x=> new  WebsiteResponse(x.WebId, x.WebOwnerId, x.WebUrl, x.IsRemoved)).ToList();
                return websites;
                //List<WebsiteResponse> results = new List<WebsiteResponse>();
                //foreach (Website website in websites)
                //{
                //    results.Add(new WebsiteResponse(website.WebId, website.WebOwnerId, website.WebUrl, website.IsRemoved));
                //}
                //return results;
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
                    //.Where(s => s.WebOwnerId == webOwnerId)
                    //.Where(s => s.WebId == webId)
                    .FirstOrDefault(x=>x.WebOwnerId == webOwnerId && x.WebId == webId);
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
                //Website temp = context.Website.Where(s => s.WebOwnerId == website.WebOwnerId)
                //    .Where(s => s.WebUrl == website.WebUrl).FirstOrDefault();
                return new WebsiteResponse(website.WebId, website.WebOwnerId, website.WebUrl, website.IsRemoved);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }

        public object checkUsernnameOrEmail(string username, string email)
        {
            WebOwner owner = context.WebOwner.Where(s => s.Username == username).FirstOrDefault();
            if (owner != null) return new
            {
                type = "ERROR",
                nameMessage = "Username has been existed"
            };
            owner = context.WebOwner.Where(s => s.Email == email).FirstOrDefault();
            if (owner != null) return new
            {
                type = "ERROR",
                emailMessage = "Email has been existed"
            };
            return new { type = "SUCCESS"};
            
        }
    }
}
