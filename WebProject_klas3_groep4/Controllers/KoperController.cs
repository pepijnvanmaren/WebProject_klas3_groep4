using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KoperController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public KoperController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GebruikerDB>> GetKopers()
        {
            var kopers = _context.Gebruikers
                .Where(u => u.Rol == "Koper")
                .ToList();
            return Ok(kopers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GebruikerDB>> GetKoper(int id)
        {
            var koper = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Koper");

            if (koper == null)
                return NotFound();

            return Ok(koper);
        }

        [HttpGet("login")]
        public async Task<ActionResult<GebruikerDB>> Login([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.Rol != "Koper")
                return NotFound();

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid)
                return Unauthorized();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<GebruikerDB>> PostKoper([FromBody] KoperCreateDto dto)
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

            return CreatedAtAction(nameof(GetKoper), new { id = koper.Id }, koper);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GebruikerDB>> PutKoper(int id, [FromBody] KoperUpdateDto dto)
        {
            var koper = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Koper");

            if (koper == null)
                return NotFound();

            koper.UserName = dto.UserName;
            koper.Email = dto.Email;
            koper.PhoneNumber = dto.PhoneNumber;
            koper.BankGegevens = dto.BankGegevens;
            koper.Adres = dto.Adres;
            koper.Postcode = dto.Postcode;

            await _userManager.UpdateAsync(koper);
            return Ok(koper);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteKoper(int id)
        {
            var koper = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Koper");

            if (koper == null)
                return NotFound();

            await _userManager.DeleteAsync(koper);
            return NoContent();
        }
    }

    public class KoperCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? BankGegevens { get; set; }
        public string? Adres { get; set; }
        public string? Postcode { get; set; }
    }

    public class KoperUpdateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? BankGegevens { get; set; }
        public string? Adres { get; set; }
        public string? Postcode { get; set; }
    }
}