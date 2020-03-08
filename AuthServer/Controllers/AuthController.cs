using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Models;
using AuthServer.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    public class AuthController : ControllerBase
    {
        IWebOwnerRepository repository;
        [HttpPost("login")]
        public IActionResult Get([FromBody] LoginRequestModel model)
        {
            repository = new WebOwnerRepositoryImpl();
            var webOwner = repository.getCustomerByUsernameAndPassword(model);
            //just hard code here.  
            if (webOwner != null)
            {

                var now = DateTime.UtcNow;

                var claims = new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64),
                    new Claim(ClaimTypes.Role, webOwner.Role)
                };

                var signingKey = new SymmetricSecurityKey(Encoding.Default.GetBytes("SecretKeyForUserTrackingSystems"));
                var jwt = new JwtSecurityToken(
                    issuer: "Iss",
                    audience: "Aud",
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromDays(30)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                var customerresponse = new
                {
                    id = webOwner.WebOwnerId,
                    full_name = webOwner.FullName,
                    email = webOwner.Email,
                    role = webOwner.Role
                };
                var responseJson = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)TimeSpan.FromDays(30).TotalSeconds,
                    web_owner = customerresponse
            };

            return Ok(responseJson);
        }
            else
            {
                return StatusCode(401);
    }

}
    }
}