using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;
using Microsoft.AspNetCore.Authorization;

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

        // ---------------------------------------------------------
        // GET ALL
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductOutputDto>>> GetProducten()
        {
            var producten = await _context.Producten
                .Select(p => new ProductOutputDto
                {
                    Id = p.ID,
                    Naam = p.Naam,
                    Foto = p.Foto,
                    Beschrijving = p.Beschrijving,
                    Oogstdatum = p.Oogstdatum.HasValue
                        ? DateOnly.FromDateTime(p.Oogstdatum.Value)
                        : DateOnly.FromDateTime(DateTime.Now),
                    Potmaat = p.Potmaat,
                    Gewicht = p.Gewicht,
                    Steellengte = p.Steellengte ?? 0,
                    Hoeveelheid = p.Hoeveelheid,
                    MinimalePrijs = p.MinimalePrijs
                })
                .ToListAsync();

            return Ok(producten);
        }

        // ---------------------------------------------------------
        // GET SINGLE
        // ---------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductOutputDto>> GetProduct(int id)
        {
            var p = await _context.Producten.FindAsync(id);
            if (p == null) return NotFound();

            var dto = new ProductOutputDto
            {
                Id = p.ID,
                Naam = p.Naam,
                Foto = p.Foto,
                Beschrijving = p.Beschrijving,
                Oogstdatum = p.Oogstdatum.HasValue
                    ? DateOnly.FromDateTime(p.Oogstdatum.Value)
                    : DateOnly.FromDateTime(DateTime.Now),
                Potmaat = p.Potmaat,
                Gewicht = p.Gewicht,
                Steellengte = p.Steellengte ?? 0,
                Hoeveelheid = p.Hoeveelheid,
                MinimalePrijs = p.MinimalePrijs
            };

            return Ok(dto);
        }

        // ---------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------
        //[Authorize(Roles ="Aanvoerder")]
        [HttpPost]
        public async Task<ActionResult<ProductOutputDto>> PostProduct([FromBody] ProductCreateDto dto)
        {
            if (dto == null) return BadRequest("Invalid product data");

            var oogstdatum = dto.Oogstdatum?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now;

            var product = new productDB
            {
                Naam = dto.Naam,
                Beschrijving = dto.Beschrijving,
                Foto = dto.Foto,
                Oogstdatum = oogstdatum,
                Potmaat = dto.Potmaat,
                Gewicht = dto.Gewicht,
                Steellengte = dto.Steellengte,
                Hoeveelheid = dto.Hoeveelheid,
                MinimalePrijs = dto.MinimalePrijs
            };

            _context.Producten.Add(product);
            await _context.SaveChangesAsync(); 

            var outDto = new ProductOutputDto
            {
                Id = product.ID,
                Naam = product.Naam,
                Foto = product.Foto,
                Beschrijving = product.Beschrijving,
                Oogstdatum = DateOnly.FromDateTime(product.Oogstdatum ?? DateTime.Now),
                Potmaat = product.Potmaat,
                Gewicht = product.Gewicht,
                Steellengte = product.Steellengte ?? 0,
                Hoeveelheid = product.Hoeveelheid,
                MinimalePrijs = product.MinimalePrijs
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, outDto);
        }

        // ---------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductOutputDto>> PutProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            var p = await _context.Producten.FindAsync(id);
            if (p == null) return NotFound();

            // Update alleen velden die niet null zijn
            if (dto.Naam != null) p.Naam = dto.Naam;
            if (dto.Beschrijving != null) p.Beschrijving = dto.Beschrijving;
            if (dto.Foto != null) p.Foto = dto.Foto;
            if (dto.Oogstdatum.HasValue) p.Oogstdatum = dto.Oogstdatum.Value.ToDateTime(TimeOnly.MinValue);
            if (dto.Potmaat.HasValue) p.Potmaat = dto.Potmaat.Value;
            if (dto.Gewicht.HasValue) p.Gewicht = dto.Gewicht.Value;
            if (dto.Steellengte.HasValue) p.Steellengte = dto.Steellengte.Value;
            if (dto.Hoeveelheid.HasValue) p.Hoeveelheid = dto.Hoeveelheid.Value;
            if (dto.MinimalePrijs.HasValue) p.MinimalePrijs = dto.MinimalePrijs.Value;

            await _context.SaveChangesAsync();

            var outDto = new ProductOutputDto
            {
                Id = p.ID,
                Naam = p.Naam,
                Foto = p.Foto,
                Beschrijving = p.Beschrijving,
                Oogstdatum = p.Oogstdatum.HasValue
                    ? DateOnly.FromDateTime(p.Oogstdatum.Value)
                    : DateOnly.FromDateTime(DateTime.Now),
                Potmaat = p.Potmaat,
                Gewicht = p.Gewicht,
                Steellengte = p.Steellengte ?? 0,
                Hoeveelheid = p.Hoeveelheid,
                MinimalePrijs = p.MinimalePrijs
            };

            return Ok(outDto);
        }

        // ---------------------------------------------------------
        // DELETE
        // ---------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var p = await _context.Producten.FindAsync(id);
            if (p == null) return NotFound();

            _context.Producten.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
