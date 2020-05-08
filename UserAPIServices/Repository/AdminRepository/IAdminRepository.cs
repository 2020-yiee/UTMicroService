using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPIServices.EFModels;
using UserAPIServices.Models;

namespace UserAPIServices.Repository.AdminRepository
{
    public interface IAdminRepository
    {
        IEnumerable<User> getAllUsers();
        IActionResult lockUser(LockRequest request);
        IActionResult getAllUserOrganizationAndWebsites(int userID);
        IActionResult getAllWebSite();
        IActionResult lockWebsite(LockRequest request);

    }
}
