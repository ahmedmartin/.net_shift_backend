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
    public class RolesController : ControllerBase
    {
        private readonly shieftContext _context;

        public RolesController(shieftContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet("{admin_id}")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles(int admin_id)
        {
            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");
            return await _context.Roles.ToListAsync();
        }

        // GET: api/Roles/5
        [HttpGet("{id},{admin_id}")]
        public async Task<ActionResult<Role>> GetRole(int id, int admin_id)
        {
            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");

            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound("Role Not Found");
            }

            return Ok(role);
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        [HttpPut("{admin_id}")]
        public async Task<IActionResult> PutRole(int admin_id, Role role)
        {
            //if (id != role.RoleId)
            //{
            //    return BadRequest();
            //}

            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!RoleExists(id))
                //{
                return NotFound("Role Not Found");
                //}
                //else
                //{
                //throw;
                //}
            }

            return Ok(role);
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{admin_id}")]
        public async Task<ActionResult<Role>> PostRole(int admin_id, Role role)
        {
            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return Ok(role);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id},{admin_id}")]
        public async Task<IActionResult> DeleteRole(int id, int admin_id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound("Role Not Found");
            }
            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok("Role Deleted");
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

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}
