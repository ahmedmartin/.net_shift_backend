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
    [Route("api/[controller]")]
    [ApiController]
    public class ShieftsController : ControllerBase
    {
        private readonly shieftContext _context;

        public ShieftsController(shieftContext context)
        {
            _context = context;
        }

        // GET: api/Shiefts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shieft>>> GetShiefts()
        {
            return await _context.Shiefts.ToListAsync();
        }

        // GET: api/Shiefts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Shieft>> GetShieft(int id)
        {
            var shieft = await _context.Shiefts.FindAsync(id);

            if (shieft == null)
            {
                return NotFound("shift Not Found");
            }

            return shieft;
        }

        [HttpGet("allusershifts/{user_id}")]
        public  IActionResult Get_Shieft_user(int user_id)
        {
            //----search if userid found 
            var userr = _context.Users as IQueryable<User>;
            userr = userr.Where(u => u.UserId == user_id);
            if(userr.Count<User>() == 0)  return NotFound("User Not Found");

            //----show shift results
            var shift = _context.Shiefts as IQueryable<Shieft>;
            shift = shift.Where(s => s.UserId == user_id);
            return Ok(shift);
        }

        [Authorize(Roles = "manger")]
        [HttpGet("allmangershifts/{manger_id}")]
        public IActionResult Get_Shieft_manger(int manger_id)
        {
            //----search if userid found 
            var userr = _context.Users as IQueryable<User>;
            userr = userr.Where(u => u.UserId == manger_id);
            if (userr.Count<User>() == 0) return NotFound("User Not Found");

            var shift = _context.Shiefts as IQueryable<Shieft>;
            shift = shift.Where(s => s.AdminId == manger_id);
            return Ok(shift);
        }

        // PUT: api/Shiefts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutShieft(Shieft shieft)
        {
            //if (id != shieft.ShiftId)
            //{
            //    return BadRequest();
            //}

            if (!await user_id_foundAsync(shieft)) return NotFound("User not Found");

            if (!await check_user_role_foundAsync(shieft.AdminId)) return NotFound("Not Admin");

            if (!await shieft_type_id_foundAsync(shieft)) return NotFound("Shift type not found");

            _context.Entry(shieft).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!ShieftExists(id))
                //{
                return NotFound("Shieft Not Found");
                //}
                //else
                //{
                //throw;
                //}
            }

            return Ok(shieft);
        }

        // POST: api/Shiefts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Shieft>> PostShieft(Shieft shieft)
        {

            if (!await user_id_foundAsync(shieft)) return NotFound("User not Found");

            if (!await check_user_role_foundAsync(shieft.AdminId)) return NotFound("Not Department Admin");

            if (!await shieft_type_id_foundAsync(shieft)) return NotFound("Shift type not found");

            _context.Shiefts.Add(shieft);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShieft", new { id = shieft.ShiftId }, shieft);
        }

        // DELETE: api/Shiefts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShieft(int id)
        {




            var shieft = await _context.Shiefts.FindAsync(id);
            if (shieft == null)
            {
                return NotFound("Shieft Not Found");
            }

            if (!await check_user_role_foundAsync(shieft.AdminId)) return NotFound("Not Department manger");

            _context.Shiefts.Remove(shieft);
            await _context.SaveChangesAsync();

            return Ok("Shift Deleted");
        }

        private async Task<bool> user_id_foundAsync(Shieft shieft)
        {
            var user = await _context.Users.FindAsync(shieft.UserId);
            return user != null;
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
                    if (role == "manger")
                    {
                        var department = await _context.Departments.FindAsync(user.DeptId);
                        if (department != null)
                        {
                            if (department.MangerId == user.UserId) return true;
                        }
                    }
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

        private async Task<bool> shieft_type_id_foundAsync(Shieft shieft)
        {
            var shift_type = await _context.ShiftTypes.FindAsync(shieft.ShiftTypeId);
            return shift_type != null;
        }



        private bool ShieftExists(int id)
        {
            return _context.Shiefts.Any(e => e.ShiftId == id);
        }
    }
}
