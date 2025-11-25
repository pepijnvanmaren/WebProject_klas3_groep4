using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ProductController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<productDB>> GetProducten()
        {
            return Ok(_context.product.ToList());
        }

        [HttpGet("{ID}")]
        public ActionResult<productDB> GetProduct(int ID)
        {
            var product = _context.product.Find(ID);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<productDB> PostProduct([FromBody] ProductDto dto)
        {
            if (dto == null)
                return BadRequest("Product data is missing.");

            var product = new productDB
            {
                Naam = dto.Naam,
                Foto = dto.Foto,
                Beschrijving = dto.Beschrijving,
                Oogstdatum = dto.Oogstdatum ?? DateOnly.FromDateTime(DateTime.Now),
                Potmaat = dto.Potmaat,
                Gewicht = dto.Gewicht,
                Steellengte = dto.Steellengte,
                Hoeveelheid = dto.Hoeveelheid,
                MinimalePrijs = dto.MinimalePrijs
            };

            _context.product.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetProduct), new { ID = product.ID }, product);
        }


        [HttpPut("{ID}")]
        public ActionResult<productDB> PutProduct(int ID, [FromBody] ProductDto dto)
        {
            var product = _context.product.Find(ID);
            if (product == null)
                return NotFound();

            product.Naam = dto.Naam;
            product.Foto = dto.Foto;
            product.Beschrijving = dto.Beschrijving;
            product.Oogstdatum = dto.Oogstdatum ?? product.Oogstdatum;
            product.Potmaat = dto.Potmaat;
            product.Gewicht = dto.Gewicht;
            product.Steellengte = dto.Steellengte;
            product.Hoeveelheid = dto.Hoeveelheid;
            product.MinimalePrijs = dto.MinimalePrijs;

            _context.SaveChanges();
            return Ok(product);
        }

        [HttpDelete("{ID}")]
        public ActionResult<productDB> DeleteProduct(int ID)
        {
            var product = _context.product.Find(ID);  // Was: _context.Gebruikers.Find(ID)
            if (product == null)
                return NotFound();

            _context.product.Remove(product);  // Was: _context.Gebruikers.Remove(product)
            _context.SaveChanges();
            return NoContent();
        }
    }

}