using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

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

        // GET ALL
        [HttpGet]
        public ActionResult<IEnumerable<VeilingOutputDto>> GetVeilingen()
        {
            var veilingen = _context.Veilingen
                .Select(v => new VeilingOutputDto
                {
                    Id = v.ID,
                    StarTijd = v.StarTijd,
                    StartDatum = v.StartDatum,
                    AantalProducten = v.AantalProducten,
                    KlokLocatie = v.KlokLocatie,
                    HuidigeSituatieVanVeiling = v.HuidigeSituatieVanVeiling,
                    Bechrijving = v.Bechrijving
                })
                .ToList();

            return Ok(veilingen);
        }

        // GET SINGLE
        [HttpGet("{id:int}")]
        public ActionResult<VeilingOutputDto> GetVeiling(int id)
        {
            var veiling = _context.Veilingen.Find(id);
            if (veiling == null)
                return NotFound();

            var dto = new VeilingOutputDto
            {
                Id = veiling.ID,
                StarTijd = veiling.StarTijd,
                StartDatum = veiling.StartDatum,
                AantalProducten = veiling.AantalProducten,
                KlokLocatie = veiling.KlokLocatie,
                HuidigeSituatieVanVeiling = veiling.HuidigeSituatieVanVeiling,
                Bechrijving = veiling.Bechrijving
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public ActionResult<VeilingOutputDto> PostVeiling([FromBody] VeilingCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var veiling = new VeilingDB
            {
                StarTijd = dto.StarTijd ?? DateTime.Now.ToString("o"),
                StartDatum = dto.StartDatum ?? DateTime.Now.ToString("o"),
                AantalProducten = dto.AantalProducten,
                KlokLocatie = dto.KlokLocatie,
                HuidigeSituatieVanVeiling = dto.HuidigeSituatieVanVeiling,
                Bechrijving = dto.Bechrijving
            };

            _context.Veilingen.Add(veiling);
            _context.SaveChanges();

            var outDto = new VeilingOutputDto
            {
                Id = veiling.ID,
                StarTijd = veiling.StarTijd,
                StartDatum = veiling.StartDatum,
                AantalProducten = veiling.AantalProducten,
                KlokLocatie = veiling.KlokLocatie,
                HuidigeSituatieVanVeiling = veiling.HuidigeSituatieVanVeiling,
                Bechrijving = veiling.Bechrijving
            };

            return CreatedAtAction(nameof(GetVeiling), new { id = veiling.ID }, outDto);
        }

        // UPDATE
        [HttpPut("{id:int}")]
        public ActionResult<VeilingOutputDto> PutVeiling(int id, [FromBody] VeilingUpdateDto dto)
        {
            var veiling = _context.Veilingen.Find(id);
            if (veiling == null)
                return NotFound();

            veiling.StarTijd = dto.StarTijd ?? veiling.StarTijd;
            veiling.StartDatum = dto.StartDatum ?? veiling.StartDatum;
            veiling.AantalProducten = dto.AantalProducten;
            veiling.KlokLocatie = dto.KlokLocatie;
            veiling.HuidigeSituatieVanVeiling = dto.HuidigeSituatieVanVeiling;
            veiling.Bechrijving = dto.Bechrijving;

            _context.SaveChanges();

            var outDto = new VeilingOutputDto
            {
                Id = veiling.ID,
                StarTijd = veiling.StarTijd,
                StartDatum = veiling.StartDatum,
                AantalProducten = veiling.AantalProducten,
                KlokLocatie = veiling.KlokLocatie,
                HuidigeSituatieVanVeiling = veiling.HuidigeSituatieVanVeiling,
                Bechrijving = veiling.Bechrijving
            };

            return Ok(outDto);
        }

        // DELETE
        [HttpDelete("{id:int}")]
        public ActionResult DeleteVeiling(int id)
        {
            var veiling = _context.Veilingen.Find(id);
            if (veiling == null)
                return NotFound();

            _context.Veilingen.Remove(veiling);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
