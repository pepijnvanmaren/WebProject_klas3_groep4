using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("AllKopers")]
        public ActionResult<IEnumerable<KoperDB>> GetKopers()
        {
            return Ok(_context.Koper.ToList());
        }

        [HttpGet("Koper/{id}")]
        public async Task<ActionResult<KoperDto>> GetKoper(int id)
        {
            var Koper = await _context.Koper
                .FirstOrDefaultAsync(k => k.ID == id);

            if (Koper == null)
                {
                return NotFound();
            }

            var KoperDto = new KoperDto
            {
                Paswoord = Koper.Paswoord,
                Naam = Koper.Naam,
                Email = Koper.Email,
                Telefoonnummer = Koper.Telefoonnummer,
                BankGegevens = Koper.BankGegevens,
                Adres = Koper.Adres,
                Postcode = Koper.Postcode
            };
            return KoperDto;
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

        [HttpPut("{id}")]
        public ActionResult<KoperDB> PutKoperDto(int id, [FromBody] KoperDto dto)
        {
            if (dto == null)
                return BadRequest("Koper data is missing.");

            var Koper = _context.Koper.Find(id);

            if (Koper == null)
                return NotFound();

            Koper.Paswoord = dto.Paswoord;
            Koper.Email = dto.Email;
            Koper.Telefoonnummer = dto.Telefoonnummer;
            Koper.BankGegevens = dto.BankGegevens;
            Koper.Adres = dto.Adres;
            Koper.Postcode = dto.Postcode;
            _context.SaveChanges();

            return Ok(Koper);
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