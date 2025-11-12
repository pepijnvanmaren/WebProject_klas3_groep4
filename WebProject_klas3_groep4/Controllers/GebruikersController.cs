using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class GebruikersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public GebruikersController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GebruikerDB>> GetGebruikers()
        {
            return Ok(_context.Gebruikers.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<GebruikerDB> GetGebruiker(int id)
        {
            var gebruiker = _context.Gebruikers.Find(id);
            if (gebruiker == null)
                return NotFound();
            return Ok(gebruiker);
        }

        [HttpPost]
        public ActionResult<GebruikerDB> PostGebruiker([FromBody] GebruikerDB gebruiker)
        {
            if (gebruiker == null)
                return BadRequest();

            _context.Gebruikers.Add(gebruiker);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetGebruiker), new { id = gebruiker.ID }, gebruiker);
        }

        [HttpPut("{id}")]
        public ActionResult<GebruikerDB> PutGebruiker(int id, [FromBody] GebruikerDB updatedGebruiker)
        {
            var gebruiker = _context.Gebruikers.Find(id);
            if (gebruiker == null)
                return NotFound();

            gebruiker.Naam = updatedGebruiker.Naam;
            gebruiker.Email = updatedGebruiker.Email;
            gebruiker.Telefoonnummer = updatedGebruiker.Telefoonnummer;
            gebruiker.Paswoord = updatedGebruiker.Paswoord;

            _context.SaveChanges();

            return Ok(gebruiker);
        }

        [HttpDelete("{id}")]
        public ActionResult<GebruikerDB> DeleteGebruiker(int id)
        {
            var gebruiker = _context.Gebruikers.Find(id);
          
            if (gebruiker == null)
                return NotFound();

            _context.Gebruikers.Remove(gebruiker);
            _context.SaveChanges();

            return NoContent();
        }
    }
    
}