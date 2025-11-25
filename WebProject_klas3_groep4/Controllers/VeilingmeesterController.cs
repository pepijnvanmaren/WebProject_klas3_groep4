using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("All")]
        public ActionResult<IEnumerable<VeilingmeesterDB>> GetVeilingmeesters()
        {
            return Ok(_context.Veilingmeesters.ToList());
        }

        [HttpGet("VeilingMeester/{id}")]
        public async Task<ActionResult<VeilingmeesterDB>> GetVeilingmeester(int id)
        {
            var Veilingmeester = await _context.Veilingmeesters
                .FirstOrDefaultAsync(v => v.ID == id);

            if (Veilingmeester == null)
                {
                return NotFound();
            }

            var VeilingmeesterDto = new VeilingmeesterDB
            {
                Naam = Veilingmeester.Naam,
                Paswoord = Veilingmeester.Paswoord,
                Email = Veilingmeester.Email,
                Telefoonnummer = Veilingmeester.Telefoonnummer,
                VeilingVestiging = Veilingmeester.VeilingVestiging
            };

            return VeilingmeesterDto;
        }

        [HttpPost]
        public ActionResult<VeilingmeesterDB> PostVeilingmeester([FromBody] VeilingmeesterDB dto)
        {
            if (dto == null)
                return BadRequest("Veilingmeester data is missing.");
            var Veilingmeester = new VeilingmeesterDB
            {
                Naam = dto.Naam,
                Paswoord = dto.Paswoord,
                Email = dto.Email,
                Telefoonnummer = dto.Telefoonnummer,
                VeilingVestiging = dto.VeilingVestiging
            };

            _context.Veilingmeesters.Add(Veilingmeester);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetVeilingmeester), new { id = Veilingmeester.ID }, Veilingmeester);
        }

        [HttpPut("AccountGegevens/{id}")]   
        public ActionResult<VeilingmeesterDB> PutVeilingmeester(int id, [FromBody] VeilingmeesterDB dto)
        {
            var Veilingmeester = _context.Veilingmeesters.Find(id);
            if (Veilingmeester == null)
                return NotFound();

            Veilingmeester.Naam = dto.Naam;
            Veilingmeester.Paswoord = dto.Paswoord;
            Veilingmeester.Email = dto.Email;
            Veilingmeester.Telefoonnummer = dto.Telefoonnummer;
            Veilingmeester.VeilingVestiging = dto.VeilingVestiging;

            _context.Veilingmeesters.Update(Veilingmeester);
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