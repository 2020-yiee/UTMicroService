using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [HttpPost("login")]
        public IActionResult Get([FromBody] LoginRequestModel model)
        {
            //just hard code here.  
            if (model.name == "catcher" && model.password == "123")
            {
                
                var now = DateTime.UtcNow;

                var claims = new Claim[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, model.name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            ,new Claim(ClaimTypes.Role, model.role == 1 ? "admin":"customer")
                };

                var signingKey = new SymmetricSecurityKey(Encoding.Default.GetBytes("SecretKeyForUserTrackingSystems"));
                var jwt = new JwtSecurityToken(
                    issuer: "Iss",
                    audience: "Aud",
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromDays(1)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                var responseJson = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)TimeSpan.FromDays(1).TotalSeconds
                };

                return Json(responseJson);
            }
            else
            {
                return Json("");
            }

        }
    }
}