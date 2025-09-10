using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleHubApi.Data;
using StyleHubApi.models;
using StyleHubApi.Models;
using StyleHubApi.Models.DTO;
using System.Security.Claims;

namespace StyleHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AddressesController(AppDbContext context) => _context = context;

        // Helper: هات اليوزر من الهيدر أو الكويري (زي الكارت)

         private string? GetUserId()=>
         
            Request.Headers.TryGetValue("X-User-Id", out var h) ? h.ToString()
             : (Request.Query.TryGetValue("userId", out var q) ? q.ToString() : null);

        

        // GET: api/addresses/mine
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<Address>>> GetMyAddresses()
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");

            return await _context.Addresses
                .Where(a => a.UserId == uid)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
        }

        // POST: api/addresses/mine
        [HttpPost("mine")]
        public async Task<ActionResult<Address>> AddAddress([FromBody] AddressDto dto)
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");
            ModelState.Remove("UserId");           // ← أهم سطر
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var firstForUser = !await _context.Addresses.AnyAsync(a => a.UserId == uid);

            var address = new Address
            {
                UserId = uid,
                Label = dto.Label,
                Line1 = dto.Line1,
                Line2 = dto.Line2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                ContactName = dto.ContactName,
                Phone = dto.Phone,
                IsDefault = firstForUser // أول عنوان يكون Default
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyAddresses), new { id = address.Id }, address);
        }

        // PUT: api/addresses/mine/{id}
        [HttpPut("mine/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto dto)
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == uid);
            if (address == null) return NotFound();

            address.Label = dto.Label;
            address.Line1 = dto.Line1;
            address.Line2 = dto.Line2;
            address.City = dto.City;
            address.State = dto.State;
            address.PostalCode = dto.PostalCode;
            address.Country = dto.Country;
            address.ContactName = dto.ContactName;
            address.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/addresses/mine/{id}
        [HttpDelete("mine/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == uid);
            if (address == null) return NotFound();

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/addresses/mine/{id}/default
        [HttpPost("mine/{id}/default")]
        public async Task<IActionResult> MakeDefault(int id)
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");

            var addresses = await _context.Addresses.Where(a => a.UserId == uid).ToListAsync();
            if (!addresses.Any(a => a.Id == id)) return NotFound();

            foreach (var a in addresses) a.IsDefault = (a.Id == id);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/addresses/mine/clear
        [HttpPost("mine/clear")]
        public async Task<IActionResult> ClearAddresses()
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");

            var addresses = await _context.Addresses.Where(a => a.UserId == uid).ToListAsync();
            if (!addresses.Any()) return NotFound("No addresses found");

            _context.Addresses.RemoveRange(addresses);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
