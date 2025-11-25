using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize (Roles = "Admin")]
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

        [HttpPost]
        public ActionResult<AanvoerderDB> PostAanvoerder([FromBody] AanvoerderDB Aanvoerder)
        {
            if (Aanvoerder == null)
                return BadRequest();

            _context.Aanvoerder.Add(Aanvoerder);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAanvoerder), new { id = Aanvoerder.ID }, Aanvoerder);
        }

        [HttpPut("{id}")]
        public ActionResult<AanvoerderDB> PutAanvoerder(int id, [FromBody] AanvoerderDB updatedAanvoerder)
        {
            var Aanvoerder = _context.Aanvoerder.Find(id);
            if (Aanvoerder == null)
                return NotFound();

            Aanvoerder.Naam = updatedAanvoerder.Naam;
            Aanvoerder.Email = updatedAanvoerder.Email;
            Aanvoerder.Telefoonnummer = updatedAanvoerder.Telefoonnummer;
            Aanvoerder.KvkNummer = updatedAanvoerder.KvkNummer;
            Aanvoerder.NaamVanBedrijf = updatedAanvoerder.NaamVanBedrijf;
            Aanvoerder.Postcode = updatedAanvoerder.Postcode;
            Aanvoerder.Adres = updatedAanvoerder.Adres;
            Aanvoerder.BedrijfTelefoonnummer = updatedAanvoerder.BedrijfTelefoonnummer;
            Aanvoerder.BedrijfEmail = updatedAanvoerder.BedrijfEmail;
            Aanvoerder.prodcten = updatedAanvoerder.prodcten;

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