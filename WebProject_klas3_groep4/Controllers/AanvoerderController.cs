using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/aanvoerders")]
    public class AanvoerderController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public AanvoerderController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AanvoerderDto>>> GetAanvoerders()
        {
            var aanvoerders = await _context.Gebruikers
                .Where(u => u.Rol == "Aanvoerder")
                .Include(u => u.Producten)
                .Select(a => new AanvoerderDto
                {
                    Id = a.Id,
                    UserName = a.UserName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    KvkNummer = a.KvkNummer,
                    NaamVanBedrijf = a.NaamVanBedrijf,
                    Postcode = a.Postcode,
                    Adres = a.Adres,
                    BedrijfTelefoonnummer = a.BedrijfTelefoonnummer,
                    BedrijfEmail = a.BedrijfEmail,
                    ProductCount = a.Producten.Count
                })
                .ToListAsync();

            return Ok(aanvoerders);
        }

        // ------------------------------------------------------------
        // GET SINGLE
        // ------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AanvoerderDto>> GetAanvoerder(int id)
        {
            var a = await _context.Gebruikers
                .Include(u => u.Producten)
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Aanvoerder");

            if (a == null)
                return NotFound();

            var dto = new AanvoerderDto
            {
                Id = a.Id,
                UserName = a.UserName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                KvkNummer = a.KvkNummer,
                NaamVanBedrijf = a.NaamVanBedrijf,
                Postcode = a.Postcode,
                Adres = a.Adres,
                BedrijfTelefoonnummer = a.BedrijfTelefoonnummer,
                BedrijfEmail = a.BedrijfEmail,
                ProductCount = a.Producten.Count
            };

            return Ok(dto);
        }

        // ------------------------------------------------------------
        // LOGIN
        // ------------------------------------------------------------
        [HttpGet("login")]
        public async Task<ActionResult<AanvoerderDto>> Login([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.Rol != "Aanvoerder")
                return NotFound();

            var valid = await _userManager.CheckPasswordAsync(user, password);

            if (!valid)
                return Unauthorized();

            await _context.Entry(user).Collection(u => u.Producten).LoadAsync();

            var dto = new AanvoerderDto
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

            return Ok(dto);
        }

        // ------------------------------------------------------------
        // CREATE
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<AanvoerderDto>> PostAanvoerder([FromBody] AanvoerderCreateDto dto)
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

            var returnDto = new AanvoerderDto
            {
                Id = aanvoerder.Id,
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                KvkNummer = aanvoerder.KvkNummer,
                NaamVanBedrijf = aanvoerder.NaamVanBedrijf,
                Postcode = aanvoerder.Postcode,
                Adres = aanvoerder.Adres,
                BedrijfTelefoonnummer = aanvoerder.BedrijfTelefoonnummer,
                BedrijfEmail = aanvoerder.BedrijfEmail,
                ProductCount = 0
            };

            return CreatedAtAction(nameof(GetAanvoerder), new { id = aanvoerder.Id }, returnDto);
        }

        // ------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<AanvoerderDto>> PutAanvoerder(int id, [FromBody] AanvoerderUpdateDto dto)
        {
            var a = await _context.Gebruikers
                .Include(u => u.Producten)
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Aanvoerder");

            if (a == null)
                return NotFound();

            a.UserName = dto.UserName;
            a.Email = dto.Email;
            a.PhoneNumber = dto.PhoneNumber;
            a.KvkNummer = dto.KvkNummer;
            a.NaamVanBedrijf = dto.NaamVanBedrijf;
            a.Postcode = dto.Postcode;
            a.Adres = dto.Adres;
            a.BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer;
            a.BedrijfEmail = dto.BedrijfEmail;

            await _userManager.UpdateAsync(a);

            var dtoReturn = new AanvoerderDto
            {
                Id = a.Id,
                UserName = a.UserName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                KvkNummer = a.KvkNummer,
                NaamVanBedrijf = a.NaamVanBedrijf,
                Postcode = a.Postcode,
                Adres = a.Adres,
                BedrijfTelefoonnummer = a.BedrijfTelefoonnummer,
                BedrijfEmail = a.BedrijfEmail,
                ProductCount = a.Producten.Count
            };

            return Ok(dtoReturn);
        }

        // ------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAanvoerder(int id)
        {
            var a = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Aanvoerder");

            if (a == null)
                return NotFound();

            await _userManager.DeleteAsync(a);

            return NoContent();
        }
    }
}
