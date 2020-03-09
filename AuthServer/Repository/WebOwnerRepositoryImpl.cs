using AuthServer.EFModels;
using AuthServer.Helper;
using AuthServer.Models;
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
        public async Task<LoginSuccessModel> getCustomerByUsernameAndPassword(LoginRequestModel model)
        {
            using (var context = new DBUTContext())
            {
                var customer =  await context.WebOwner
                    .Where(s => s.Username == model.username)
                    .Where(s => s.IsRemoved == false)
                    .FirstOrDefaultAsync();
                bool checkPassword = false;
                if (customer != null)
                {
                    LoginSuccessModel resultReturn = new LoginSuccessModel();
                    checkPassword = BCrypt.Net.BCrypt.Verify(model.password, customer.Password);
                    if (checkPassword)
                    {
                        string role = customer.Role;
                        string fullname = customer.FullName;
                        resultReturn.Id = customer.WebOwnerId;
                        resultReturn.FullName = fullname;
                        resultReturn.Role = role;
                        resultReturn.Token = _helper.GenerateJwtToken(model.username, customer, role);
                        return resultReturn;
                    }
                    
                }
                return null;
            }
        }
    }
}
