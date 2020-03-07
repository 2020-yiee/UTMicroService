using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomersAPIServices.EFModels;
using CustomersAPIServices.Models.RequestModels;
using CustomersAPIServices.Models.ResponseModels;
using CustomersAPIServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomersAPIServices.Controllers
{
    [Route("api/[controller]")]
    public class WebOwnerController : Controller
    {
        IWebOwnerRepository repository;
        [Authorize]
        [HttpGet]
        public IActionResult GetAllWebOwners()
        {
            repository = new WebOwnerRepositoryImpl();
            IEnumerable<WebOwnerResponse> result = repository.getAllWebOwners();
            if (result != null) return Ok(result);
            return NotFound();
        }

        [Authorize]
        [HttpGet("WebOwner")]   
        public IActionResult getWebOwner([FromBody] GetWebOwnerRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            WebOwnerResponse result = repository.getWebOwner(request);
            if (result!=null) return Ok(result);
            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateWebOwner([FromBody] CreateWebOwnerRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            var webOwner = repository.createWebOwner(request);

            if (webOwner != null) return Ok(webOwner);
            return StatusCode(400);
        }

        [HttpDelete]
        [Authorize]
        public IActionResult deleteWebOwner(int customerId)
        {
            repository = new WebOwnerRepositoryImpl();
            bool result = repository.deleteWebOwner(customerId);
            if (result) return Ok();
            return BadRequest();
        }

        [HttpPut]
        [Authorize]
        public IActionResult updateWebOwner([FromBody] UpdateWebOwnerRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            bool result = repository.updateWebOwner(request);
            if (result) return Ok();
            return BadRequest();
        }
        
        //=======================================================================================

        [HttpGet("websites")]
        [Authorize]
        public IActionResult getWebSites(int customerId)
        {
            repository = new WebOwnerRepositoryImpl();
            IEnumerable<WebsiteResponse> result = repository.getWebsites(customerId);
            if (result != null) return Ok(result);
            return NotFound();
        }

        [HttpPost("website")]
        [Authorize]
        public IActionResult CreateWebSite([FromBody] CreateWebsiteRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            var website = repository.createWebsite(request);

            if (website != null) return Ok(website);
            return StatusCode(400);
        }

        [HttpDelete("website")]
        [Authorize]
        public IActionResult deleteWebsite([FromBody] DeleteWebsiteRequest request)
        {
            repository = new WebOwnerRepositoryImpl();
            bool result = repository.deleteWebsite(request.customerId,request.webId);
            if (result) return Ok();
            return BadRequest();
        }
    }
}