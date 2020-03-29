using AuthServer.EFModels;
using AuthServer.Helper;
using AuthServer.Models;
using AuthServer.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Repository
{
    public class AuthUserRepositoryImpl : IAuthUserRepository
    {
        private readonly IHelperFunction _helper;
        public AuthUserRepositoryImpl(IHelperFunction helper)
        {
            _helper = helper;
        }

        public IActionResult getAdminByEmailAndPassword(LoginRequestModel model)
        {
            using (var context = new DBUTContext())
            {
                var admin = context.Admin
                    .Where(s => s.Email == model.email)
                    .Where(s => s.Actived == true)
                    .FirstOrDefault();
                bool checkPassword = false;
                if (admin != null)
                {
                    //LoginSuccessModel resultReturn = new LoginSuccessModel();
                    checkPassword = BCrypt.Net.BCrypt.Verify(model.password, admin.Password);
                    if (checkPassword)
                    {
                        int totalUsers = context.User.Count();
                        int totalWebsites = context.Website.Count();

                        return new OkObjectResult(new
                        {
                            admin = admin,
                            token = _helper.GenerateJwtToken(model.email, admin, "admin"),
                            totalUsers = totalUsers,
                            totalWebsites = totalWebsites

                        }) ;
                    }
                    else
                    {
                        return new BadRequestResult();
                    }

                }
                return new NotFoundResult();
            }
        }

        public IActionResult getUserByEmailAndPassword(LoginRequestModel model)
        {
            using (var context = new DBUTContext())
            {
                var user = context.User
                    .Where(s => s.Email == model.email)
                    .Where(s => s.Actived == true)
                    .FirstOrDefault();
                bool checkPassword = false;
                if (user != null)
                {
                    //LoginSuccessModel resultReturn = new LoginSuccessModel();
                    checkPassword = BCrypt.Net.BCrypt.Verify(model.password, user.Password);
                    if (checkPassword)
                    {
                        List<int> organizationIds = context.Access.Where(s => s.UserId == user.UserId).Select(s => s.OrganizationId).ToList();
                        if (organizationIds == null || organizationIds.Count == 0)
                        {
                            return new OkObjectResult(new
                            {
                                token = _helper.GenerateJwtToken(model.email, user, "user"),
                                organizations = new List<Object>(),
                                user = new { userID = user.UserId, fullName = user.FullName, email = user.Email }

                            });
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
                                    , x.Verified, x.CreatedAt, context.User.Where(s => s.UserId == x.AuthorId).FirstOrDefault().FullName))
                                    .ToList();
                            var access = context.Access.Where(s => s.UserId == user.UserId && s.OrganizationId == organization.OrganizationId).FirstOrDefault();

                            organizationResponses.Add(new OrganizationResponse(organization.OrganizationId, organization.Name, access.Role, websites));
                        }

                        return new OkObjectResult(new
                        {
                            token = _helper.GenerateJwtToken(model.email, user, "user"),
                            organizations = organizationResponses,
                            user = new { userID = user.UserId, fullName = user.FullName, email = user.Email }

                        });
                    }
                    else
                    {
                        return new BadRequestResult();
                    }

                }
                return new NotFoundResult();
            }
        }

        public IActionResult processForgotPassword(string mail)
        {
            using (var context = new DBUTContext())
            {
                var user = context.User.Where(s => s.Email == mail).FirstOrDefault();
                if (user == null) return new NotFoundResult();
                var password = _helper.RandomPassword();
                user.Password = Hashing.HashPassword(password);
                context.SaveChanges();
                bool sendMail = _helper.sendMail(user.Email, user.FullName, password);
                if (sendMail) return new OkResult();
                else return new UnprocessableEntityResult();

            }
        }
    }
}
