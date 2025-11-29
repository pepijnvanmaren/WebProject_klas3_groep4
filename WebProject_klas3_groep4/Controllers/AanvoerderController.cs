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
        // GET ALL - Iedereen mag dit zien
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AanvoerderOutputDto>>> GetAanvoerders()
        {
            var aanvoerders = await _context.Gebruikers
                .Where(u => u.Rol == "Aanvoerder")
                .Include(u => u.Producten)
                .Select(u => new AanvoerderOutputDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Rol = "Aanvoerder",
                    NaamVanBedrijf = u.NaamVanBedrijf,
                    KvkNummer = u.KvkNummer,
                    Adres = u.Adres,
                    Postcode = u.Postcode,
                    BedrijfTelefoonnummer = u.BedrijfTelefoonnummer,
                    BedrijfEmail = u.BedrijfEmail,
                    ProductIds = u.Producten != null ? u.Producten.Select(p => p.ID).ToList() : new List<int>()
                })
                .ToListAsync();

            return Ok(aanvoerders);
        }

        // ------------------------------------------------------------
        // GET SINGLE - Iedereen mag dit zien
        // ------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AanvoerderOutputDto>> GetAanvoerder(int id)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "Aanvoerder")
                return NotFound("Aanvoerder niet gevonden");

            var producten = await _context.Producten
                .Where(p => p.AanvoerderId == id)
                .Select(p => p.ID)
                .ToListAsync();

            var dto = new AanvoerderOutputDto
            {
                Id = aanvoerder.Id,
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Rol = "Aanvoerder",
                NaamVanBedrijf = aanvoerder.NaamVanBedrijf,
                KvkNummer = aanvoerder.KvkNummer,
                Adres = aanvoerder.Adres,
                Postcode = aanvoerder.Postcode,
                BedrijfTelefoonnummer = aanvoerder.BedrijfTelefoonnummer,
                BedrijfEmail = aanvoerder.BedrijfEmail,
                ProductIds = producten
            };

            return Ok(dto);
        }

        // ------------------------------------------------------------
        // CREATE - Iedereen mag registreren
        // ------------------------------------------------------------
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<AanvoerderOutputDto>> PostAanvoerder([FromBody] AanvoerderCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            var aanvoerder = new GebruikerDB
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Rol = "Aanvoerder",
                NaamVanBedrijf = dto.NaamVanBedrijf,
                KvkNummer = dto.KvkNummer,
                Adres = dto.Adres,
                Postcode = dto.Postcode,
                BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer,
                BedrijfEmail = dto.BedrijfEmail
            };

            var result = await _userManager.CreateAsync(aanvoerder, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var outDto = new AanvoerderOutputDto
            {
                Id = aanvoerder.Id,
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Rol = "Aanvoerder",
                NaamVanBedrijf = aanvoerder.NaamVanBedrijf,
                KvkNummer = aanvoerder.KvkNummer,
                Adres = aanvoerder.Adres,
                Postcode = aanvoerder.Postcode,
                BedrijfTelefoonnummer = aanvoerder.BedrijfTelefoonnummer,
                BedrijfEmail = aanvoerder.BedrijfEmail
            };

            return CreatedAtAction(nameof(GetAanvoerder), new { id = aanvoerder.Id }, outDto);
        }

        // ------------------------------------------------------------
        // UPDATE - Alleen jezelf mag je eigen gegevens aanpassen
        // ------------------------------------------------------------
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AanvoerderOutputDto>> PutAanvoerder(int id, [FromBody] AanvoerderUpdateDto dto)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "Aanvoerder")
                return NotFound("Aanvoerder niet gevonden");

            var user = await _userManager.GetUserAsync(User);
            if (user?.Id != id)
                return Forbid("Je kunt alleen je eigen gegevens aanpassen");

            aanvoerder.UserName = dto.UserName;
            aanvoerder.Email = dto.Email;
            aanvoerder.PhoneNumber = dto.PhoneNumber;
            aanvoerder.NaamVanBedrijf = dto.NaamVanBedrijf;
            aanvoerder.KvkNummer = dto.KvkNummer;
            aanvoerder.Adres = dto.Adres;
            aanvoerder.Postcode = dto.Postcode;
            aanvoerder.BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer;
            aanvoerder.BedrijfEmail = dto.BedrijfEmail;

            var result = await _userManager.UpdateAsync(aanvoerder);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var outDto = new AanvoerderOutputDto
            {
                Id = aanvoerder.Id,
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Rol = "Aanvoerder",
                NaamVanBedrijf = aanvoerder.NaamVanBedrijf,
                KvkNummer = aanvoerder.KvkNummer,
                Adres = aanvoerder.Adres,
                Postcode = aanvoerder.Postcode,
                BedrijfTelefoonnummer = aanvoerder.BedrijfTelefoonnummer,
                BedrijfEmail = aanvoerder.BedrijfEmail
            };

            return Ok(outDto);
        }

        // ------------------------------------------------------------
        // DELETE - Alleen admin mag verwijderen
        // ------------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAanvoerder(int id)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "Aanvoerder")
                return NotFound("Aanvoerder niet gevonden");

            await _userManager.DeleteAsync(aanvoerder);

            return NoContent();
        }
    }
}