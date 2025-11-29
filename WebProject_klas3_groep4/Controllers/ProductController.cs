using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<GebruikerDB> _userManager;

        public ProductController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------------------------------------------------------
        // GET ALL
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductOutputDto>>> GetProducten()
        {
            var producten = await _context.Producten
                .Include(p => p.Aanvoerder)
                .Include(p => p.Veiling)
                .Select(p => new ProductOutputDto
                {
                    Id = p.ID,
                    Naam = p.Naam,
                    Foto = p.Foto,
                    Beschrijving = p.Beschrijving,
                    Oogstdatum = p.Oogstdatum,
                    Potmaat = p.Potmaat,
                    Gewicht = p.Gewicht,
                    Steellengte = p.Steellengte ?? 0,
                    Hoeveelheid = p.Hoeveelheid,
                    MinimalePrijs = p.MinimalePrijs,
                    AanvoerderId = p.AanvoerderId,
                    AanvoerderNaam = p.Aanvoerder != null ? p.Aanvoerder.UserName : null,
                    VeilingId = p.VeilingId,
                    VeilingNaam = p.Veiling != null ? p.Veiling.StarTijd : null
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
            var p = await _context.Producten
                .Include(p => p.Aanvoerder)
                .Include(p => p.Veiling)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (p == null) return NotFound("Product niet gevonden");

            var dto = new ProductOutputDto
            {
                Id = p.ID,
                Naam = p.Naam,
                Foto = p.Foto,
                Beschrijving = p.Beschrijving,
                Oogstdatum = p.Oogstdatum,
                Potmaat = p.Potmaat,
                Gewicht = p.Gewicht,
                Steellengte = p.Steellengte ?? 0,
                Hoeveelheid = p.Hoeveelheid,
                MinimalePrijs = p.MinimalePrijs,
                AanvoerderId = p.AanvoerderId,
                AanvoerderNaam = p.Aanvoerder != null ? p.Aanvoerder.UserName : null,
                VeilingId = p.VeilingId,
                VeilingNaam = p.Veiling != null ? p.Veiling.StarTijd : null
            };

            return Ok(dto);
        }

        // ---------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------
        [Authorize(Roles = "Aanvoerder")]
        [HttpPost]
        public async Task<ActionResult<ProductOutputDto>> PostProduct([FromBody] ProductCreateDto dto)
        {
            if (dto == null) return BadRequest("Invalid product data");

            // NIEUW: Haal de ingelogde gebruiker op
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("Gebruiker niet gevonden");

            // Controleer of veiling bestaat als deze wordt meegegeven
            if (dto.VeilingId.HasValue)
            {
                var veiling = await _context.Veilingen.FindAsync(dto.VeilingId);
                if (veiling == null)
                    return BadRequest("Veiling niet gevonden");
            }

            var product = new productDB
            {
                Naam = dto.Naam,
                Beschrijving = dto.Beschrijving,
                Foto = dto.Foto != null ? $"data:image/jpeg;base64,{dto.Foto}" : null,  // Voeg het prefix toe
                Oogstdatum = dto.Oogstdatum,
                Potmaat = dto.Potmaat,
                Gewicht = dto.Gewicht,
                Steellengte = dto.Steellengte,
                Hoeveelheid = dto.Hoeveelheid,
                MinimalePrijs = dto.MinimalePrijs,
                AanvoerderId = user.Id,  // NIEUW: Automatisch gekoppeld aan ingelogde gebruiker
                VeilingId = dto.VeilingId
            };

            _context.Producten.Add(product);
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
                Steellengte = product.Steellengte ?? 0,
                Hoeveelheid = product.Hoeveelheid,
                MinimalePrijs = product.MinimalePrijs,
                AanvoerderId = product.AanvoerderId,
                AanvoerderNaam = user.UserName,
                VeilingId = product.VeilingId
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, outDto);
        }

        // ---------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------
        [Authorize(Roles = "Aanvoerder")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductOutputDto>> PutProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            var p = await _context.Producten.FindAsync(id);
            if (p == null) return NotFound("Product niet gevonden");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Alleen de eigenaar of admin kan aanpassen
            if (p.AanvoerderId != user.Id)
                return Forbid("Je kunt dit product niet bewerken");

            // Controleer of nieuwe veiling bestaat als deze wordt meegegeven
            if (dto.VeilingId.HasValue && dto.VeilingId != p.VeilingId)
            {
                var veiling = await _context.Veilingen.FindAsync(dto.VeilingId);
                if (veiling == null)
                    return BadRequest("Veiling niet gevonden");
            }

            // Update alleen velden die niet null zijn
            if (dto.Naam != null) p.Naam = dto.Naam;
            if (dto.Beschrijving != null) p.Beschrijving = dto.Beschrijving;
            if (dto.Foto != null) p.Foto = dto.Foto;
            if (dto.Oogstdatum.HasValue) p.Oogstdatum = dto.Oogstdatum.Value;
            if (dto.Potmaat.HasValue) p.Potmaat = dto.Potmaat.Value;
            if (dto.Gewicht.HasValue) p.Gewicht = dto.Gewicht.Value;
            if (dto.Steellengte.HasValue) p.Steellengte = dto.Steellengte.Value;
            if (dto.Hoeveelheid.HasValue) p.Hoeveelheid = dto.Hoeveelheid.Value;
            if (dto.MinimalePrijs.HasValue) p.MinimalePrijs = dto.MinimalePrijs.Value;
            if (dto.VeilingId.HasValue) p.VeilingId = dto.VeilingId;

            await _context.SaveChangesAsync();

            var outDto = new ProductOutputDto
            {
                Id = p.ID,
                Naam = p.Naam,
                Foto = p.Foto,
                Beschrijving = p.Beschrijving,
                Oogstdatum = p.Oogstdatum,
                Potmaat = p.Potmaat,
                Gewicht = p.Gewicht,
                Steellengte = p.Steellengte ?? 0,
                Hoeveelheid = p.Hoeveelheid,
                MinimalePrijs = p.MinimalePrijs,
                AanvoerderId = p.AanvoerderId,
                VeilingId = p.VeilingId
            };

            return Ok(outDto);
        }

        // ---------------------------------------------------------
        // DELETE
        // ---------------------------------------------------------
        [Authorize(Roles = "Aanvoerder")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var p = await _context.Producten.FindAsync(id);
            if (p == null) return NotFound("Product niet gevonden");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Alleen de eigenaar kan verwijderen
            if (p.AanvoerderId != user.Id)
                return Forbid("Je kunt dit product niet verwijderen");

            _context.Producten.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------------------------------------------------------
        // GET PRODUCTS BY VEILING
        // ---------------------------------------------------------
        [HttpGet("veiling/{veilingId:int}")]
        public async Task<ActionResult<IEnumerable<ProductOutputDto>>> GetProductenByVeiling(int veilingId)
        {
            var producten = await _context.Producten
                .Where(p => p.VeilingId == veilingId)
                .Include(p => p.Aanvoerder)
                .Include(p => p.Veiling)
                .Select(p => new ProductOutputDto
                {
                    Id = p.ID,
                    Naam = p.Naam,
                    Foto = p.Foto,
                    Beschrijving = p.Beschrijving,
                    Oogstdatum = p.Oogstdatum,
                    Potmaat = p.Potmaat,
                    Gewicht = p.Gewicht,
                    Steellengte = p.Steellengte ?? 0,
                    Hoeveelheid = p.Hoeveelheid,
                    MinimalePrijs = p.MinimalePrijs,
                    AanvoerderId = p.AanvoerderId,
                    AanvoerderNaam = p.Aanvoerder != null ? p.Aanvoerder.UserName : null,
                    VeilingId = p.VeilingId
                })
                .ToListAsync();

            return Ok(producten);
        }

        // ---------------------------------------------------------
        // GET PRODUCTS BY AANVOERDER
        // ---------------------------------------------------------
        [HttpGet("aanvoerder/{aanvoerderId:int}")]
        public async Task<ActionResult<IEnumerable<ProductOutputDto>>> GetProductenByAanvoerder(int aanvoerderId)
        {
            var producten = await _context.Producten
                .Where(p => p.AanvoerderId == aanvoerderId)
                .Include(p => p.Aanvoerder)
                .Include(p => p.Veiling)
                .Select(p => new ProductOutputDto
                {
                    Id = p.ID,
                    Naam = p.Naam,
                    Foto = p.Foto,
                    Beschrijving = p.Beschrijving,
                    Oogstdatum = p.Oogstdatum,
                    Potmaat = p.Potmaat,
                    Gewicht = p.Gewicht,
                    Steellengte = p.Steellengte ?? 0,
                    Hoeveelheid = p.Hoeveelheid,
                    MinimalePrijs = p.MinimalePrijs,
                    AanvoerderId = p.AanvoerderId,
                    AanvoerderNaam = p.Aanvoerder != null ? p.Aanvoerder.UserName : null,
                    VeilingId = p.VeilingId
                })
                .ToListAsync();

            return Ok(producten);
        }
    }
}