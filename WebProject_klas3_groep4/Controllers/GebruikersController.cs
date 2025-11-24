using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.DTO;
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

        [HttpGet]
        public ActionResult<GebruikerDB> GetGebruikerDto([FromQuery] GebruikerDto dto)
        {
            if (dto == null)
                return BadRequest("Gebruiker data is missing.");

            var gebruiker = new GebruikerDB
            {
                Naam = dto.Naam,
                Paswoord = dto.Paswoord
            };
            return Ok(gebruiker);
        }

        [HttpPost]
        public ActionResult<GebruikerDB> PostGebruiker([FromBody] GebruikerDto dto)
        {
            if (dto == null)
                return BadRequest("Gebruiker data is missing.");


            var gebruiker = new GebruikerDB
            {
                Naam = dto.Naam,
                Paswoord = dto.Paswoord,
                Email = dto.Email,
                Telefoonnummer = dto.Telefoonnummer,
                Rol = dto.Rol
            };

            _context.Gebruikers.Add(gebruiker);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetGebruiker), new { id = gebruiker.ID }, gebruiker);
        }


        [HttpPut("{id}")]
        public ActionResult<GebruikerDB> PutGebruiker(int id, [FromBody] GebruikerDto dto)
        {
            if (dto == null)
                return BadRequest("ProductDTO data is missing.");
            var gebruiker = _context.Gebruikers.Find(id);
            if (gebruiker == null)
                return NotFound();

            gebruiker.Naam = dto.Naam;
            gebruiker.Email = dto.Email;
            gebruiker.Telefoonnummer = dto.Telefoonnummer;
            gebruiker.Paswoord = dto.Paswoord;

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