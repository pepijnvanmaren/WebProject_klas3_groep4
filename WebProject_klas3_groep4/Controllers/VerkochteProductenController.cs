using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VerkochteProductenController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public VerkochteProductenController(DatabaseContext context)
        {
            _context = context;
        }
        // GET ALL
        [HttpGet]
        public ActionResult<IEnumerable<VerkochteProductenOutputDto>> GetVerkochteProducten()
        {
            var verkochteProducten = _context.VerkochteProducten
                .Include(v => v.Product)
                .Include(v => v.Koper)
                .Select(v => new VerkochteProductenOutputDto
                {
                    ID = v.ID,
                    HoeveelHeid = v.HoeveelHeid,
                    VerkochtePrijs = v.VerkochtePrijs,
                    ProductId = v.ProductId,
                    KoperId = v.KoperId
                })
                .ToList();

            return Ok(verkochteProducten);
        }


        // CREATE
        [HttpPost]
        public async Task<ActionResult<VerkochteProductenOutputDto>> PostVeiling(
        [FromBody] VerkochteProductenCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            var koperIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(koperIdClaim, out int koperId))
                return Unauthorized("Ongeldige gebruiker");

            var entity = new VerkochteProdcutenDB
            {
                HoeveelHeid = dto.HoeveelHeid,
                VerkochtePrijs = dto.VerkochtePrijs,
                ProductId = dto.ProductId,
                KoperId = koperId
            };

            _context.VerkochteProducten.Add(entity);
            await _context.SaveChangesAsync();

            var outDto = new VerkochteProductenOutputDto
            {
                ID = entity.ID,
                HoeveelHeid = entity.HoeveelHeid,
                VerkochtePrijs = entity.VerkochtePrijs,
                ProductId = entity.ProductId,
                KoperId = entity.KoperId
            };

            return CreatedAtAction(nameof(GetVerkochteProducten),
                new { id = entity.ID }, outDto);
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