using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

namespace WebProject_klas3_groep4.Controllers
{
    //Auth Controller in Swagger
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        //UserManager<GebruikerDB> wordt gebruikt voor gebruikers vinden/wachtwoorden/roles
        private readonly UserManager<GebruikerDB> _userManager;
        //IConfiguration leest settings van appsettings.json
        private readonly IConfiguration _config;

        public AuthController(UserManager<GebruikerDB> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        //login endpoint
        [AllowAnonymous] //Iedereen mag inloggen
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);  //zoekt email
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password)) //check wachtwoord
                return Unauthorized("Wrong email or password.");    //401

            var claims = new[]  //alle data in de token
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); //Pakt de secretkey van appsettings.json
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //Signed de token met HMAC SHA256?

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],  //sets Issuer
                claims: claims, //add claims
                expires: DateTime.UtcNow.AddHours(3), //3 uur expirement
                signingCredentials: creds //sign met HMAC SHA256
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),    //token = JWT string
                rol = user.Rol //Slaat Rol op in Rol voor Frontend
            });
        }

        //Me Endpoint, gebruikt de JWT token.
        [Authorize] //JWT token nodig om te gebruiken
        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  //Slaat ID op van User

            if (string.IsNullOrEmpty(userId))   //Checkt of je authorized bent, anders return
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)   //checkt of de user nog bestaat
                return Unauthorized();

            return Ok(new   //return de info
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

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
