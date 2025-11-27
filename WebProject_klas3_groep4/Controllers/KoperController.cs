using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/kopers")]
    public class KoperController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public KoperController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GebruikerDto>>> GetKopers()
        {
            var kopers = await _context.Gebruikers
                .Where(u => u.Rol == "Koper")
                .Select(u => new GebruikerDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Rol = u.Rol
                })
                .ToListAsync();

            return Ok(kopers);
        }

        // ------------------------------------------------------------
        // GET SINGLE
        // ------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GebruikerDto>> GetKoper(int id)
        {
            var koper = await _userManager.FindByIdAsync(id.ToString());

            if (koper == null || koper.Rol != "Koper")
                return NotFound();

            var dto = new GebruikerDto
            {
                Id = koper.Id,
                UserName = koper.UserName,
                Email = koper.Email,
                PhoneNumber = koper.PhoneNumber,
                Rol = koper.Rol
            };

            return Ok(dto);
        }

        // ------------------------------------------------------------
        // CREATE
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<GebruikerDto>> PostKoper([FromBody] KoperCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var koper = new GebruikerDB
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Rol = "Koper",
                BankGegevens = dto.BankGegevens,
                Adres = dto.Adres,
                Postcode = dto.Postcode
            };

            var result = await _userManager.CreateAsync(koper, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var outDto = new GebruikerDto
            {
                Id = koper.Id,
                UserName = koper.UserName,
                Email = koper.Email,
                PhoneNumber = koper.PhoneNumber,
                Rol = koper.Rol
            };

            return CreatedAtAction(nameof(GetKoper), new { id = koper.Id }, outDto);
        }

        // ------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult<GebruikerDto>> PutKoper(int id, [FromBody] KoperUpdateDto dto)
        {
            var koper = await _userManager.FindByIdAsync(id.ToString());

            if (koper == null || koper.Rol != "Koper")
                return NotFound();

            koper.UserName = dto.UserName;
            koper.Email = dto.Email;
            koper.PhoneNumber = dto.PhoneNumber;
            koper.BankGegevens = dto.BankGegevens;
            koper.Adres = dto.Adres;
            koper.Postcode = dto.Postcode;

            var result = await _userManager.UpdateAsync(koper);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var returnDto = new GebruikerDto
            {
                Id = koper.Id,
                UserName = koper.UserName,
                Email = koper.Email,
                PhoneNumber = koper.PhoneNumber,
                Rol = koper.Rol
            };

            return Ok(returnDto);
        }

        // ------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteKoper(int id)
        {
            var koper = await _userManager.FindByIdAsync(id.ToString());

            if (koper == null || koper.Rol != "Koper")
                return NotFound();

            await _userManager.DeleteAsync(koper);

            return NoContent();
        }
    }
}