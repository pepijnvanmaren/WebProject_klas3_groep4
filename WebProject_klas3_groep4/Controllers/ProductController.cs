using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

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

        // GET ALL
        [HttpGet]
        public ActionResult<IEnumerable<ProductOutputDto>> GetProducten()
        {
            var producten = _context.Producten
                .Select(p => new ProductOutputDto
                {
                    Id = p.ID,
                    Naam = p.Naam,
                    Foto = p.Foto,
                    Beschrijving = p.Beschrijving,
                    Oogstdatum = p.Oogstdatum,
                    Potmaat = p.Potmaat,
                    Gewicht = p.Gewicht,
                    Steellengte = p.Steellengte,
                    Hoeveelheid = p.Hoeveelheid,
                    MinimalePrijs = p.MinimalePrijs
                })
                .ToList();

            return Ok(producten);
        }

        // GET SINGLE
        [HttpGet("{id:int}")]
        public ActionResult<ProductOutputDto> GetProduct(int id)
        {
            var product = _context.Producten.Find(id);
            if (product == null)
                return NotFound();

            var dto = new ProductOutputDto
            {
                Id = product.ID,
                Naam = product.Naam,
                Foto = product.Foto,
                Beschrijving = product.Beschrijving,
                Oogstdatum = product.Oogstdatum,
                Potmaat = product.Potmaat,
                Gewicht = product.Gewicht,
                Steellengte = product.Steellengte,
                Hoeveelheid = product.Hoeveelheid,
                MinimalePrijs = product.MinimalePrijs
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public ActionResult<ProductOutputDto> PostProduct([FromBody] ProductCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

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

            var outDto = new ProductOutputDto
            {
                Id = product.ID,
                Naam = product.Naam,
                Foto = product.Foto,
                Beschrijving = product.Beschrijving,
                Oogstdatum = product.Oogstdatum,
                Potmaat = product.Potmaat,
                Gewicht = product.Gewicht,
                Steellengte = product.Steellengte,
                Hoeveelheid = product.Hoeveelheid,
                MinimalePrijs = product.MinimalePrijs
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, outDto);
        }

        // UPDATE
        [HttpPut("{id:int}")]
        public ActionResult<ProductOutputDto> PutProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            var product = _context.Producten.Find(id);
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

            var outDto = new ProductOutputDto
            {
                Id = product.ID,
                Naam = product.Naam,
                Foto = product.Foto,
                Beschrijving = product.Beschrijving,
                Oogstdatum = product.Oogstdatum,
                Potmaat = product.Potmaat,
                Gewicht = product.Gewicht,
                Steellengte = product.Steellengte,
                Hoeveelheid = product.Hoeveelheid,
                MinimalePrijs = product.MinimalePrijs
            };

            return Ok(outDto);
        }

        // DELETE
        [HttpDelete("{id:int}")]
        public ActionResult DeleteProduct(int id)
        {
            var product = _context.Producten.Find(id);
            if (product == null)
                return NotFound();

            _context.product.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
