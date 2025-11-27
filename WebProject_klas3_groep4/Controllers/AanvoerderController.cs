using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Authorize(Roles ="Aanvoerder")]
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
        public async Task<ActionResult<IEnumerable<AanvoerderOutputDto>>> GetAanvoerders()
        {
            var aanvoerders = await _context.Gebruikers
                .Where(u => u.Rol == "Aanvoerder")
                .Select(u => new AanvoerderOutputDto
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Password = "", // GEEN wachtwoord teruggeven
                    NaamVanBedrijf = u.NaamVanBedrijf,
                    KvkNummer = u.KvkNummer,
                    Adres = u.Adres,
                    Postcode = u.Postcode,
                    BedrijfTelefoonnummer = u.BedrijfTelefoonnummer,
                    BedrijfEmail = u.BedrijfEmail
                })
                .ToListAsync();

            return Ok(aanvoerders);
        }

        // ------------------------------------------------------------
        // GET SINGLE
        // ------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AanvoerderOutputDto>> GetAanvoerder(int id)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "Aanvoerder")
                return NotFound();

            var dto = new AanvoerderOutputDto
            {
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Password = "",
                NaamVanBedrijf = aanvoerder.NaamVanBedrijf,
                KvkNummer = aanvoerder.KvkNummer,
                Adres = aanvoerder.Adres,
                Postcode = aanvoerder.Postcode,
                BedrijfTelefoonnummer = aanvoerder.BedrijfTelefoonnummer,
                BedrijfEmail = aanvoerder.BedrijfEmail
            };

            return Ok(dto);
        }

        // ------------------------------------------------------------
        // CREATE

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<AanvoerderOutputDto>> PostAanvoerder([FromBody] AanvoerderCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

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

            var outDto = new GebruikerDto
            {
                Id = aanvoerder.Id,
                UserName = aanvoerder.UserName, 
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Rol = aanvoerder.Rol
            };

            return CreatedAtAction(nameof(GetAanvoerder), new { id = aanvoerder.Id }, outDto);
        }

        // ------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AanvoerderOutputDto>> PutAanvoerder(int id, [FromBody] AanvoerderUpdateDto dto)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "Aanvoerder")
                return NotFound();

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
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Password = "",
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
        // DELETE
        // ------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAanvoerder(int id)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "Aanvoerder")
                return NotFound();

            await _userManager.DeleteAsync(aanvoerder);

            return NoContent();
        }
    }
}
