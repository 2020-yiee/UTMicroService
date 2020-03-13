using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthServer.EFModels;
using AuthServer.Models;
using AuthServer.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Controllers
{
    [Route("api/auth")]
    [EnableCors]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUserRepository _repository;
        public AuthController(IAuthUserRepository repository)
        {
            _repository = repository;
        }
        
        [HttpPost("login")]
        public async Task<object> Get([FromBody] LoginRequestModel model)
        {
            var webOwner = await _repository.getCustomerByUsernameAndPassword(model);
            if (Object.Equals(webOwner,false)) return StatusCode(401);
            if (webOwner == null) return StatusCode(403);
            return Ok(webOwner);

        }
    }
}