using DataAccesLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TcpListenerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);

            if (user != null)
            {
                string token = Generate(user);
                //HttpContext.Session.SetString("JwToken",token);
               
                return Ok(token);
            }
            return NotFound("User not found");
        }
      
        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Email,user.EmailAddress),
                new Claim(ClaimTypes.GivenName,user.GivenName),
                new Claim(ClaimTypes.Surname,user.Surname),
                new Claim(ClaimTypes.Role,user.Role),
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddSeconds(20),
                signingCredentials: credentials);
          
            //var useridentity = new ClaimsIdentity(claims, "Bearer");
            //ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
            // HttpContext.SignInAsync(principal);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(UserLogin userLogin)
        {
            using var c = new Context();
            var currentUser = c.TblUser.FirstOrDefault(ba => ba.Username.ToLower() == userLogin.Username.ToLower()
            && ba.Password == userLogin.Password);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }
}
