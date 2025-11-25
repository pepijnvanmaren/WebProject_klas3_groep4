using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingmeesterController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public VeilingmeesterController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GebruikerDB>> GetVeilingmeesters()
        {
            var veilingmeesters = _context.Gebruikers
                .Where(u => u.Rol == "Veilingmeester")
                .Include(u => u.Veilingen)
                .ToList();
            return Ok(veilingmeesters);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GebruikerDB>> GetVeilingmeester(int id)
        {
            var veilingmeester = await _context.Gebruikers
                .Include(u => u.Veilingen)
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Veilingmeester");

            if (veilingmeester == null)
                return NotFound();

            return Ok(veilingmeester);
        }

        [HttpGet("login")]
        public async Task<ActionResult<GebruikerDB>> Login([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.Rol != "Veilingmeester")
                return NotFound();

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid)
                return Unauthorized();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<GebruikerDB>> PostVeilingmeester([FromBody] VeilingmeesterCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var veilingmeester = new GebruikerDB
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Rol = "Veilingmeester",
                VeilingVestiging = dto.VeilingVestiging
            };

            var result = await _userManager.CreateAsync(veilingmeester, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetVeilingmeester), new { id = veilingmeester.Id }, veilingmeester);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GebruikerDB>> PutVeilingmeester(int id, [FromBody] VeilingmeesterUpdateDto dto)
        {
            var veilingmeester = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Veilingmeester");

            if (veilingmeester == null)
                return NotFound();

            veilingmeester.UserName = dto.UserName;
            veilingmeester.Email = dto.Email;
            veilingmeester.PhoneNumber = dto.PhoneNumber;
            veilingmeester.VeilingVestiging = dto.VeilingVestiging;

            await _userManager.UpdateAsync(veilingmeester);
            return Ok(veilingmeester);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVeilingmeester(int id)
        {
            var veilingmeester = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Veilingmeester");

            if (veilingmeester == null)
                return NotFound();

            await _userManager.DeleteAsync(veilingmeester);
            return NoContent();
        }
    }

    public class VeilingmeesterCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? VeilingVestiging { get; set; }
    }

    public class VeilingmeesterUpdateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? VeilingVestiging { get; set; }
    }
}