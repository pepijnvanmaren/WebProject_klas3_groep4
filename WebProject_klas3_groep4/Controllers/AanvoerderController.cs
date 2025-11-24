using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public ActionResult<IEnumerable<AanvoerderDB>> GetAanvoerders()
        {
            return Ok(_context.Aanvoerder.ToList());
        }

        [HttpGet("{id:int}")]
        public ActionResult<AanvoerderDB> GetAanvoerder(int id)
        {
            var Aanvoerder = _context.Aanvoerder.Find(id);
            if (Aanvoerder == null)
                return NotFound();
            return Ok(Aanvoerder);
        }

        [HttpGet("{Email}")]
        public ActionResult<AanvoerderDB> GetAanvoerder(string Email, string passwoord)
        {
            var Aanvoerder = _context.Aanvoerder.Find(Email, passwoord);
            if (Aanvoerder == null)
                return NotFound();
            return Ok(Aanvoerder);
        }

        [HttpGet]
        public ActionResult<AanvoerderDB> GetAanvoerderProductenDto([FromQuery] AanvoerderDto dto)
        {
            if (dto == null)
                return BadRequest("Aanvoerder data is missing.");
            var Aanvoerder = new AanvoerderDB
            {
                prodcten = dto.prodcten
            };
            return Ok(Aanvoerder);
        }
        [HttpGet]
        public ActionResult<AanvoerderDB> GetAanvoerderDataDto([FromQuery] AanvoerderDto dto)
        {
            if (dto == null)
                return BadRequest("Aanvoerder data is missing.");
            var Aanvoerder = new AanvoerderDB
            {
                Naam = dto.Naam,
                Email = dto.Email,
                Telefoonnummer = dto.Telefoonnummer
            };
            return Ok(Aanvoerder);
        }

        [HttpGet]
        public ActionResult<AanvoerderDB> GetAanvoerderDataBedrijfDto([FromQuery] AanvoerderDto dto)
        {
            if (dto == null)
                return BadRequest("Aanvoerder data is missing.");
            var Aanvoerder = new AanvoerderDB
            {
                KvkNummer = dto.KvkNummer,
                NaamVanBedrijf = dto.NaamVanBedrijf,
                BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer,
                BedrijfEmail = dto.BedrijfEmail,
                Postcode = dto.Postcode,
                Adres = dto.Adres
            };
            return Ok(Aanvoerder);
        }

        [HttpPost]
        public ActionResult<AanvoerderDB> PostAanvoerder([FromBody] AanvoerderDto dto)
        {
            if (dto == null)
                return BadRequest("Aanvoerder data is missing.");

            var Aanvoerder = new AanvoerderDB
            {

                Naam = dto.Naam,
                Email = dto.Email,
                Telefoonnummer = dto.Telefoonnummer,
                KvkNummer = dto.KvkNummer,
                NaamVanBedrijf = dto.NaamVanBedrijf,
                Postcode = dto.Postcode,
                Adres = dto.Adres,
                BedrijfTelefoonnummer = dto.BedrijfTelefoonnummer,
                BedrijfEmail = dto.BedrijfEmail,
                prodcten = dto.prodcten
            };

            _context.Aanvoerder.Add(Aanvoerder);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAanvoerder), new { id = Aanvoerder.ID }, Aanvoerder);
        }

        [HttpPut("{id}")]
        public ActionResult<AanvoerderDB> PutAanvoerder(int id, [FromBody] AanvoerderDto dto)
        {
            var Aanvoerder = _context.Aanvoerder.Find(id);
            if (dto == null)
                return BadRequest("Aanvoerder data is missing.");
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
            Aanvoerder.prodcten = dto.prodcten;

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