using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public LotController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<LotDB>> GetLots()
        {
            return Ok(_context.Lots.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<LotDB> GetLot(int id)
        {
            var Lot = _context.Lots.Find(id);
            if (Lot == null)
                return NotFound();
            return Ok(Lot);
        }

        [HttpPost]
        public ActionResult<LotDB> PostLot([FromBody] LotDB Lot)
        {
            if (Lot == null)
                return BadRequest();

            _context.Lots.Add(Lot);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetLot), new { id = Lot.ID }, Lot);
        }

        [HttpPut("{id}")]
        public ActionResult<LotDB> PutLot(int id, [FromBody] LotDB updatedLot)
        {
            var Lot = _context.Lots.Find(id);
            if (Lot == null)
                return NotFound();

            Lot.ProductID = updatedLot.ProductID;
            Lot.VeilingID = updatedLot.VeilingID;
            Lot.GebeurtenisDatum = updatedLot.GebeurtenisDatum;
            Lot.GewensteLocatie = updatedLot.GewensteLocatie;

            _context.SaveChanges();

            return Ok(Lot);
        }

        [HttpDelete("{id}")]
        public ActionResult<LotDB> DeleteLot(int id)
        {
            var Lot = _context.Lots.Find(id);

            if (Lot == null)
                return NotFound();

            _context.Lots.Remove(Lot);
            _context.SaveChanges();

            return NoContent();
        }
    }

}