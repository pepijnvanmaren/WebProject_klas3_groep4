using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
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

        [HttpGet]
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

        [HttpGet("{Email}")]
        public ActionResult<KoperDB> GetKoper(string Email, string passwoord)
        {
            var Koper = _context.Koper.Find(Email, passwoord);
            if (Koper == null)
                return NotFound();
            return Ok(Koper);
        }

        [HttpPost]
        public ActionResult<KoperDB> PostKoper([FromBody] KoperDB Koper)
        {
            if (Koper == null)
                return BadRequest();

            _context.Koper.Add(Koper);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetKoper), new { id = Koper.ID }, Koper);
        }

        [HttpPut("{id}")]
        public ActionResult<KoperDB> PutKoper(int id, [FromBody] KoperDB updatedKoper)
        {
            var Koper = _context.Koper.Find(id);
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
            var Koper = _context.Koper.Find(id);

            if (Koper == null)
                return NotFound();

            _context.Koper.Remove(Koper);
            _context.SaveChanges();

            return NoContent();
        }
    }

}