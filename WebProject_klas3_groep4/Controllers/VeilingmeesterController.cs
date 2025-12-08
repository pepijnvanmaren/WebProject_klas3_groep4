using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;

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

        // GET ALL
        [HttpGet]
        public ActionResult<IEnumerable<VeilingmeesterOutputDto>> GetVeilingmeesters()
        {
            var veilingmeesters = _context.Gebruikers
                .Where(u => u.Rol == "Veilingmeester")
                .Include(u => u.Veilingen)
                .Select(u => new VeilingmeesterOutputDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    VeilingVestiging = u.VeilingVestiging,
                    Rol = "Veilingmeester"
                })
                .ToList();
            return Ok(veilingmeesters);
        }

        // GET SINGLE
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VeilingmeesterOutputDto>> GetVeilingmeester(int id)
        {
            var veilingmeester = await _context.Gebruikers
                .Include(u => u.Veilingen)
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Veilingmeester");

            if (veilingmeester == null)
                return NotFound();

            var dto = new VeilingmeesterOutputDto
            {
                Id = veilingmeester.Id,
                UserName = veilingmeester.UserName,
                Email = veilingmeester.Email,
                PhoneNumber = veilingmeester.PhoneNumber,
                VeilingVestiging = veilingmeester.VeilingVestiging,
                Rol = "Veilingmeester"
            };

            return Ok(dto);
        }

        // LOGIN
        [HttpGet("login")]
        public async Task<ActionResult<VeilingmeesterOutputDto>> Login([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.Rol != "Veilingmeester")
                return NotFound();

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid)
                return Unauthorized();

            var dto = new VeilingmeesterOutputDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                VeilingVestiging = user.VeilingVestiging,
                Rol = "Veilingmeester"
            };

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        public async Task<ActionResult<VeilingmeesterOutputDto>> PostVeilingmeester([FromBody] VeilingmeesterCreateDto dto)
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

            var outDto = new VeilingmeesterOutputDto
            {
                Id = veilingmeester.Id,
                UserName = veilingmeester.UserName,
                Email = veilingmeester.Email,
                PhoneNumber = veilingmeester.PhoneNumber,
                VeilingVestiging = veilingmeester.VeilingVestiging,
                Rol = "Veilingmeester"
            };

            return CreatedAtAction(nameof(GetVeilingmeester), new { id = veilingmeester.Id }, outDto);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<ActionResult<VeilingmeesterOutputDto>> PutVeilingmeester(int id, [FromBody] VeilingmeesterUpdateDto dto)
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

            var outDto = new VeilingmeesterOutputDto
            {
                Id = veilingmeester.Id,
                UserName = veilingmeester.UserName,
                Email = veilingmeester.Email,
                PhoneNumber = veilingmeester.PhoneNumber,
                VeilingVestiging = veilingmeester.VeilingVestiging,
                Rol = "Veilingmeester"
            };

            return Ok(outDto);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVeilingmeester(int id)
        {
            var veilingmeester = await _context.Gebruikers
                .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "veilingmeester");

            if (veilingmeester == null)
                return NotFound();

            await _userManager.DeleteAsync(veilingmeester);
            return NoContent();
        }
    }
}
