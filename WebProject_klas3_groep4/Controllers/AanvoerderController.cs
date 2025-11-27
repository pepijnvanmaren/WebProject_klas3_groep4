using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.Controllers
{
    [ApiController]
    [Route("api/aanvoerders")]
    public class AanvoerderController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<GebruikerDB> _userManager;

        public AanvoerderController(DatabaseContext context, UserManager<GebruikerDB> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GebruikerDto>>> GetAanvoerders()
        {
            var aanvoerders = await _context.Gebruikers
                .Where(u => u.Rol == "Aanvoerder")
                .Select(u => new GebruikerDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Rol = u.Rol
                })
                .ToListAsync();

            return Ok(aanvoerders);
        }

        // ------------------------------------------------------------
        // GET SINGLE
        // ------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GebruikerDto>> GetAanvoerder(int id)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "aanvoerder")
                return NotFound();

            var dto = new GebruikerDto
            {
                Id = aanvoerder.Id,
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Rol = aanvoerder.Rol
            };

            return Ok(dto);
        }

        // ------------------------------------------------------------
        // CREATE
        // ------------------------------------------------------------
       [ Authorize(Roles = "Aanvoerder")]
        [HttpPost]
        public async Task<ActionResult<GebruikerDto>> PostAanvoerder([FromBody] AanvoerderCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var aanvoerder = new GebruikerDB
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Rol = "Aanvoerder",
                Adres = dto.Adres,
                Postcode = dto.Postcode
            };

            var result = await _userManager.CreateAsync(aanvoerder, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var outDto = new GebruikerDto
            {
                Id = aanvoerder.Id,
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Rol = aanvoerder.Rol
            };

            return CreatedAtAction(nameof(GetAanvoerder), new { id = aanvoerder.Id }, outDto);
        }

        // ------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult<GebruikerDto>> PutAanvoerder(int id, [FromBody] AanvoerderUpdateDto dto)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "Aanvoerder")
                return NotFound();

            aanvoerder.UserName = dto.UserName;
            aanvoerder.Email = dto.Email;
            aanvoerder.PhoneNumber = dto.PhoneNumber;
            aanvoerder.Adres = dto.Adres;
            aanvoerder.Postcode = dto.Postcode;

            var result = await _userManager.UpdateAsync(aanvoerder);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var returnDto = new GebruikerDto
            {
                Id = aanvoerder.Id,
                UserName = aanvoerder.UserName,
                Email = aanvoerder.Email,
                PhoneNumber = aanvoerder.PhoneNumber,
                Rol = aanvoerder.Rol
            };

            return Ok(returnDto);
        }

        // ------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAanvoerder(int id)
        {
            var aanvoerder = await _userManager.FindByIdAsync(id.ToString());

            if (aanvoerder == null || aanvoerder.Rol != "Aanvoerder")
                return NotFound();

            await _userManager.DeleteAsync(aanvoerder);

            return NoContent();
        }
    }
}