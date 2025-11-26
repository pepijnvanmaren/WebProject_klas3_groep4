using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/kopers")]
    [Authorize] // geen Bearer
    public class KoperController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public KoperController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public async Task<ActionResult<GebruikerDto>> Login([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.Rol != "Koper")
                return NotFound();

            var valid = await _userManager.CheckPasswordAsync(user, password);
            if (!valid)
                return Unauthorized();

            // Login via Identity cookies
            var signInManager = HttpContext.RequestServices.GetRequiredService<SignInManager<GebruikerDB>>();
            await signInManager.SignInAsync(user, isPersistent: true);

            return Ok(new GebruikerDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Rol = user.Rol
            });
        }

        // Overige endpoints blijven hetzelfde
    }
}
