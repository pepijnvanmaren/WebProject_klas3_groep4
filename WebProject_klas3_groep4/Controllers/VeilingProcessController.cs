using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/veiling-process")]
    public class VeilingProcessController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public VeilingProcessController(DatabaseContext context)
        {
            _context = context;
        }

        // ========================================
        // START VEILING
        // ========================================
        [HttpPost("start")]
        public async Task<IActionResult> StartVeiling()
        {
            // Check of er al een actieve veiling is
            var actieveVeiling = await _context.Producten
                .AnyAsync(p => p.Status == VeilingStatus.Actief);

            if (actieveVeiling)
                return BadRequest("Er is al een actieve veiling");

            // Haal alle producten in wachtrij op
            var productenInWachtrij = await _context.Producten
                .Where(p => p.Status == VeilingStatus.InWachtrij)
                .OrderBy(p => p.ID)
                .ToListAsync();

            if (!productenInWachtrij.Any())
                return BadRequest("Geen producten in wachtrij");

            // Geef elk product een volgnummer
            int volgorde = 1;
            foreach (var product in productenInWachtrij)
            {
                product.VeilingVolgorde = volgorde++;
            }

            // Start eerste product
            var eersteProduct = productenInWachtrij.First();
            eersteProduct.Status = VeilingStatus.Actief;
            eersteProduct.VeilingStartTijd = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Veiling gestart",
                aantalProducten = productenInWachtrij.Count,
                huidigProduct = MapToOutputDto(eersteProduct)
            });
        }

        // ========================================
        // STOP VEILING
        // ========================================
        [HttpPost("stop")]
        public async Task<IActionResult> StopVeiling()
        {
            var actieveProducten = await _context.Producten
                .Where(p => p.Status == VeilingStatus.Actief || p.Status == VeilingStatus.InWachtrij)
                .ToListAsync();

            foreach (var product in actieveProducten)
            {
                if (product.Status == VeilingStatus.Actief)
                    product.Status = VeilingStatus.VerlatenVeiling;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Veiling gestopt" });
        }

        // ========================================
        // GET VEILING STATUS
        // ========================================
        [HttpGet("status")]
        public async Task<IActionResult> GetVeilingStatus()
        {
            var actiefProduct = await _context.Producten
                .FirstOrDefaultAsync(p => p.Status == VeilingStatus.Actief);

            var volgendProduct = await _context.Producten
                .Where(p => p.Status == VeilingStatus.InWachtrij)
                .OrderBy(p => p.VeilingVolgorde)
                .FirstOrDefaultAsync();

            var aantalInWachtrij = await _context.Producten
                .CountAsync(p => p.Status == VeilingStatus.InWachtrij);

            bool isInPauze = false;
            int pauzeRemainingSeconds = 0;
            const int PauzeTijd = 30; // seconden

            if (actiefProduct != null && actiefProduct.IsGekocht && actiefProduct.VerkochtOp.HasValue)
            {
                isInPauze = true;
                var elapsed = (int)(DateTime.UtcNow - actiefProduct.VerkochtOp.Value).TotalSeconds;
                pauzeRemainingSeconds = Math.Max(0, PauzeTijd - elapsed);
                // If pauze has passed, we still let front-end call /volgende to transition.
            }

            return Ok(new
            {
                isActief = actiefProduct != null,
                huidigProduct = actiefProduct != null ? MapToOutputDto(actiefProduct) : null,
                volgendProduct = volgendProduct != null ? MapToOutputDto(volgendProduct) : null,
                aantalInWachtrij = aantalInWachtrij,
                volgorde = actiefProduct?.VeilingVolgorde,
                isInPauze = isInPauze,
                pauzeRemainingSeconds = pauzeRemainingSeconds
            });
        }

        // ========================================
        // GET PRODUCT QUEUE
        // ========================================
        [HttpGet("queue")]
        public async Task<IActionResult> GetQueue()
        {
            var producten = await _context.Producten
                .Where(p => p.Status == VeilingStatus.InWachtrij || p.Status == VeilingStatus.Actief)
                .OrderBy(p => p.VeilingVolgorde ?? int.MaxValue)
                .ToListAsync();

            var dtos = producten.Select(p => MapToOutputDto(p)).ToList();

            return Ok(dtos);
        }

        // ========================================
        // KOOP PRODUCT
        // ========================================
        [Authorize]
        [HttpPost("koop")]
        public async Task<IActionResult> KoopProduct([FromBody] KoopProductDto dto)
        {
            var product = await _context.Producten
                .FirstOrDefaultAsync(p => p.ID == dto.ProductId && p.Status == VeilingStatus.Actief);

            if (product == null)
                return NotFound("Product niet actief in veiling");

            if (dto.Aantal < 1)
                return BadRequest("Aantal moet minimaal 1 zijn.");

            if (dto.Aantal > product.Hoeveelheid)
                return BadRequest("Aantal is groter dan beschikbare voorraad.");

            var koperIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(koperIdClaim, out int koperId))
                return Unauthorized();

            // Markeer als gekocht maar laat status Actief - we gaan in pauze
            product.Hoeveelheid -= dto.Aantal;
            product.IsGekocht = true;
            product.VerkochtOp = DateTime.UtcNow;
            product.KoperID = koperId;
            product.VerkochtePrijs = dto.Prijs;
            // We zetten Status pas op Verkocht in VolgendProduct zodat frontend/pauze goed werkt

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product gekocht!",
                product = MapToOutputDto(product),
                koperId = koperId,
                prijs = dto.Prijs,
                pauzeDurationSeconds = 30
            });
        }

        // ========================================
        // VOLGENDE PRODUCT (na 30 sec pauze)
        // ========================================
        [HttpPost("volgende")]
        public async Task<IActionResult> VolgendProduct()
        {
            // Haal huidig actief product op
            var huidigProduct = await _context.Producten
                .FirstOrDefaultAsync(p => p.Status == VeilingStatus.Actief);

            if (huidigProduct != null)
            {
                if (huidigProduct.IsGekocht)
                {
                    // Markeer als verkocht omdat het gekocht is
                    huidigProduct.Status = VeilingStatus.Verkocht;
                }
                else
                {
                    // Niet verkocht -> verlaten veiling
                    huidigProduct.Status = VeilingStatus.VerlatenVeiling;
                }
            }

            // Zoek volgend product in wachtrij
            var volgendProduct = await _context.Producten
                .Where(p => p.Status == VeilingStatus.InWachtrij)
                .OrderBy(p => p.VeilingVolgorde)
                .FirstOrDefaultAsync();

            if (volgendProduct == null)
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Geen producten meer in wachtrij",
                    veilingAfgelopen = true
                });
            }

            // Start volgend product
            volgendProduct.Status = VeilingStatus.Actief;
            volgendProduct.VeilingStartTijd = DateTime.UtcNow;
            // Reset eventuele flags
            volgendProduct.IsGekocht = false;
            volgendProduct.VerkochtOp = null;
            volgendProduct.KoperID = null;
            volgendProduct.VerkochtePrijs = null;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Volgend product gestart",
                product = MapToOutputDto(volgendProduct),
                veilingAfgelopen = false
            });
        }

        [Authorize]
        [HttpPost("herstart")]
        public async Task<IActionResult> HerstartProduct([FromBody] HerstartProductDto dto)
        {
            var product = await _context.Producten
                .FirstOrDefaultAsync(p => p.ID == dto.ProductId);

            if (product == null)
                return NotFound("Product niet gevonden");

            // Update de hoeveelheid met de overgebleven hoeveelheid
            product.Hoeveelheid = dto.NieuweHoeveelheid;

            // Zet het product opnieuw in de wachtrij
            product.Status = VeilingStatus.InWachtrij;

            // Optioneel: reset flags
            product.IsGekocht = false;
            product.VerkochtOp = null;
            product.KoperID = null;
            product.VerkochtePrijs = null;

            // Voeg product weer op de juiste volgorde toe
            int maxVolgorde = await _context.Producten.MaxAsync(p => (int?)p.VeilingVolgorde) ?? 0;
            product.VeilingVolgorde = maxVolgorde + 1;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product opnieuw in veiling gezet", product = product.ID });
        }

        // ========================================
        // HELPER METHOD
        // ========================================
        private ProductOutputDto MapToOutputDto(productDB p)
        {
            return new ProductOutputDto
            {
                Id = p.ID,
                Naam = p.Naam,
                Foto = p.Foto,
                Beschrijving = p.Beschrijving,
                Oogstdatum = p.Oogstdatum.HasValue
                    ? DateOnly.FromDateTime(p.Oogstdatum.Value)
                    : DateOnly.FromDateTime(DateTime.UtcNow), // Gebruik UtcNow in plaats van Now
                Potmaat = p.Potmaat,
                Gewicht = p.Gewicht,
                Steellengte = p.Steellengte ?? 0,
                Hoeveelheid = p.Hoeveelheid,
                MinimalePrijs = p.MinimalePrijs
            };
        }
    }

    // ========================================
    // DTO
    // ========================================
    public class KoopProductDto
    {
        public int ProductId { get; set; }
        public int Aantal { get; set; }
        public double Prijs { get; set; }
    }

    public class HerstartProductDto
    {
        public int ProductId { get; set; }
        public int NieuweHoeveelheid { get; set; }
    }

}