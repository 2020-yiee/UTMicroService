using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UserAPIServices.EFModels;
using UserAPIServices.Models;
using UserAPIServices.Repository;
using UserAPIServices.Repository.AdminRepository;

namespace UserAPIServices.Controllers
{
    [EnableCors]
    [Route("api/admin")]
    public class AdminController : Controller
    {
        protected int GetUserId()
        {
            try
            {
                return int.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        IAdminRepository repository;

        public AdminController(IAdminRepository repository)
        {
            this.repository = repository;
        }
        //==========================================admin====================================================

        [Authorize(Roles = "admin")]
        [HttpGet("users")]
        public IActionResult GetAllUser()
        {
            IEnumerable<User> result = repository.getAllUsers();
            if (result != null) return Ok(result);
            return BadRequest();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("user/lock")]
        public IActionResult lockUser([FromBody] LockRequest request)
        {
            return repository.lockUser(request);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("ors-and-webs")]
        public IActionResult getUserOrganizationAndWebsites(int userID)
        {
            return repository.getAllUserOrganizationAndWebsites(userID);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("websites")]
        public IActionResult getAllWebsites()
        {
            return repository.getAllWebSite();

        }

        [Authorize(Roles = "admin")]
        [HttpPut("website/lock")]
        public IActionResult lockWebsites([FromBody] LockRequest request)
        {
            return repository.lockWebsite(request);
        }
    }
}