using System;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProductController> _logger;

        public ProductController(DatabaseContext context, IWebHostEnvironment env, ILogger<ProductController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
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

            // Accept DateTime? so Swagger/ModelBinder can parse common date formats.
            public DateTime? Oogstdatum { get; set; }

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

                        // Logging for debugging
            _logger.LogInformation("Create product received Oogstdatum (modelbinder): {Oogstdatum}", model.Oogstdatum);
            var rawFormValue = Request.Form["Oogstdatum"].ToString();
            if (!string.IsNullOrEmpty(rawFormValue))
                _logger.LogInformation("Create product received Oogstdatum (raw form): {Raw}", rawFormValue);

            if (string.IsNullOrWhiteSpace(model.Naam))
                return BadRequest("Naam is required.");

            // Resolve DateOnly oogstdatum from either DateTime? binder or raw string fallback
            DateOnly oogstdatum;
            if (model.Oogstdatum.HasValue)
            {
                oogstdatum = DateOnly.FromDateTime(model.Oogstdatum.Value);
            }
            else
            {
                var dateFormats = new[]
                {
                    "yyyy-MM-dd",      // HTML date input
                    "yyyy/MM/dd",
                    "dd-MM-yyyy",
                    "dd/MM/yyyy",
                    "M/d/yyyy",
                    "MM/dd/yyyy",
                    "yyyy-MM-ddTHH:mm:ss",
                    "o"                // round-trip ISO
                };

                if (string.IsNullOrWhiteSpace(rawFormValue))
                {
                    // default to today when nothing provided
                    oogstdatum = DateOnly.FromDateTime(DateTime.Now);
                }
                else if (!DateOnly.TryParseExact(rawFormValue, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out oogstdatum))
                {
                    if (!DateOnly.TryParse(rawFormValue, CultureInfo.CurrentCulture, DateTimeStyles.None, out oogstdatum))
                    {
                        if (DateTime.TryParse(rawFormValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                        {
                            oogstdatum = DateOnly.FromDateTime(dt);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to parse Oogstdatum value: {Value}", rawFormValue);
                            return BadRequest("Invalid Oogstdatum format. Use yyyy-MM-dd or an ISO date.");
                        }
                    }
                }
            }

            if (!int.TryParse(model.Potmaat, NumberStyles.Integer, CultureInfo.InvariantCulture, out var potmaat))
                potmaat = 0;

            if (!double.TryParse(model.Gewicht, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var gewicht))
                gewicht = 0.0;

            if (!double.TryParse(model.Steellengte, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var steellengte))
                steellengte = 0.0;

            if (!int.TryParse(model.Hoeveelheid, NumberStyles.Integer, CultureInfo.InvariantCulture, out var hoeveelheid))
                hoeveelheid = 0;

            if (!int.TryParse(model.MinimalePrijs, NumberStyles.Integer, CultureInfo.InvariantCulture, out var minimalePrijs))
                minimalePrijs = 0;

            var product = new productDB
            {
                Naam = model.Naam,
                Beschrijving = model.Beschrijving,
                Foto = imageUrl,
                Oogstdatum = oogstdatum,
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
            var product = _context.product.Find(ID);
            if (product == null)
                return NotFound();

            _context.product.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }

}