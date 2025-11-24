using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(DatabaseContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

        public class ProductCreateModel
        {
            public string? Naam { get; set; }
            public string? Beschrijving { get; set; }
            public IFormFile? Foto { get; set; }
            public string? Oogstdatum { get; set; }
            public string? Potmaat { get; set; }
            public string? Gewicht { get; set; }
            public string? Steellengte { get; set; }
            public string? Hoeveelheid { get; set; }
            public string? MinimalePrijs { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateModel model)
        {
            string? imageUrl = null;

            if (model.Foto != null && model.Foto.Length > 0)
            {
                // simple validation: allow only common image types
                var permitted = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(model.Foto.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || Array.IndexOf(permitted, ext) < 0)
                {
                    return BadRequest("Invalid image type.");
                }

                var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
                Directory.CreateDirectory(uploadsRoot);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.Foto.CopyToAsync(stream);
                }

                imageUrl = $"/uploads/{fileName}";
            }

            // TODO: persist product to DB using your DatabaseContext (not implemented here)
            // Example response:
            return Ok(new { message = "Product saved (file uploaded)", imageUrl });
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