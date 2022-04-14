#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sheift.Models;

namespace sheift.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftTypesController : ControllerBase
    {
        private readonly shieftContext _context;

        public ShiftTypesController(shieftContext context)
        {
            _context = context;
        }

        // GET: api/ShiftTypes
        [AllowAnonymous]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ShiftType>>> GetShiftTypes()
        {
           // if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");
            return await _context.ShiftTypes.ToListAsync();
        }

        // GET: api/ShiftTypes/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftType>> GetShiftType( int id)
        {

            var shiftType = await _context.ShiftTypes.FindAsync(id);

            if (shiftType == null)
            {
                return NotFound("Shift Type Not Found");
            }

            return shiftType;
        }

        // PUT: api/ShiftTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{admin_id}")]
        public async Task<IActionResult> PutShiftType(int admin_id, ShiftType shiftType)
        {
            //if (id != shiftType.ShiftTypeId)
            //{
            //    return BadRequest();
            //}
            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");
            _context.Entry(shiftType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(shiftType);
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!ShiftTypeExists(id))
                //{
                return NotFound("Shift Type Not Found");
                //}
                //else
                //{
                //    throw;
                //}
            }

            return NoContent();
        }

        // POST: api/ShiftTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        [HttpPost("{admin_id}")]
        public async Task<ActionResult<ShiftType>> PostShiftType(int admin_id, ShiftType shiftType)
        {
            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");

            _context.ShiftTypes.Add(shiftType);
            await _context.SaveChangesAsync();
            return Ok(shiftType);

        }

        // DELETE: api/ShiftTypes/5
        [HttpDelete("{id},{admin_id}")]
        public async Task<IActionResult> DeleteShiftType(int id, int admin_id)
        {
            var shiftType = await _context.ShiftTypes.FindAsync(id);
            if (shiftType == null)
            {
                return NotFound();
            }
            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");
            _context.ShiftTypes.Remove(shiftType);
            await _context.SaveChangesAsync();

            return Ok("Shift Type Deleted");
        }

        private async Task<bool> check_user_role_foundAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                var role = await get_role_foundAsync(user);
                if (role != null)
                {
                    if (role == "admin") return true;
                }


            }
            return false;
        }

        private async Task<String> get_role_foundAsync(User user)
        {
            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role != null)
            {
                return role.RoleName;
            }
            return null;
        }

        private bool ShiftTypeExists(int id)
        {
            return _context.ShiftTypes.Any(e => e.ShiftTypeId == id);
        }
    }
}
