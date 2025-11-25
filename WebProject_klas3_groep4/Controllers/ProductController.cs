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
        private readonly DatabaseContext _context;

        public ProductController(DatabaseContext context)
        {
            _context = context;
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

            _context.Producten.Add(product);
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
            product.Oogstdatum = dto.Oogstdatum ?? product.Oogstdatum;
            product.Potmaat = dto.Potmaat;
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

            _context.Producten.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
