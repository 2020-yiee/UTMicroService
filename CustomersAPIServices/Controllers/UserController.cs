using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using CustomersAPIServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CustomersAPIServices.Controllers
{
    [EnableCors]
    public class UserController : Controller
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

        IUserRepository repository;
        [Authorize(Roles ="admin")]
        [HttpGet("api/users")]
        public IActionResult GetAllUser()
        {
            repository = new UserRepositoryImpl();
            IEnumerable<UserResponse> result = repository.getAllWebOwners();
            if (result != null) return Ok(result);
            return BadRequest();
        }

        [Authorize]
        [HttpGet("api/user")]   
        public IActionResult getUser()
        {
            repository = new UserRepositoryImpl();
            try
            {
                Object result = repository.getUser(GetUserId());
                if (result != null) return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
            
            return BadRequest();
        }

        [HttpPost("api/user")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            repository = new UserRepositoryImpl();
            var user = repository.createUser(request);
            if (user != null) return Ok(user);
            return BadRequest(user);
        }

        [HttpPut("api/user")]
        [Authorize]
        public IActionResult updateUser([FromBody] UpdateUserRequest request)
        {
            repository = new UserRepositoryImpl();
            bool result = repository.updateUser(request,GetUserId());
            if (result) return Ok();
            return BadRequest();
        }

        [HttpDelete("api/user")]
        [Authorize]
        public IActionResult deleteUser()
        {
            repository = new UserRepositoryImpl();
            bool result = repository.deleteUser(GetUserId());
            if (result) return Ok();
            return BadRequest();
        }

        [HttpGet("api/user/check")]
        public IActionResult checkUsernameOrEmail(string email)
        {
            repository = new UserRepositoryImpl();
            var result = repository.checkUserEmail( email);
            if (result != null) return Ok(result);
            return BadRequest();
        }
        
        //=======================================================================================

        [HttpPost("api/website/verify")]
        public IActionResult verifyWebsite([FromBody] verifiedRequest request)
        {
            repository = new UserRepositoryImpl();
            bool result = repository.verifyWebsite(request);
            if (result) return Ok();
            return BadRequest();
        }


        [HttpGet("api/user/websites")]
        [Authorize]
        public IActionResult getWebSites()
        {
            repository = new UserRepositoryImpl();
            IEnumerable<WebsiteResponse> result = repository.getWebsites(GetUserId());
            if (result != null) return Ok(result);
            return BadRequest();
        }

        [HttpPost("api/user/website")]
        [Authorize]
        public IActionResult CreateWebSite([FromBody] CreateWebsiteRequest request)
        {
            repository = new UserRepositoryImpl();
            var website = repository.createWebsite(request,GetUserId());

            if (website != null) return Ok(website);
            return BadRequest();
        }

        [HttpDelete("api/user/website")]
        [Authorize]
        public IActionResult deleteWebsite([FromBody] DeleteWebsiteRequest request)
        {
            repository = new UserRepositoryImpl();
            bool result = repository.deleteWebsite(GetUserId(),request.webID);
            if (result) return Ok();
            return BadRequest();
        }



        //===================================================================================

        [Authorize]
        [HttpPost("api/user/organization")]
        public IActionResult createOrganization([FromBody] OrganizationRequest request)
        {
            repository = new UserRepositoryImpl();
            Object result = repository.createOrganization(request,GetUserId());
            if (result != null) return Ok(result);
            return BadRequest();
        }
        [Authorize]
        [HttpPut("api/user/organization")]
        public IActionResult updateOrganization([FromBody] UpdateOrganizationRequest request)
        {
            repository = new UserRepositoryImpl();
            Object result = repository.updateOrganization(request,GetUserId());
            if (result != null) return Ok(result);
            return BadRequest();
        }
        [Authorize]
        [HttpDelete("api/user/organization")]
        public IActionResult DeleteOrganization(int organizationID)
        {
            repository = new UserRepositoryImpl();
            Object result = repository.DeleteOrganization(organizationID,GetUserId());
            if (result != null) return Ok(result);
            return BadRequest();
        }
    }
}