using AuthServer.EFModels;
using AuthServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Repository
{
    public interface IWebOwnerRepository
    {
        Task<LoginSuccessModel> getCustomerByUsernameAndPassword(LoginRequestModel model);

    }
}
