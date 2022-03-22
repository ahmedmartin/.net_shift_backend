#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sheift.Models;
using BC = BCrypt.Net.BCrypt;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace sheift.Controllers
{
    //[Authorize(Roles ="admin")] // ----- autherized by role---------
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly shieftContext _context;

        public UsersController(shieftContext context)
        {
            _context = context;
        }

        // GET: api/Users
        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            for (int i = 0; i < users.Count; i++) {
                users[i].Password = "";
            }
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User Not Found");
            }

            var jsonn = JsonConvert.SerializeObject(user);
            var dictionaryy = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonn);
            dictionaryy.Remove("Password");

            return Ok(dictionaryy);
        }

        //----------filter data-----------
        //[HttpGet("user")]
        //public IActionResult GetUser_byname(string user_name)
        //{

        //    if (string.IsNullOrWhiteSpace(user_name))
        //    {
        //        var user = _context.Users.ToList();
        //        for (int i = 0; i < user.Count; i++)
        //        {
        //            user[i].Password = "";
        //        }
        //        return Ok(user);
        //    }
        //    else
        //    {
        //var userr = _context.Users as IQueryable<User>;

        //userr = userr.Where(u => u.UserName == user_name);

        //        return Ok(userr);
        //    }   
        //}

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutUser(User user)
        {
            //if (id != user.UserId)
            //{
            //    return BadRequest("user not found");
            //}

            if (!await department_id_foundAsync(user))
            {
                return NotFound("Department not found");
            }


            if (!await role_id_foundAsync(user))
            {
                return NotFound("Role not found");
            }

            String dp_pass = BC.HashPassword(user.Password);
            user.Password = dp_pass;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!UserExists(id))
                //{
                return NotFound("User Not Found");
                //}
                //else
                //{
                //    throw;
                //}
            }

            var json = JsonConvert.SerializeObject(user);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
            dictionary.Remove("Password");

            return Ok(dictionary);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {



            if (!await department_id_foundAsync(user))  return NotFound("Department not found");
            


            if (!await role_id_foundAsync(user))  return NotFound("Role not found");

            String dp_pass = BC.HashPassword(user.Password);
            user.Password = dp_pass;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var json = JsonConvert.SerializeObject(user);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
            dictionary.Remove("Password");

            return Ok(dictionary);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            if (!await check_user_role_foundAsync(id)) return NotFound("Not Admin");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User Deleted");
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

        private async Task<bool> department_id_foundAsync(User user)
        {
            var department = await _context.Departments.FindAsync(user.DeptId);
            return department != null;
        }

        private async Task<bool> role_id_foundAsync(User user)
        {
            var role = await _context.Roles.FindAsync(user.RoleId);
            return role != null;
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
