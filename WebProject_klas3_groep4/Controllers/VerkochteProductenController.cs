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
        public VerkochteProductenController(DatabaseContext context)
        {
            _context = context;
        }
        // GET ALL
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
                .FirstAsync();

            var prijsPerAantal = result.VerkochtePrijs / result.HoeveelHeid;

            return Ok(prijsPerAantal);
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
                .FirstAsync();

            var prijsPerAantal = result.VerkochtePrijs / result.HoeveelHeid;

            return Ok(prijsPerAantal);
        }
        // CREATE
        [HttpPost("{hvl:int},{vpp:double},{Pid:int},{Kid:int}")]
        public async Task<ActionResult> PostVeiling(int hvl, double vpp, int Pid, int Kid)
        {
            await _context.Database.ExecuteSqlRawAsync(@"INSERT INTO VerkochteProducten (HoeveelHeid, VerkochtePrijs, VerkoopDatum, ProductId, KoperId) VALUES (@hvl, @vpp, @datum, @pid, @kid)",
                new SqlParameter("@hvl", hvl),
                new SqlParameter("@vpp", vpp),
                new SqlParameter("@datum", DateTime.Now),
                new SqlParameter("@pid", Pid),
                new SqlParameter("@kid", Kid)
            );

            return Ok();
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