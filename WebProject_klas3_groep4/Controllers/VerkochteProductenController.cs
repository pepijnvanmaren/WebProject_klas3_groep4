using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerkochteProductenController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public VerkochteProductenController(DatabaseContext context)
        {
            _context = context;
        }
        // GET ALL
        [HttpGet]
        public ActionResult<IEnumerable<VerkochteProductenCreateDto>> GetVerkochteProducten()
        {
            var VerkochteProducten = _context.VerkochteProducten
                .Include(u => u.Product)
                .Include(u => u.Koper)
                .Select(u => new VerkochteProductenCreateDto
                {
                    HoeveelHeid = u.HoeveelHeid,
                    VerkochtePrijs = u.VerkochtePrijs,
                    ProductId = u.ProductId,
                    KoperId = u.KoperId,
                    VerkoopDatum = u.VerkoopDatum
                })
                .ToList();
            return Ok(VerkochteProducten);
        }

        // CREATE
        [HttpPost]
        public async Task<ActionResult<VerkochteProductenOutputDto>> PostVeiling([FromBody] VerkochteProductenCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid veiling data");

            var VerkochteProducten = new VerkochteProdcutenDB
            {
                HoeveelHeid = dto.HoeveelHeid,
                VerkochtePrijs = dto.VerkochtePrijs,
                ProductId = dto.ProductId,
                KoperId = dto.KoperId,
                VerkoopDatum = dto.VerkoopDatum = DateTime.Now
            };

            _context.VerkochteProducten.Add(VerkochteProducten);
            await _context.SaveChangesAsync();

            var outDto = new VerkochteProductenOutputDto
            {
                ID = VerkochteProducten.ID,
                HoeveelHeid = VerkochteProducten.HoeveelHeid,
                VerkochtePrijs = VerkochteProducten.VerkochtePrijs,
                ProductId = VerkochteProducten.ProductId,
                KoperId = VerkochteProducten.KoperId,
                VerkoopDatum = VerkochteProducten.VerkoopDatum
            };

            return CreatedAtAction(nameof(GetVerkochteProducten), new { id = VerkochteProducten.ID }, outDto);
        }
        [HttpDelete("{ID:int}")]
        public async Task<ActionResult> DeleteVerkochteProducten(int ID)
        {
            var p = await _context.VerkochteProducten.FindAsync(ID);
            if (p == null) return NotFound("VerkochteProducten niet gevonden");

            _context.VerkochteProducten.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}