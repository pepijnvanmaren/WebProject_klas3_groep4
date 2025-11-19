using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public VeilingController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VeilingDB>> GetVeilingen()
        {
            return Ok(_context.Veilingen.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<VeilingDB> GetVeiling(int id)
        {
            var Veiling = _context.Veilingen.Find(id);
            if (Veiling == null)
                return NotFound();
            return Ok(Veiling);
        }

        [HttpPost]
        public ActionResult<VeilingDB> PostVeiling([FromBody] VeilingDB Veiling)
        {
            if (Veiling == null)
                return BadRequest();

            _context.Veilingen.Add(Veiling);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetVeiling), new { id = Veiling.ID }, Veiling);
        }

        [HttpPut("{id}")]
        public ActionResult<VeilingDB> PutVeiling(int id, [FromBody] VeilingDB updatedVeiling)
        {
            var Veiling = _context.Veilingen.Find(id);
            if (Veiling == null)
                return NotFound();

            Veiling.StarTijd = updatedVeiling.StarTijd;
            Veiling.StartDatum = updatedVeiling.StartDatum;
            Veiling.AantalProducten = updatedVeiling.AantalProducten;
            Veiling.KlokLocatie = updatedVeiling.KlokLocatie;
            Veiling.HuidigeSituatieVanVeiling = updatedVeiling.HuidigeSituatieVanVeiling;
            Veiling.Bechrijving = updatedVeiling.Bechrijving;

            _context.SaveChanges();

            return Ok(Veiling);
        }

        [HttpDelete("{id}")]
        public ActionResult<VeilingDB> DeleteVeiling(int id)
        {
            var Veiling = _context.Veilingen.Find(id);

            if (Veiling == null)
                return NotFound();

            _context.Veilingen.Remove(Veiling);
            _context.SaveChanges();

            return NoContent();
        }
    }

}