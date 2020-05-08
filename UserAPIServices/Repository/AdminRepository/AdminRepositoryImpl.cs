using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPIServices.EFModels;
using UserAPIServices.Models;
using UserAPIServices.Models.ResponseModels;

namespace UserAPIServices.Repository.AdminRepository
{
    public class AdminRepositoryImpl : IAdminRepository
    {
        public IActionResult getAllUserOrganizationAndWebsites(int userID)
        {
            using (var context = new DBUTContext())
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

                        organizationResponses.Add(new OrganizationResponseForAdmin(organization.OrganizationId, organization.Name, access.Role, organization.Removed, websites));
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
        }

        public IEnumerable<User> getAllUsers()
        {
            using (var context = new DBUTContext())
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
        }

        public IActionResult getAllWebSite()
        {
            using (var context = new DBUTContext())
            {
                List<Website> websites = context.Website.ToList();
                List<WebsiteResponseForAdmin> responses = new List<WebsiteResponseForAdmin>();
                foreach (var website in websites)
                {
                    User user = context.User.Where(s => s.UserId == website.AuthorId).FirstOrDefault();
                    if (user == null) return null;
                    WebsiteResponseForAdmin response = new WebsiteResponseForAdmin(website.WebId, website.DomainUrl, website.Removed
                        , website.OrganizationId, website.Verified, website.CreatedAt, user.FullName, user.UserId, user.Actived);
                    responses.Add(response);
                }
                return new OkObjectResult(responses);
            }
        }

        public IActionResult lockUser(LockRequest request)
        {
            using (var context = new DBUTContext())
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
        }

        public IActionResult lockWebsite(LockRequest request)
        {
            using (var context = new DBUTContext())
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
        }
    }
}
