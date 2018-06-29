using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TestForSintezTech.Helpers;
using TestForSintezTech.Models;

namespace TestForSintezTech.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            db = context;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user.Login == null || user.Password == null)
                return BadRequest();

            if (db.Users.Any(x => x.Login == user.Login))
                return BadRequest();

            var helper = new PasswordHelper();
            user.Password = helper.HashPassword(user.Password);
            db.Users.Add(user);
            int result = await db.SaveChangesAsync();

            return Ok(JsonConvert.SerializeObject(result, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [HttpPost("/token")]
        public async Task<IActionResult> Token([FromBody]User user)
        {
            var identity = GetIdentity(user.Login, user.Password);
            if (identity == null)
            {
                Response.StatusCode = 400;
                return Content("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    //expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.GetLifeTime())),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //var response = new
            //{
            //    access_token = encodedJwt,
            //    username = identity.Name
            //};

            return Content(encodedJwt);
        }

        private ClaimsIdentity GetIdentity(string login, string password)
        {
            var helper = new PasswordHelper();
            User user = db.Users.SingleOrDefault(u => u.Login == login && helper.VerifyHashedPassword(u.Password, password));
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token");
                return claimsIdentity;
            }

            return null;
        }
    }
}