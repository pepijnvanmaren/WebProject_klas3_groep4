using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<GebruikerDB> _signInManager;
        private readonly UserManager<GebruikerDB> _userManager;

        public AuthController(SignInManager<GebruikerDB> signInManager, UserManager<GebruikerDB> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // -------------------- LOGIN --------------------
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Wrong email or password.");

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, true, false);

            if (!result.Succeeded)
                return Unauthorized("Wrong email or password.");

            return Ok(new { message = "Logged in successfully." });
        }

        // -------------------- LOGOUT --------------------
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully." });
        }

        // -------------------- GET CURRENT USER --------------------
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Rol,
                user.VeilingVestiging
            });
        }
    }
}
