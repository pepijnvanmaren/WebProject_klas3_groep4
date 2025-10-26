using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
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
        public ActionResult<IEnumerable<KoperDB>> GetKopers()
        {
            return Ok(_context.Kopers.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<KoperDB> GetKoper(int id)
        {
            var Koper = _context.Kopers.Find(id);
            if (Koper == null)
                return NotFound();
            return Ok(Koper);
        }

        [HttpGet("{Naam}")]
        public ActionResult<KoperDB> GetKoper(string Naam)
        {
            var Koper = _context.Kopers.Find(Naam);
            if (Koper == null)
                return NotFound();
            return Ok(Koper);
        }

        [HttpPost]
        public ActionResult<KoperDB> PostKoper([FromBody] KoperDB Koper)
        {
            if (Koper == null)
                return BadRequest();

            _context.Kopers.Add(Koper);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetKoper), new { id = Koper.ID }, Koper);
        }

        [HttpPut("{id}")]
        public ActionResult<KoperDB> PutKoper(int id, [FromBody] KoperDB updatedKoper)
        {
            var Koper = _context.Kopers.Find(id);
            if (Koper == null)
                return NotFound();

            Koper.Naam = updatedKoper.Naam;
            Koper.Email = updatedKoper.Email;
            Koper.Telefoonnummer = updatedKoper.Telefoonnummer;
            Koper.BankGegevens = updatedKoper.BankGegevens;
            Koper.Adres = updatedKoper.Adres;
            Koper.Postcode = updatedKoper.Postcode;
            Koper.Lists = updatedKoper.Lists;

            _context.SaveChanges();

            return Ok(Koper);
        }

        [HttpDelete("{id}")]
        public ActionResult<KoperDB> DeleteKoper(int id)
        {
            var Koper = _context.Kopers.Find(id);

            if (Koper == null)
                return NotFound();

            _context.Kopers.Remove(Koper);
            _context.SaveChanges();

            return NoContent();
        }
    }

}