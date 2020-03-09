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
    public class WebOwnerController : Controller
    {

        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
        }

        IWebOwnerRepository repository;
        [Authorize(Roles ="admin")]
        [HttpGet("api/web-owners")]
        public IActionResult GetAllWebOwners()
        {
            repository = new WebOwnerRepositoryImpl();
            IEnumerable<WebOwnerResponse> result = repository.getAllWebOwners();
            if (result != null) return Ok(result);
            return NotFound();
        }

        [Authorize]
        [HttpGet("api/web-owner")]   
        public IActionResult getWebOwner()
        {
            repository = new WebOwnerRepositoryImpl();
            Object result = repository.getWebOwner(GetUserId());
            if (result!=null) return Ok(result);
            return NotFound();
        }

        [HttpPost("api/web-owner")]
        public IActionResult CreateWebOwner([FromBody] CreateWebOwnerRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            var webOwner = repository.createWebOwner(request);
            if (webOwner != null) return Ok(webOwner);
            return BadRequest(webOwner);
        }

        [HttpDelete("api/web-owner")]
        [Authorize]
        public IActionResult deleteWebOwner(int webOwnerId)
        {
            repository = new WebOwnerRepositoryImpl();
            bool result = repository.deleteWebOwner(webOwnerId);
            if (result) return Ok();
            return BadRequest();
        }

        [HttpPut("api/web-owner")]
        [Authorize]
        public IActionResult updateWebOwner([FromBody] UpdateWebOwnerRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            bool result = repository.updateWebOwner(request);
            if (result) return Ok();
            return BadRequest();
        }

        [HttpGet("api/web-owner/check")]
        public IActionResult checkUsernameOrEmail(string username, string email)
        {
            repository = new WebOwnerRepositoryImpl();
            var result = repository.checkUsernnameOrEmail(username, email);
            if (result != null) return Ok(result);
            return BadRequest();
        }
        
        //=======================================================================================

        [HttpGet("api/web-owner/websites")]
        [Authorize]
        public IActionResult getWebSites()
        {
            repository = new WebOwnerRepositoryImpl();
            IEnumerable<WebsiteResponse> result = repository.getWebsites(GetUserId());
            if (result != null) return Ok(result);
            return NotFound();
        }

        [HttpPost("api/web-owner/website")]
        [Authorize]
        public IActionResult CreateWebSite([FromBody] CreateWebsiteRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            var website = repository.createWebsite(request);

            if (website != null) return Ok(website);
            return StatusCode(400);
        }

        [HttpDelete("api/web-owner/website")]
        [Authorize]
        public IActionResult deleteWebsite([FromBody] DeleteWebsiteRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            bool result = repository.deleteWebsite(request.webOwnerId,request.webId);
            if (result) return Ok();
            return BadRequest();
        }
    }
}