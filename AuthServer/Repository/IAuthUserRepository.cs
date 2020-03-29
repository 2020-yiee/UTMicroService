using AuthServer.EFModels;
using AuthServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Repository
{
    public interface IAuthUserRepository
    {
        IActionResult getUserByEmailAndPassword(LoginRequestModel model);
        IActionResult getAdminByEmailAndPassword(LoginRequestModel model);
        IActionResult processForgotPassword(string mail);
    }
}
