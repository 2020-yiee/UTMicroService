using AuthServer.EFModels;
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
        public WebOwner getCustomerByUsernameAndPassword(LoginRequestModel model)
        {
            using (var context = new DBUTContext())
            {
                var customer = context.WebOwner
                    .Where(s => s.Username == model.username)
                    .Where(s => s.IsRemoved == false)
                    .FirstOrDefault();
                if (customer != null)
                {
                    return Hashing.ValidatePassword(model.password, customer.Password) ? customer : null;
                }
                return null;
            }
        }
    }
}
