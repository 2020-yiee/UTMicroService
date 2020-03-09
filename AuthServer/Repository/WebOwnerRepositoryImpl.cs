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
    public class WebOwnerRepositoryImpl : IWebOwnerRepository
    {
        private readonly IHelperFunction _helper;
        public WebOwnerRepositoryImpl(IHelperFunction helper)
        {
            _helper = helper;
        }
        public async Task<Object> getCustomerByUsernameAndPassword(LoginRequestModel model)
        {
            using (var context = new DBUTContext())
            {
                var customer = await context.WebOwner
                    .Where(s => s.Username == model.username)
                    .Where(s => s.IsRemoved == false)
                    .FirstOrDefaultAsync();
                bool checkPassword = false;
                if (customer != null)
                {
                    //LoginSuccessModel resultReturn = new LoginSuccessModel();
                    checkPassword = BCrypt.Net.BCrypt.Verify(model.password, customer.Password);
                    if (checkPassword)
                    {
                        //string role = customer.Role;
                        //string fullname = customer.FullName;
                        //resultReturn.Id = customer.WebOwnerId;
                        //resultReturn.FullName = fullname;
                        //resultReturn.Role = role;
                        //resultReturn.Token = _helper.GenerateJwtToken(model.username, customer, role);
                        //return resultReturn;
                        var website = context.Website
                            .Where(x => x.WebOwnerId == customer.WebOwnerId)
                            .ToList()
                            .Select(x => new WebsiteResponse(x.WebId, x.WebOwnerId, x.WebUrl, x.IsRemoved))
                            .ToList();
                        return new
                        {
                            profile = new WebOwnerResponse(customer.WebOwnerId, customer.Username, customer.FullName, customer.Email, customer.Role),
                            token = _helper.GenerateJwtToken(model.username, customer, customer.Role),
                            websites = website

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
