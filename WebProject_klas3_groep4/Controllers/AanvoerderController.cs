using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AanvoerderController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public AanvoerderController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GebruikerDB>> GetAanvoerders()
        {
            var aanvoerders = _context.Gebruikers
                .Where(u => u.Rol == "Aanvoerder")
                .Include(u => u.Producten)
                .ToList();
            return Ok(aanvoerders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GebruikerDB>> GetAanvoerder(int id)
        {
            var aanvoerder = await _context.Gebruikers
                .Include(u => u.Producten)
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Aanvoerder");

            if (aanvoerder == null)
                return NotFound();

            return Ok(aanvoerder);
        }

        [HttpGet("login")]
        public async Task<ActionResult<GebruikerDB>> Login([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.Rol != "Aanvoerder")
                return NotFound();

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid)
                return Unauthorized();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<GebruikerDB>> PostAanvoerder([FromBody] AanvoerderCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var aanvoerder = new GebruikerDB
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Rol = "Aanvoerder",
                KvkNummer = dto.KvkNummer,
                NaamVanBedrijf = dto.NaamVanBedrijf,
                Postcode = dto.Postcode,
                Adres = dto.Adres,
                BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer,
                BedrijfEmail = dto.BedrijfEmail
            };

            var result = await _userManager.CreateAsync(aanvoerder, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetAanvoerder), new { id = aanvoerder.Id }, aanvoerder);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GebruikerDB>> PutAanvoerder(int id, [FromBody] AanvoerderUpdateDto dto)
        {
            var aanvoerder = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Aanvoerder");

            if (aanvoerder == null)
                return NotFound();

            aanvoerder.UserName = dto.UserName;
            aanvoerder.Email = dto.Email;
            aanvoerder.PhoneNumber = dto.PhoneNumber;
            aanvoerder.KvkNummer = dto.KvkNummer;
            aanvoerder.NaamVanBedrijf = dto.NaamVanBedrijf;
            aanvoerder.Postcode = dto.Postcode;
            aanvoerder.Adres = dto.Adres;
            aanvoerder.BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer;
            aanvoerder.BedrijfEmail = dto.BedrijfEmail;

            await _userManager.UpdateAsync(aanvoerder);
            return Ok(aanvoerder);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAanvoerder(int id)
        {
            var aanvoerder = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Aanvoerder");

            if (aanvoerder == null)
                return NotFound();

            await _userManager.DeleteAsync(aanvoerder);
            return NoContent();
        }
    }

    public class AanvoerderCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? KvkNummer { get; set; }
        public string? NaamVanBedrijf { get; set; }
        public string? Postcode { get; set; }
        public string? Adres { get; set; }
        public string? BedrijfTelefoonnummer { get; set; }
        public string? BedrijfEmail { get; set; }
    }

    public class AanvoerderUpdateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? KvkNummer { get; set; }
        public string? NaamVanBedrijf { get; set; }
        public string? Postcode { get; set; }
        public string? Adres { get; set; }
        public string? BedrijfTelefoonnummer { get; set; }
        public string? BedrijfEmail { get; set; }
    }
}