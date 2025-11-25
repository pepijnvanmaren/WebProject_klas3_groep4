using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KoperController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public KoperController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<KoperDB>> GetKopers()
        {
            return Ok(_context.Koper.ToList());
        }

        [HttpGet("{id:int}")]
        public ActionResult<KoperDB> GetKoper(int id)
        {
            var Koper = _context.Koper.Find(id);
            if (Koper == null)
                return NotFound();
            return Ok(Koper);
        }

        [HttpGet("gebruiker")]
        public ActionResult<KoperDB> GetKoperDto([FromQuery] KoperDto dto)
        {
            if (dto == null)
                return BadRequest("Gebruiker data is missing.");

            var Koper = new KoperDB
            {
                Naam = dto.Naam,
                Paswoord = dto.Paswoord
            };
            return Ok(Koper);
        }

        [HttpPost("KoperAanmaken")]
        public ActionResult<KoperDB> PostVolledigKoperDto([FromBody] KoperDto dto)
        {
            if (dto == null)
                return BadRequest("Koper data is missing.");
            var Koper = new KoperDB
            {
                Naam = dto.Naam,
                Paswoord = dto.Paswoord,
                Email = dto.Email,
                Telefoonnummer = dto.Telefoonnummer,
                BankGegevens = dto.BankGegevens,
                Adres = dto.Adres,
                Postcode = dto.Postcode
            };
            _context.Koper.Add(Koper);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetKoper), new { id = Koper.ID }, Koper);
        }

        [HttpPost("KoperAanmakenBasis")]
        public ActionResult<KoperDB> PostKoperDto([FromBody] KoperDto dto)
        {
            if (dto == null)
                return BadRequest("Koper data is missing.");
            var Koper = new KoperDB
            {
                Naam = dto.Naam,
                Paswoord = dto.Paswoord,
                Email = dto.Email
            };
            _context.Koper.Add(Koper);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetKoper), new { id = Koper.ID }, Koper);
        }

        [HttpPut]
        public ActionResult<KoperDB> PutKoperDto([FromBody] KoperDto dto)
        {
            var existingKoper = _context.Koper.FirstOrDefault(k => k.Naam == dto.Naam);
            if (existingKoper == null)
                return NotFound();
            existingKoper.Paswoord = dto.Paswoord;
            existingKoper.Email = dto.Email;
            existingKoper.Telefoonnummer = dto.Telefoonnummer;
            existingKoper.BankGegevens = dto.BankGegevens;
            existingKoper.Adres = dto.Adres;
            existingKoper.Postcode = dto.Postcode;
            _context.Koper.Update(existingKoper);
            _context.SaveChanges();
            return Ok(existingKoper);
        }

        [HttpDelete("{id}")]
        public ActionResult<KoperDB> DeleteKoper(int id)
        {
            var Koper = _context.Koper.Find(id);

            if (Koper == null)
                return NotFound();

            _context.Koper.Remove(Koper);
            _context.SaveChanges();

            return NoContent();
        }
    }

}