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
        public IActionResult Get([FromBody] LoginRequestModel model)
        {
            return _repository.getUserByEmailAndPassword(model);
            
        }

        [HttpPost("login/admin")]
        public IActionResult loginAdmin([FromBody] LoginRequestModel model)
        {
            return _repository.getAdminByEmailAndPassword(model);
            

        }

        [HttpGet("forgot-password")]
        public IActionResult resend([FromQuery] string mail)
        {
            return _repository.processForgotPassword(mail);
        }
    }
}