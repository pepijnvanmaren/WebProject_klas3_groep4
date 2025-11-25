using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("all")]
        public ActionResult<IEnumerable<VeilingDB>> GetVeilingen()
        {
            return Ok(_context.Veilingen.ToList());
        }

        [HttpGet("AlleVeiling/{ID}")]
        public async Task<ActionResult<VeilingDB>> GetVeiling(int ID)
        {
            var Veiling = await _context.Veilingen
                .FirstOrDefaultAsync(v => v.ID == ID);
            if (Veiling == null)
                {
                return NotFound();
            }

            var VeilingDto = new VeilingDB
            {
                StarTijd = Veiling.StarTijd,
                StartDatum = Veiling.StartDatum,
                AantalProducten = Veiling.AantalProducten,
                KlokLocatie = Veiling.KlokLocatie,
                HuidigeSituatieVanVeiling = Veiling.HuidigeSituatieVanVeiling,
                Bechrijving = Veiling.Bechrijving
            };

            return VeilingDto;
        }

        [HttpPost]
        public ActionResult<VeilingDB> PostVeilingDto([FromBody] VeilingDB dto)
        {
            if (dto == null)
                return BadRequest("Veiling data is missing.");
            var Veiling = new VeilingDB
            {
                StarTijd = dto.StarTijd,
                StartDatum = dto.StartDatum,
                KlokLocatie = dto.KlokLocatie,
                Bechrijving = dto.Bechrijving
            };
            _context.Veilingen.Add(Veiling);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetVeiling), new { id = Veiling.ID }, Veiling);
        }

        [HttpPut("Veiling/{id}")]
        public ActionResult<VeilingDB> PutVeiling(int id, [FromBody] VeilingDB dto)
        {
            var existingVeiling = _context.Veilingen.Find(id);
            if (existingVeiling == null)
                return NotFound();

            existingVeiling.StarTijd = dto.StarTijd;
            existingVeiling.StartDatum = dto.StartDatum;
            existingVeiling.AantalProducten = dto.AantalProducten;
            existingVeiling.KlokLocatie = dto.KlokLocatie;
            existingVeiling.HuidigeSituatieVanVeiling = dto.HuidigeSituatieVanVeiling;
            existingVeiling.Bechrijving = dto.Bechrijving;

            _context.Veilingen.Update(existingVeiling);
            _context.SaveChanges();
            return Ok(existingVeiling);
        }
        [HttpPut("Producten/{id}")]
        public ActionResult<VeilingDB> PutVeilingAantalDto(int id, [FromBody] VeilingDB dto)
        {
            var existingVeiling = _context.Veilingen.Find(id);
            if (existingVeiling == null)
                return NotFound();

            existingVeiling.AantalProducten = dto.AantalProducten;

            _context.Veilingen.Update(existingVeiling);
            _context.SaveChanges();
            return Ok(existingVeiling);
        }
        [HttpPut("Situatie/{id}")]

        public ActionResult<VeilingDB> PutVeilingSituatieDto(int id, [FromBody] VeilingDB dto)
        {
            var existingVeiling = _context.Veilingen.Find(id);
            if (existingVeiling == null)
                return NotFound();

            existingVeiling.HuidigeSituatieVanVeiling = dto.HuidigeSituatieVanVeiling;

            _context.Veilingen.Update(existingVeiling);
            _context.SaveChanges();
            return Ok(existingVeiling);
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