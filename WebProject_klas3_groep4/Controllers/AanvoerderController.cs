using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/aanvoerders")]
    [Authorize] // Verwijder AuthenticationSchemes = "Bearer"
    public class AanvoerderController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public AanvoerderController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AanvoerderDto>> Login([FromBody] LoginDto dto)
        {
            if (dto == null)
                return BadRequest("Login data is leeg.");

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || user.Rol != "Aanvoerder")
                return NotFound("Gebruiker niet gevonden of verkeerde rol.");

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid)
                return Unauthorized("Ongeldig wachtwoord.");

            await _context.Entry(user).Collection(u => u.Producten).LoadAsync();

            // Login via Identity cookies
            var signInManager = HttpContext.RequestServices.GetRequiredService<SignInManager<GebruikerDB>>();
            await signInManager.SignInAsync(user, isPersistent: true);

            var result = new AanvoerderDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                KvkNummer = user.KvkNummer,
                NaamVanBedrijf = user.NaamVanBedrijf,
                Postcode = user.Postcode,
                Adres = user.Adres,
                BedrijfTelefoonnummer = user.BedrijfTelefoonnummer,
                BedrijfEmail = user.BedrijfEmail,
                ProductCount = user.Producten.Count
            };

            return Ok(result);
        }


        // Overige endpoints (GET, POST, PUT, DELETE) blijven hetzelfde, [Authorize] gebruiken
    }
}
