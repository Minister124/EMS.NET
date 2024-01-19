using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repository.Contracts;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if (user == null)
            {
                return BadRequest("User Is Empty");
            }
            var register = await userAccount.CreateAsync(user);
            return Ok(register);
        }
        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if (user == null)
            {
                return BadRequest("User is Cannot be Null");
            }
            var login = await userAccount.SignInAsync(user);
            return Ok(login);
        }
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null) return BadRequest("Token is Null");
            var result = await userAccount.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }
    }
}
