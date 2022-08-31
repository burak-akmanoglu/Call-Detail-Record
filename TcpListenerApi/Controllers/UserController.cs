using DataAccesLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TcpListenerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Admins")]
        // [Authorize(Roles = "Administrator")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AdminsEndpoint()
        {
            var currentUser = GetCurrentUser();
            using var c = new Context();
        //    return Ok($"Hi {currentUser.GivenName}, you are an {currentUser.Role}");
            return Ok(c.TblUser.ToList());
        }
        
        [HttpPost]
        public IActionResult UserAdd(User p)
        {
            using var c = new Context();
            c.Add(p);
            c.SaveChanges();
            return Created("", p);
        }
        [HttpGet("{id}")]
        public IActionResult UserGet(int id)
        {
            using var c = new Context();
            var value = c.TblUser.Find(id);

            if (value == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(value);
            }

        }
        [HttpDelete]
        public IActionResult UserDelete(int id)
        {
            using var c = new Context();
            var value = c.TblUser.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                c.Remove(value);
                c.SaveChanges();
                return NoContent();
            }
        }
        [HttpPut]
        public IActionResult UpdateUser(User parametre)
        {
            using var c = new Context();
            var value = c.Find<User>(parametre.UserModelId);

            if (value == null)
            {
                return NotFound();

            }
            else
            {
                value.Username = parametre.Username;
                value.Surname = parametre.Surname;
                value.Password = parametre.Password;
                value.Role = parametre.Role;
                value.GivenName = parametre.GivenName;
                value.EmailAddress = parametre.EmailAddress;
               
                c.Update(value);
                c.SaveChanges();
                return NoContent();
            }
        }
        /// <summary>
        /// ///////////////////////////////////
        /// </summary>
        /// <returns></returns>
        [HttpGet("Seller")]
        [Authorize(Roles = "Seller")]
        public IActionResult SellersEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi {currentUser.GivenName}, you are an {currentUser.Role}");
        }
        [HttpGet("AdminsandSeller")]
        [Authorize(Roles = "Seller,Administrator")]
        public IActionResult AdminsandSellerEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi {currentUser.GivenName}, you are an {currentUser.Role}");
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("Hi you are here");
        }

        private User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new User
                {
                    Username = userClaims.FirstOrDefault(ba => ba.Type == ClaimTypes.NameIdentifier)?.Value,
                    EmailAddress = userClaims.FirstOrDefault(ba => ba.Type == ClaimTypes.Email)?.Value,
                    GivenName = userClaims.FirstOrDefault(ba => ba.Type == ClaimTypes.GivenName)?.Value,
                    Surname = userClaims.FirstOrDefault(ba => ba.Type == ClaimTypes.Surname)?.Value,
                    Role = userClaims.FirstOrDefault(ba => ba.Type == ClaimTypes.Role)?.Value,
                };
            }
            return null;
        }
    }
}
