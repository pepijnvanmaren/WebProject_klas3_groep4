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
    public class VeilingController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public VeilingController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------------------------------------------------------
        // GET ALL
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VeilingOutputDto>>> GetVeilingen()
        {
            var veilingen = await _context.Veilingen
                .Include(v => v.Veilingmeester)
                .Include(v => v.Producten)
                .Select(v => new VeilingOutputDto
                {
                    Id = v.ID,
                    StarTijd = v.StarTijd,
                    StartDatum = v.StartDatum,
                    AantalProducten = v.AantalProducten,
                    KlokLocatie = v.KlokLocatie,
                    HuidigeSituatieVanVeiling = v.HuidigeSituatieVanVeiling,
                    Bechrijving = v.Bechrijving,
                    VeilingmeesterId = v.VeilingmeesterId,
                    VeilingmeesterNaam = v.Veilingmeester != null ? v.Veilingmeester.UserName : null
                })
                .ToListAsync();

            return Ok(veilingen);
        }

        // ---------------------------------------------------------
        // GET SINGLE
        // ---------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VeilingOutputDto>> GetVeiling(int id)
        {
            var veiling = await _context.Veilingen
                .Include(v => v.Veilingmeester)
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanvoerder)
                .FirstOrDefaultAsync(v => v.ID == id);

            if (veiling == null)
                return NotFound("Veiling niet gevonden");

            var dto = new VeilingOutputDto
            {
                Id = veiling.ID,
                StarTijd = veiling.StarTijd,
                StartDatum = veiling.StartDatum,
                AantalProducten = veiling.AantalProducten,
                KlokLocatie = veiling.KlokLocatie,
                HuidigeSituatieVanVeiling = veiling.HuidigeSituatieVanVeiling,
                Bechrijving = veiling.Bechrijving,
                VeilingmeesterId = veiling.VeilingmeesterId,
                VeilingmeesterNaam = veiling.Veilingmeester != null ? veiling.Veilingmeester.UserName : null,
                Producten = veiling.Producten?.Select(p => new ProductOutputDto
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
                    MinimalePrijs = p.MinimalePrijs,
                    AanvoerderId = p.AanvoerderId,
                    AanvoerderNaam = p.Aanvoerder != null ? p.Aanvoerder.UserName : null,
                    VeilingId = p.VeilingId
                }).ToList()
            };

            return Ok(dto);
        }

        // ---------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<VeilingOutputDto>> PostVeiling([FromBody] VeilingCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid veiling data");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("Gebruiker niet gevonden");

            var veiling = new VeilingDB
            {
                StarTijd = dto.StarTijd ?? DateTime.Now.ToString("HH:mm"),
                StartDatum = dto.StartDatum ?? DateTime.Now.ToString("yyyy-MM-dd"),
                AantalProducten = dto.AantalProducten,
                KlokLocatie = dto.KlokLocatie,
                HuidigeSituatieVanVeiling = dto.HuidigeSituatieVanVeiling,
                Bechrijving = dto.Bechrijving,
                VeilingmeesterId = dto.VeilingmeesterId ?? user.Id
            };

            _context.Veilingen.Add(veiling);
            await _context.SaveChangesAsync();

            var outDto = new VeilingOutputDto
            {
                Id = veiling.ID,
                StarTijd = veiling.StarTijd,
                StartDatum = veiling.StartDatum,
                AantalProducten = veiling.AantalProducten,
                KlokLocatie = veiling.KlokLocatie,
                HuidigeSituatieVanVeiling = veiling.HuidigeSituatieVanVeiling,
                Bechrijving = veiling.Bechrijving,
                VeilingmeesterId = veiling.VeilingmeesterId,
                VeilingmeesterNaam = user.UserName
            };

            return CreatedAtAction(nameof(GetVeiling), new { id = veiling.ID }, outDto);
        }

        // ---------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------
        [Authorize(Roles = "Veilingmeester")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<VeilingOutputDto>> PutVeiling(int id, [FromBody] VeilingUpdateDto dto)
        {
            var veiling = await _context.Veilingen.FindAsync(id);
            if (veiling == null)
                return NotFound("Veiling niet gevonden");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Alleen de eigenaar kan aanpassen
            if (veiling.VeilingmeesterId != user.Id)
                return Forbid("Je kunt deze veiling niet bewerken");

            if (dto.StarTijd != null) veiling.StarTijd = dto.StarTijd;
            if (dto.StartDatum != null) veiling.StartDatum = dto.StartDatum;
            if (dto.AantalProducten > 0) veiling.AantalProducten = dto.AantalProducten.Value;
            if (dto.KlokLocatie != null) veiling.KlokLocatie = dto.KlokLocatie;
            if (dto.HuidigeSituatieVanVeiling != null) veiling.HuidigeSituatieVanVeiling = dto.HuidigeSituatieVanVeiling;
            if (dto.Bechrijving != null) veiling.Bechrijving = dto.Bechrijving;
            if (dto.VeilingmeesterId.HasValue) veiling.VeilingmeesterId = dto.VeilingmeesterId.Value;

            await _context.SaveChangesAsync();

            return Ok("Veiling bijgewerkt");
        }

        // ---------------------------------------------------------
        // DELETE
        // ---------------------------------------------------------
      //  [Authorize(Roles = "Veilingmeester")]
        [HttpDelete("{id:String}")]
        public async Task<ActionResult> DeleteVeiling(int id)
        {
            var veiling = await _context.Veilingen.FindAsync(id);
            if (veiling == null)
                return NotFound("Veiling niet gevonden");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Alleen de eigenaar kan verwijderen
            if (veiling.VeilingmeesterId != user.Id)
                return Forbid("Je kunt deze veiling niet verwijderen");

            _context.Veilingen.Remove(veiling);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ---------------------------------------------------------
        // GET VEILINGEN BY VEILINGMEESTER
        // ---------------------------------------------------------
        [HttpGet("veilingmeester/{veilingmeesterId:int}")]
        public async Task<ActionResult<IEnumerable<VeilingOutputDto>>> GetVeilingenByVeilingmeester(int veilingmeesterId)
        {
            var veilingen = await _context.Veilingen
                .Where(v => v.VeilingmeesterId == veilingmeesterId)
                .Include(v => v.Veilingmeester)
                .Include(v => v.Producten)
                .Select(v => new VeilingOutputDto
                {
                    Id = v.ID,
                    StarTijd = v.StarTijd,
                    StartDatum = v.StartDatum,
                    AantalProducten = v.AantalProducten,
                    KlokLocatie = v.KlokLocatie,
                    HuidigeSituatieVanVeiling = v.HuidigeSituatieVanVeiling,
                    Bechrijving = v.Bechrijving,
                    VeilingmeesterId = v.VeilingmeesterId,
                    VeilingmeesterNaam = v.Veilingmeester != null ? v.Veilingmeester.UserName : null
                })
                .ToListAsync();

            return Ok(veilingen);
        }
    }
}