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
        private const int VEILING_DUUR_SECONDS = 30;

        public VeilingProcessController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("gestart-product")]
        public async Task<IActionResult> GetGestartProduct()
        {
            var producten = await _context.Producten
                .Where(p => p.productstatus == "Gestart")
                .OrderBy(p => p.ID)
                .ToListAsync();

            return Ok(new 
            {
                producten
            });
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
                .OrderBy(p => p.VeilingVolgorde)
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
            eersteProduct.VeilingStartTijd = DateTime.UtcNow; // ✅ zet starttijd
            eersteProduct.IsGekocht = false; // reset flags
            eersteProduct.VerkochtOp = null;
            eersteProduct.KoperID = null;
            eersteProduct.VerkochtePrijs = null;

            await _context.SaveChangesAsync();

            Console.WriteLine($"DEBUG: Veiling gestart -> {eersteProduct.Naam}, VeilingStartTijd={eersteProduct.VeilingStartTijd}");

            return Ok(new
            {
                message = "Veiling gestart",
                aantalProducten = productenInWachtrij.Count,
                huidigProduct = MapToOutputDto(eersteProduct)
            });
        }

        // ========================================
        // GET VEILING STATUS
        // ========================================
        [HttpGet("status")]
        public async Task<IActionResult> GetVeilingStatus()
        {
            try
            {
                // Huidig actief product
                var huidigProductEntity = await _context.Producten
                    .AsNoTracking()
                    .Where(p => p.Status == VeilingStatus.Actief)
                    .OrderBy(p => p.VeilingVolgorde)
                    .FirstOrDefaultAsync();

                // Volgend product in wachtrij
                var volgendProduct = await _context.Producten
                    .AsNoTracking()
                    .Where(p => p.Status == VeilingStatus.InWachtrij)
                    .OrderBy(p => p.VeilingVolgorde)
                    .Select(p => new ProductVeilingDto
                    {
                        Id = p.ID,
                        Naam = p.Naam,
                        Foto = p.Foto,
                        Beschrijving = p.Beschrijving,
                        Hoeveelheid = p.Hoeveelheid,
                        MinimalePrijs = p.MinimalePrijs,
                        HuidigePrijs = p.MinimalePrijs // startprijs = min totdat actief
                    })
                    .FirstOrDefaultAsync();

                // Aantal producten in wachtrij
                var aantalInWachtrij = await _context.Producten
                    .AsNoTracking()
                    .CountAsync(p => p.Status == VeilingStatus.InWachtrij);

                bool isActief = huidigProductEntity != null;
                bool isInPauze = false;
                int remainingSeconds = 0;
                ProductVeilingDto? huidigProduct = null;

                if (huidigProductEntity != null)
                {
                    if (!huidigProductEntity.VeilingStartTijd.HasValue)
                    {
                        huidigProductEntity.VeilingStartTijd = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }

                    var elapsedSeconds = (int)(DateTime.UtcNow - huidigProductEntity.VeilingStartTijd.Value).TotalSeconds;
                    remainingSeconds = Math.Max(0, VEILING_DUUR_SECONDS - elapsedSeconds);
                    if (remainingSeconds <= 0)
                        isInPauze = true;

                    huidigProduct = new ProductVeilingDto
                    {
                        Id = huidigProductEntity.ID,
                        Naam = huidigProductEntity.Naam,
                        Foto = huidigProductEntity.Foto,
                        Beschrijving = huidigProductEntity.Beschrijving,
                        Hoeveelheid = huidigProductEntity.Hoeveelheid,
                        MinimalePrijs = huidigProductEntity.MinimalePrijs,
                        HuidigePrijs = CalculateCurrentPrice(huidigProductEntity)
                    };

                    Console.WriteLine($"DEBUG: {huidigProduct.Naam}, elapsed={elapsedSeconds}s, huidigePrijs={huidigProduct.HuidigePrijs}");
                }

                var statusDto = new VeilingStatusDto
                {
                    IsActief = isActief,
                    IsInPauze = isInPauze,
                    RemainingSeconds = remainingSeconds,
                    HuidigProduct = huidigProduct,
                    VolgendProduct = volgendProduct,
                    AantalInWachtrij = aantalInWachtrij
                };

                return Ok(statusDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Er is iets misgegaan bij ophalen van de veiling status");
            }
        }

        // ========================================
        // Huidige prijs berekenen
        // ========================================
        private double CalculateCurrentPrice(productDB product)
        {
            if (!product.VeilingStartTijd.HasValue)
                return product.MinimalePrijs;

            var elapsedSeconds = (DateTime.UtcNow - product.VeilingStartTijd.Value).TotalSeconds;
            var progress = Math.Max(0, Math.Min(1, elapsedSeconds / VEILING_DUUR_SECONDS));

            double min = product.MinimalePrijs;
            double max = min * 10; // startprijs = 10× minimale prijs
            var currentPrice = max - (max - min) * progress;

            Console.WriteLine($"DEBUG: {product.Naam}, elapsed={elapsedSeconds:F2}s, progress={progress:F2}, huidigePrijs={currentPrice:F2}");
            return Math.Round(currentPrice, 2);
        }


        // =========================
        // HELPER METHOD
        // =========================
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
                    : DateOnly.FromDateTime(DateTime.UtcNow),
                Potmaat = p.Potmaat,
                Gewicht = p.Gewicht,
                Steellengte = p.Steellengte ?? 0,
                Hoeveelheid = p.Hoeveelheid,
                MinimalePrijs = p.MinimalePrijs
            };
        }
    }

    // =========================
    // DTO CLASSES
    // =========================
    public class VeilingStatusDto
    {
        public bool IsActief { get; set; }
        public bool IsInPauze { get; set; }
        public int RemainingSeconds { get; set; }
        public ProductVeilingDto? HuidigProduct { get; set; }
        public ProductVeilingDto? VolgendProduct { get; set; }
        public int AantalInWachtrij { get; set; }
    }

    public class ProductVeilingDto
    {
        public int Id { get; set; }
        public string Naam { get; set; } = null!;
        public string? Foto { get; set; }
        public string? Beschrijving { get; set; }
        public int Hoeveelheid { get; set; }
        public int MinimalePrijs { get; set; }
        public double HuidigePrijs { get; set; }
    }
}
