
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly AppDbContext userContext;
        readonly ITokenService tokenService;
        IPasswordHasher<IdentityUser> passwordHasher;

        public AuthController(AppDbContext userContext, ITokenService tokenService,IPasswordHasher<IdentityUser> passwordHasher)
        {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.passwordHasher = passwordHasher;
        }

        [HttpPost, Route("login")]
        public IActionResult Login(User userFromClient)
        {
            Console.WriteLine(userFromClient);
            if (userFromClient == null)
            {
                return BadRequest("Invalid client request");
            }

            var user = userContext.Users
                .FirstOrDefault(u => (u.Login == userFromClient.Login) );

            if (user == null  || !(passwordHasher.VerifyHashedPassword(new IdentityUser(),user.Password,userFromClient.Password) == PasswordVerificationResult.Success))
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userFromClient.Login),
                new Claim(ClaimTypes.Role, "Manager")
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();
            user.LoginModel = new LoginModel();
            user.LoginModel.RefreshToken = refreshToken;
            user.LoginModel.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            userContext.SaveChanges();

            return Ok(new
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}