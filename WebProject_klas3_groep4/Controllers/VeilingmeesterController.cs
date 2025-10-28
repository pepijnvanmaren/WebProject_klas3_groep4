using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingmeesterController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public VeilingmeesterController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VeilingmeesterDB>> GetVeilingmeesters()
        {
            return Ok(_context.Veilingmeesters.ToList());
        }

        [HttpGet("{id:int}")]
        public ActionResult<VeilingmeesterDB> GetVeilingmeester(int id)
        {
            var Veilingmeester = _context.Veilingmeesters.Find(id);
            if (Veilingmeester == null)
                return NotFound();
            return Ok(Veilingmeester);
        }

        [HttpGet("{Email}")]
        public ActionResult<VeilingmeesterDB> GetVeilingmeester(string Email)
        {
            var Veilingmeester = _context.Veilingmeesters.Find(Email);
            if (Veilingmeester == null)
                return NotFound();
            return Ok(Veilingmeester);
        }

        [HttpPost]
        public ActionResult<VeilingmeesterDB> PostVeilingmeester([FromBody] VeilingmeesterDB Veilingmeester)
        {
            if (Veilingmeester == null)
                return BadRequest();

            _context.Veilingmeesters.Add(Veilingmeester);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetVeilingmeester), new { id = Veilingmeester.ID }, Veilingmeester);
        }

        [HttpPut("{id}")]
        public ActionResult<VeilingmeesterDB> PutVeilingmeester(int id, [FromBody] VeilingmeesterDB updatedVeilingmeester)
        {
            var Veilingmeester = _context.Veilingmeesters.Find(id);
            if (Veilingmeester == null)
                return NotFound();

            Veilingmeester.Naam = updatedVeilingmeester.Naam;
            Veilingmeester.Email = updatedVeilingmeester.Email;
            Veilingmeester.Telefoonnummer = updatedVeilingmeester.Telefoonnummer;
            Veilingmeester.VeilingVestiging = updatedVeilingmeester.VeilingVestiging;
            Veilingmeester.Veilingen = updatedVeilingmeester.Veilingen;


            _context.SaveChanges();

            return Ok(Veilingmeester);
        }

        [HttpDelete("{id}")]
        public ActionResult<VeilingmeesterDB> DeleteVeilingmeester(int id)
        {
            var Veilingmeester = _context.Veilingmeesters.Find(id);

            if (Veilingmeester == null)
                return NotFound();

            _context.Veilingmeesters.Remove(Veilingmeester);
            _context.SaveChanges();

            return NoContent();
        }
    }

}