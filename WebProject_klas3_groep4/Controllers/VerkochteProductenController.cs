using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerkochteProductenController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;
        public VerkochteProductenController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET 
        [HttpGet]
        public async Task<ActionResult> GetVerkochteProducten()
        {
            var result = await _context.VerkochteProducten
                .FromSqlRaw("SELECT * FROM VerkochteProducten")
                .ToListAsync();

            return Ok(result);
        }
        [HttpGet("GetallProducten/{id:int}")]
        public async Task<ActionResult> GetallProducten(int id)
        {
            var result = await _context.VerkochteProducten
                .FromSqlRaw(
                    "SELECT * FROM VerkochteProducten WHERE ProductId = @ID",
                    new SqlParameter("@ID", id)
                )
                .Select(vp => new
                {
                    Result = vp.HoeveelHeid != 0
                     ? vp.VerkochtePrijs / vp.HoeveelHeid
                     : 0,
                    vp.VerkoopDatum
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetallAllProducten")]
        public async Task<ActionResult> GetallAllProducten()
        {
            var result = await _context.VerkochteProducten
                .FromSqlRaw(
                    "SELECT * FROM VerkochteProducten"
                )
                .Select(vp => new
                {
                    Result = vp.HoeveelHeid != 0
                     ? vp.VerkochtePrijs / vp.HoeveelHeid
                     : 0,
                    vp.VerkoopDatum
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetallGemiddeldeProduct/{id:int}")]
        public async Task<ActionResult> GetallGemiddeldeProduct(int id)
        {
            var result = await _context.Set<GemiddeldeAllesDto>()
               .FromSqlRaw(@"SELECT 
                            SUM(VerkochtePrijs) AS VerkochtePrijs,
                            SUM(HoeveelHeid) AS HoeveelHeid
                            FROM VerkochteProducten
                            WHERE ProductId = @ID",
                            new SqlParameter("@ID", id)
                )
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null || result.HoeveelHeid == 0)
            {
                return Ok(0);
            }

            var prijsPerAantal = result.VerkochtePrijs / result.HoeveelHeid;

            return Ok(Math.Round(prijsPerAantal, 2));
        }

        [HttpGet("GetallGemiddeldeAlles")]
        public async Task<ActionResult> GetallGemiddeldeAlles()
        {
            var result = await _context.Set<GemiddeldeAllesDto>()
                .FromSqlRaw(@"SELECT 
                            SUM(VerkochtePrijs) AS VerkochtePrijs,
                            SUM(HoeveelHeid) AS HoeveelHeid
                            FROM VerkochteProducten")
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null || result.HoeveelHeid == 0)
            {
                return Ok(0);
            }

            var prijsPerAantal = result.VerkochtePrijs / result.HoeveelHeid;

            return Ok(Math.Round(prijsPerAantal, 2));
        }
        // CREATE
        [HttpPost]
        public async Task<ActionResult> PostVeiling([FromBody] PostVeilingDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("Gebruiker niet gevonden");

            await _context.Database.ExecuteSqlRawAsync(
                @"INSERT INTO VerkochteProducten 
          (HoeveelHeid, VerkochtePrijs, VerkoopDatum, ProductId, KoperId) 
          VALUES (@hvl, @vpp, @datum, @pid, @kid)",
                new SqlParameter("@hvl", dto.Aantal),
                new SqlParameter("@vpp", dto.Prijs),
                new SqlParameter("@datum", DateTime.UtcNow),
                new SqlParameter("@pid", dto.ProductId),
                new SqlParameter("@kid", user.Id)
            );

            return Ok();
        }
        // DELETE
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