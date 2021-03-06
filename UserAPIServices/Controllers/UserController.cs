﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPIServices.EFModels;
using UserAPIServices.Models;
using UserAPIServices.Models.RequestModels;
using UserAPIServices.Models.ResponseModels;
using UserAPIServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace UserAPIServices.Controllers
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

        public UserController(IUserRepository repository)
        {
            this.repository = repository;
        }

        
        //====================================user crud======================================================
        [Authorize]
        [HttpGet("api/user")]   
        public IActionResult getUser()
        {
                return repository.getNewInviteMember(GetUserId());
            
        }

        [HttpPost("api/user")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            return repository.createUser(request);
            
        }

        [HttpPut("api/user")]
        [Authorize]
        public IActionResult updateUser([FromBody] UpdateUserRequest request)
        {
            bool result = repository.updateUser(request,GetUserId());
            if (result) return Ok();
            return BadRequest();
        }

        [HttpDelete("api/user")]
        [Authorize]
        public IActionResult deleteUser()
        {
            bool result = repository.deleteUser(GetUserId());
            if (result) return Ok();
            return BadRequest();
        }

        [HttpGet("api/user/check")]
        public IActionResult checkUsernameOrEmail(string email)
        {
            return repository.checkUserEmail( email);
        }

        [Authorize]
        [HttpPost("api/user/organization/member/invite")]
        public IActionResult inviteUser([FromBody] inviteUserRequest inviteUserRequest)
        {
            return repository.inviteUser(GetUserId(), inviteUserRequest.email, inviteUserRequest.organizationID, inviteUserRequest.roleID);
        }

        [Authorize]
        [HttpPost("api/user/organization/member/invite/new")]
        public IActionResult inviteNewUser([FromBody] InviteNewUserRequest request)
        {
            return repository.inviteNewUser(request, GetUserId()
                , this.User.Claims.First(i => i.Type == "mail").Value
                , int.Parse(this.User.Claims.First(i => i.Type == "orgID").Value)
                , int.Parse(this.User.Claims.First(i => i.Type == "orgRole").Value));
        }


        [Authorize]
        [HttpDelete("api/user/organization/member/uninvite")]
        public IActionResult uninviteUser(string email, int organizationID)
        {
            return repository.uninviteUser(GetUserId(), email, organizationID);
        }

        [Authorize]
        [HttpPut("api/user/organization/member/change-role")]
        public IActionResult changeRole([FromBody] changeRoleMemberRequest changeRoleMemberRequest) 
        {
            return repository.changeRole(GetUserId(), changeRoleMemberRequest.email, changeRoleMemberRequest.organizationID);
        }

        //===========================================website============================================

        [HttpPost("api/website/verify")]
        public IActionResult verifyWebsite([FromBody] verifiedRequest request)
        {
            bool result = repository.verifyWebsite(request);
            if (result) return Ok();
            return BadRequest();
        }


        [HttpGet("api/user/websites")]
        [Authorize]
        public IActionResult getWebSites()
        {
            IEnumerable<WebsiteResponse> result = repository.getWebsites(GetUserId());
            if (result != null) return Ok(result);
            return BadRequest();
        }

        [HttpPost("api/user/website")]
        [Authorize]
        public IActionResult CreateWebSite([FromBody] CreateWebsiteRequest request)
        {
            var website = repository.createWebsite(request,GetUserId());

            if (website != null) return Ok(website);
            return BadRequest();
        }

        [HttpDelete("api/user/website")]
        [Authorize]
        public IActionResult deleteWebsite([FromBody] DeleteWebsiteRequest request)
        {
            bool result = repository.deleteWebsite(GetUserId(),request.webID);
            if (result) return Ok();
            return BadRequest();
        }
        //========================================Organization===========================================

        [Authorize]
        [HttpPost("api/user/organization")]
        public IActionResult createOrganization([FromBody] OrganizationRequest request)
        {
            Object result = repository.createOrganization(request,GetUserId());
            if (result != null) return Ok(result);
            return BadRequest();
        }
        [Authorize]
        [HttpPut("api/user/organization")]
        public IActionResult updateOrganization([FromBody] UpdateOrganizationRequest request)
        {
            Object result = repository.updateOrganization(request,GetUserId());
            if (result != null) return Ok(result);
            return BadRequest();
        }
        [Authorize]
        [HttpDelete("api/user/organization")]
        public IActionResult DeleteOrganization(int organizationID)
        {
            Object result = repository.DeleteOrganization(organizationID,GetUserId());
            if (result != null) return Ok(result);
            return BadRequest();
        }

        [Authorize]
        [HttpGet("api/user/organization/members")]
        public IActionResult getMemberOrganization(int organizationID)
        {
            return repository.getAllMemberOfOrganization(organizationID, GetUserId());
        }
    }
}