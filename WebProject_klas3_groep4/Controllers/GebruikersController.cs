using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class GebruikersController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public GebruikersController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GebruikerDB>> GetGebruikers()
        {
            return Ok(_context.Gebruikers.ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GebruikerDB>> GetGebruiker(int id)
        {
            var gebruiker = await _userManager.FindByIdAsync(id.ToString());
            if (gebruiker == null)
                return NotFound();
            return Ok(gebruiker);
        }

        [HttpPost]
        public async Task<ActionResult<GebruikerDB>> PostGebruiker([FromBody] GebruikerCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var gebruiker = new GebruikerDB
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Rol = dto.Rol ?? "Gebruiker"
            };

            var result = await _userManager.CreateAsync(gebruiker, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetGebruiker), new { id = gebruiker.Id }, gebruiker);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GebruikerDB>> PutGebruiker(int id, [FromBody] GebruikerUpdateDto dto)
        {
            var gebruiker = await _userManager.FindByIdAsync(id.ToString());
            if (gebruiker == null)
                return NotFound();

            gebruiker.UserName = dto.UserName;
            gebruiker.Email = dto.Email;
            gebruiker.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(gebruiker);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Als password moet worden gewijzigd
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(gebruiker);
                await _userManager.ResetPasswordAsync(gebruiker, token, dto.NewPassword);
            }

            return Ok(gebruiker);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGebruiker(int id)
        {
            var gebruiker = await _userManager.FindByIdAsync(id.ToString());
            if (gebruiker == null)
                return NotFound();

            await _userManager.DeleteAsync(gebruiker);
            return NoContent();
        }
    }

    // DTOs voor veilige data transfer
    public class GebruikerCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? Rol { get; set; }
    }

    public class GebruikerUpdateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? NewPassword { get; set; }
    }
}