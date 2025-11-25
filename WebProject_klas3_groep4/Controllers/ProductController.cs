using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        //dependencies
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _env;  //Wordt niet gebruikt.

        //Constructor voor dependencies
        public ProductController(DatabaseContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //Getrequest
        [HttpGet]
        public ActionResult<IEnumerable<productDB>> GetProducten()
        {
            return Ok(_context.Producten.ToList());
        }

        //Getrequest (by id) //mag misschien weg?
        [HttpGet("{ID}")]
        public ActionResult<productDB> GetProduct(int ID)
        {
            var product = _context.Producten.Find(ID);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        //Model voor product aanmaken
        public class ProductCreateModel
        {
            public string Naam { get; set; }
            public string Beschrijving { get; set; }
            [Column(TypeName = "nvarchar(max)")]
            public IFormFile Foto { get; set; }
            public DateTime Oogstdatum { get; set; }
            public int? Potmaat { get; set; } // <-- changed to int?
            public double Gewicht { get; set; }
            public double? Steellengte { get; set; }
            public int Hoeveelheid { get; set; }
            public int MinimalePrijs { get; set; }
        }

        //Post voor product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateModel model)
        {
            //naam niet nullable
            if (string.IsNullOrWhiteSpace(model.Naam))
                return BadRequest("Naam is required.");

            //handles foto input
            string? imageDataUri = null;
            if (model.Foto != null && model.Foto.Length > 0)
            {
                var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(model.Foto.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || Array.IndexOf(allowedExt, ext) < 0)
                    return BadRequest("Invalid image type.");

                if (!model.Foto.ContentType.StartsWith("image/"))
                    return BadRequest("Uploaded file is not an image.");

                await using var ms = new MemoryStream();
                await model.Foto.CopyToAsync(ms);
                var bytes = ms.ToArray();

                if (bytes.LongLength > 10 * 1024 * 1024) // 10 MB limit
                    return BadRequest("Image too large. Max 10 MB.");

                var base64 = Convert.ToBase64String(bytes);
                imageDataUri = $"data:{model.Foto.ContentType};base64,{base64}";
            }

            //Checkt of oogstdatum niet in de toekomst is
            DateOnly oogstdatum;
            if (model.Oogstdatum != default)
                oogstdatum = DateOnly.FromDateTime(model.Oogstdatum);
            //anders zet de date op nu
            else
                oogstdatum = DateOnly.FromDateTime(DateTime.Now);

            int? potmaat = model.Potmaat; // direct gebruiken als nullable int

            double gewicht = model.Gewicht;
            double steellengte = model.Steellengte ?? 0;
            int hoeveelheid = model.Hoeveelheid;
            int minimalePrijs = model.MinimalePrijs;

            //Object van product om data in te stoppen en dan te posten
            var product = new productDB
            {
                Naam = model.Naam,
                Beschrijving = model.Beschrijving,
                Foto = imageDataUri,
                Oogstdatum = oogstdatum.ToDateTime(TimeOnly.MinValue),
                Potmaat = potmaat,
                Gewicht = gewicht,
                Steellengte = steellengte,
                Hoeveelheid = hoeveelheid,
                MinimalePrijs = minimalePrijs
            };

            _context.product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { ID = product.ID }, product);
        }

        //Putrequest
        [HttpPut("{ID}")]
        public ActionResult<productDB> PutProduct(int ID, [FromBody] ProductDto dto)
        {
            var product = _context.Producten.Find(ID);
            if (product == null)
                return NotFound();

            product.Naam = dto.Naam;
            product.Foto = dto.Foto;
            product.Beschrijving = dto.Beschrijving;
            product.Oogstdatum = dto.Oogstdatum.HasValue ? dto.Oogstdatum.Value.ToDateTime(TimeOnly.MinValue) : product.Oogstdatum;
            product.Potmaat = dto.Potmaat; // <-- assign int directly
            product.Gewicht = dto.Gewicht;
            product.Steellengte = dto.Steellengte;
            product.Hoeveelheid = dto.Hoeveelheid;
            product.MinimalePrijs = dto.MinimalePrijs;

            _context.SaveChanges();
            return Ok(product);
        }

        // DELETE product
        [HttpDelete("{ID}")]
        public ActionResult<productDB> DeleteProduct(int ID)
        {
            var product = _context.product.Find(ID);
            if (product == null)
                return NotFound();

            _context.product.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
