using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        readonly AppDbContext userContext;
        readonly ITokenService tokenService;

        public TokenController(AppDbContext userContext, ITokenService tokenService)
        {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            if(accessToken != null){
                var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
                var username = principal.Identity.Name; //this is mapped to the Name claim by default
                Console.WriteLine(username);
                var user = userContext.Users.SingleOrDefault(u => u.Login == username);

                if (user == null )
                {
                    return BadRequest("Invalid client request");
                }

                var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                user.LoginModel = new LoginModel();
                user.LoginModel.RefreshToken = newRefreshToken;
                userContext.SaveChanges();
                return new ObjectResult(new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                });
            }
            else{
                return BadRequest("Invalid client request");
            }
            
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;

            var user = userContext.Users.SingleOrDefault(u => u.Login == username);
            if (user == null) return BadRequest();

            user.LoginModel.RefreshToken = null;

            userContext.SaveChanges();

            return NoContent();
        }
    }
}