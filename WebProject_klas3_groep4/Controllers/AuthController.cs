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
        [Authorize]
        [HttpPut("update-username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameRequest dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var existingUser = await _userManager.FindByNameAsync(dto.NewUsername);
            if (existingUser != null && existingUser.Id != user.Id)
                return BadRequest("Deze gebruikersnaam is al in gebruik");

            user.UserName = dto.NewUsername;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Ok(new { message = "Gebruikersnaam succesvol gewijzigd" });
        }

        // ------------------------ UPDATE EMAIL ------------------------
        [Authorize]
        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var existingUser = await _userManager.FindByEmailAsync(dto.NewEmail);
            if (existingUser != null && existingUser.Id != user.Id)
                return BadRequest("Dit e-mailadres is al in gebruik");

            user.Email = dto.NewEmail;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Ok(new { message = "E-mail succesvol gewijzigd" });
        }

        // ------------------------ UPDATE PASSWORD ------------------------
        [Authorize]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Ok(new { message = "Wachtwoord succesvol gewijzigd" });
        }

        // ------------------------ DELETE ACCOUNT ------------------------
        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return BadRequest("Onjuist wachtwoord");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Ok(new { message = "Account succesvol verwijderd" });
        }
    }

    // ------------------------ REQUEST MODELS ------------------------
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUsernameRequest
    {
        public string NewUsername { get; set; }
    }

    public class UpdateEmailRequest
    {
        public string NewEmail { get; set; }
    }

    public class UpdatePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class DeleteAccountRequest
    {
        public string Password { get; set; }
    }
}
    

