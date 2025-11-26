using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/gebruikers")]
    public class GebruikersController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public GebruikersController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GebruikerDto>>> GetGebruikers()
        {
            var gebruikers = await _context.Gebruikers
                .Select(g => new GebruikerDto
                {
                    Id = g.Id,
                    UserName = g.UserName,
                    Email = g.Email,
                    PhoneNumber = g.PhoneNumber,
                    Rol = g.Rol
                })
                .ToListAsync();

            return Ok(gebruikers);
        }

        // ------------------------------------------------------------
        // GET SINGLE
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<GebruikerDto>> GetGebruiker(int id)
        {
            var gebruiker = await _userManager.FindByIdAsync(id.ToString());
            if (gebruiker == null)
                return NotFound();

            var dto = new GebruikerDto
            {
                Id = gebruiker.Id,
                UserName = gebruiker.UserName,
                Email = gebruiker.Email,
                PhoneNumber = gebruiker.PhoneNumber,
                Rol = gebruiker.Rol
            };

            return Ok(dto);
        }

        // ------------------------------------------------------------
        // CREATE
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<GebruikerDto>> PostGebruiker([FromBody] GebruikerCreateDto dto)
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

            var returnDto = new GebruikerDto
            {
                Id = gebruiker.Id,
                UserName = gebruiker.UserName,
                Email = gebruiker.Email,
                PhoneNumber = gebruiker.PhoneNumber,
                Rol = gebruiker.Rol
            };

            return CreatedAtAction(nameof(GetGebruiker), new { id = gebruiker.Id }, returnDto);
        }

        // ------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<GebruikerDto>> PutGebruiker(int id, [FromBody] GebruikerUpdateDto dto)
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

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(gebruiker);
                var passwordResult = await _userManager.ResetPasswordAsync(gebruiker, token, dto.NewPassword);
                if (!passwordResult.Succeeded)
                    return BadRequest(passwordResult.Errors);
            }

            var returnDto = new GebruikerDto
            {
                Id = gebruiker.Id,
                UserName = gebruiker.UserName,
                Email = gebruiker.Email,
                PhoneNumber = gebruiker.PhoneNumber,
                Rol = gebruiker.Rol
            };

            return Ok(returnDto);
        }

        // ------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------
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
}
