using Microsoft.AspNetCore.Mvc;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public ActionResult<productDB> PostProduct([FromBody] productDB product)
        {
            if (product == null)
                return BadRequest();

            _context.product.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, product);
        }

        [HttpPut("{ID}")]
        public ActionResult<productDB> PutProduct(int ID, [FromBody] productDB updatedProduct)
        {
            var product = _context.product.Find(ID);
            if (product == null)
                return NotFound();

            product.Naam = updatedProduct.Naam;
            product.Foto = updatedProduct.Foto;
            product.Beschrijving = updatedProduct.Beschrijving;
            product.bedrijf = updatedProduct.bedrijf;
            product.startprijs = updatedProduct.startprijs;
            product.hoeveelheid = updatedProduct.hoeveelheid;
            product.gekocht = updatedProduct.gekocht;





            _context.SaveChanges();

            return Ok(product);
        }

        [HttpDelete("{ID}")]
        public ActionResult<productDB> DeleteProduct(int ID)
        {
            var product = _context.Gebruikers.Find(ID);

            if (product == null)
                return NotFound();

            _context.Gebruikers.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }
    }

}
