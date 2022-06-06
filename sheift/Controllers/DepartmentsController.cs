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
    public class DepartmentsController : ControllerBase
    {
        private readonly shieftContext _context;

        public DepartmentsController(shieftContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        //[HttpGet("{admin_id}")]
        public async Task<ActionResult<IEnumerable<DepartmentsDataWithmanger>>> GetDepartments()
        {
            //if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");

            return  _context.DepartmentsDataWithmangers;
        }

        [HttpGet("department_manger")]
        //[HttpGet("{admin_id}")]
        public async Task<ActionResult<IEnumerable<DepartmentManger>>> GetDepartments_manger()
        {
            //if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");

            return await _context.DepartmentMangers.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("manger_departments/{manger_id}")]
        //[HttpGet("{id},{admin_id}")]
        public async Task<ActionResult<IEnumerable<DepartmentsDataWithmanger>>> GetDepartment(int manger_id)
        {

            var shift_details = await _context.DepartmentsDataWithmangers.Where(s => s.MangerId == manger_id).OrderBy(d => d.DepName).ToListAsync();

            return Ok(shift_details);
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [HttpPut("{admin_id}")]
        public async Task<IActionResult> PutDepartment(int admin_id, Department department)
        {
            

            if (!await check_user_role_foundAsync(admin_id))
            { return NotFound("Not Admin"); }

            //if (!await check_user_manger_foundAsync(department., department.DepId, true))
            //{ return NotFound("Not manger or Not Include This Department"); }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(department);
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!DepartmentExists(id))
                //{
                return NotFound("Department Not Found");
                //}
                //else
                //{
                //    throw;
                //}
            }

            return Ok(department);
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [HttpPut("edita_department_manger/{admin_id}")]
        public async Task<IActionResult> PutDepartment_manger(int admin_id, DepartmentManger department)
        {


            if (!await check_user_role_foundAsync(admin_id))
            { return NotFound("Not Admin"); }

            //if (!await check_user_manger_foundAsync(department., department.DepId, true))
            //{ return NotFound("Not manger or Not Include This Department"); }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(department);
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!DepartmentExists(id))
                //{
                return NotFound("Department Not Found");
                //}
                //else
                //{
                //    throw;
                //}
            }

            return Ok(department);
        }

        // POST: api/Departments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "admin")]
        [HttpPost("add_department/{admin_id}")]
        public async Task<ActionResult<Department>> PostDepartment(int admin_id, Department department)
        {

            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.DepId }, department);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("add_department_manger/{admin_id}")]
        public async Task<ActionResult<DepartmentManger>> PostDepartment_manger(int admin_id, DepartmentManger department)
        {

            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");
            //if (!await check_user_manger_foundAsync(department.MangerId, department.DepId, false)) return NotFound("Not manger");
            _context.DepartmentMangers.Add(department);
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetDepartment", new { id = department.DepId }, department);
        }

        // DELETE: api/Departments/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id},{admin_id}")]
        public async Task<IActionResult> DeleteDepartment(int id, int admin_id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound("Department Not Found");
            }
            if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return Ok("Department Deleted");
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

        private async Task<bool> check_user_manger_foundAsync(int id, int dep_id, bool update)
        {
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                var role = await get_role_foundAsync(user);
                // when you create department not recomended to check if manger in the department or not 
                //when you update department should check if manger in department or not
                if (update)
                {
                    if (role != null && (role == "manger" || role == "admin") && user.DeptId == dep_id) return true;
                }
                else
                   if (role != null && (role == "manger" || role == "admin")) return true;
            }
            return false;
        }

        //private async Task<bool> check_user_department_foundAsync(int id, int dep_id)
        //{
        //    var user = await _context.Users.FindAsync(id);

        //    if (user != null)
        //    {
        //      if (user.DeptId == dep_id) return true;
        //    }
        //    return false;
        //}

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepId == id);
        }
    }
}
