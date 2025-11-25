using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AanvoerderController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public AanvoerderController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("AllPersoonsData")]
        public ActionResult<IEnumerable<AanvoerderDB>> GetAanvoerders()
        {
            return Ok(_context.Aanvoerder.ToList());
        }
        [HttpGet("data/{ID}")]
        public async Task<ActionResult<AanvoerderDataDto>> GetAanvoerderDataDto(int ID)
        {
            var aanvoerder = await _context.Aanvoerder
            .FirstOrDefaultAsync(a => a.ID == ID);

            if (aanvoerder == null)
            {
                return NotFound();
            }

            var AanvoerderDataDto = new AanvoerderDataDto
            {
                Naam = aanvoerder.Naam,
                Email = aanvoerder.Email,
                Telefoonnummer = aanvoerder.Telefoonnummer
            };

            return AanvoerderDataDto;

        }
        [HttpGet("Aanvoerder/{ID}")]
        public async Task<ActionResult<AanvoerderDto>> GetAanvoerderInfo(int ID)
        {
            var aanvoerder = await _context.Aanvoerder
                .FirstOrDefaultAsync(a => a.ID == ID);

            if (aanvoerder == null)
            {
                return NotFound();
            }

            var aanvoerderDto = new AanvoerderDto
            {
                Paswoord = aanvoerder.Paswoord,
                Naam = aanvoerder.Naam,
                Email = aanvoerder.Email,
                Telefoonnummer = aanvoerder.Telefoonnummer,
                KvkNummer = aanvoerder.KvkNummer,
                NaamVanBedrijf = aanvoerder.NaamVanBedrijf,
                Postcode = aanvoerder.Postcode,
                Adres = aanvoerder.Adres,
                BedrijfTelefoonnummer = aanvoerder.BedrijfTelefoonnummer,
                BedrijfEmail = aanvoerder.BedrijfEmail,
                Rol = aanvoerder.Rol
            };
            return aanvoerderDto;

        }

        [HttpPost]
        public ActionResult<AanvoerderDB> PostAanvoerder([FromBody] AanvoerderDto dto)
        {
            if (dto == null)
                return BadRequest("Aanvoerder data is missing.");

            var Aanvoerder = new AanvoerderDB
            {
                Paswoord = dto.Paswoord,
                Naam = dto.Naam,
                Email = dto.Email,
                Telefoonnummer = dto.Telefoonnummer,
                KvkNummer = dto.KvkNummer,
                NaamVanBedrijf = dto.NaamVanBedrijf,
                Postcode = dto.Postcode,
                Adres = dto.Adres,
                BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer,
                BedrijfEmail = dto.BedrijfEmail,
                Rol = dto.Rol
            };

            _context.Aanvoerder.Add(Aanvoerder);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAanvoerderInfo), new { id = Aanvoerder.ID }, Aanvoerder);
        }

        [HttpPut("{id}")]
        public ActionResult<AanvoerderDB> PutAanvoerder(int id, [FromBody] AanvoerderDto dto)
        {
            if (dto == null)
                return BadRequest("Aanvoerder data is missing.");

            var Aanvoerder = _context.Aanvoerder.Find(id);

            if (Aanvoerder == null)
                return NotFound();

            Aanvoerder.Naam = dto.Naam;
            Aanvoerder.Email = dto.Email;
            Aanvoerder.Telefoonnummer = dto.Telefoonnummer;
            Aanvoerder.KvkNummer = dto.KvkNummer;
            Aanvoerder.NaamVanBedrijf = dto.NaamVanBedrijf;
            Aanvoerder.Postcode = dto.Postcode;
            Aanvoerder.Adres = dto.Adres;
            Aanvoerder.BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer;
            Aanvoerder.BedrijfEmail = dto.BedrijfEmail;
            _context.SaveChanges();

            return Ok(Aanvoerder);
        }

        [HttpDelete("{id}")]
        public ActionResult<AanvoerderDB> DeleteAanvoerder(int id)
        {
            var Aanvoerder = _context.Aanvoerder.Find(id);

            if (Aanvoerder == null)
                return NotFound();

            _context.Aanvoerder.Remove(Aanvoerder);
            _context.SaveChanges();

            return NoContent();
        }
    }

}