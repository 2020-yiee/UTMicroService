using AuthServer.EFModels;
using AuthServer.Helper;
using AuthServer.Models;
using AuthServer.Models.ResponseModels;
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
        public async Task<Object> getCustomerByUsernameAndPassword(LoginRequestModel model)
        {
            using (var context = new DBUTContext())
            {
                var user = await context.User
                    .Where(s => s.Email == model.email)
                    .Where(s => s.Actived == true)
                    .FirstOrDefaultAsync();
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
                            return new
                            {
                                token = _helper.GenerateJwtToken(model.email, user, "user"),
                                organizations = new List<Object>(),
                                user = new {userID = user.UserId,fullName = user.FullName, email =  user.Email }
                                
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
                                    .Select(x => new WebsiteResponse(x.WebId,x.DomainUrl, x.Removed, x.OrganizationId))
                                    .ToList();

                            organizationResponses.Add(new OrganizationResponse(organization.OrganizationId, organization.Name, websites));
                        }

                        return new
                        {
                            token = _helper.GenerateJwtToken(model.email, user, "user"),
                            organizations = organizationResponses,
                            user = new { userID = user.UserId, fullName = user.FullName, email = user.Email }
                            
                        };
                    }
                    else
                    {
                        return false;
                    }

                }
                return null;
            }
        }
    }
}
