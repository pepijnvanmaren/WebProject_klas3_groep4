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
            var VerkochteProducten = await _context.Set<VerkochteProdcutenDB>()
                .FromSqlRaw("SELECT * FROM VerkochteProducten")
                .AsNoTracking()
                .ToListAsync();
            return Ok(VerkochteProducten);
        }

        // CREATE
        [HttpPost]
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