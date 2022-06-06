#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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


        //GET: api/Shiefts
        [HttpGet("shift_details_inMonth")]
        public async Task<ActionResult<IEnumerable<ShiftDetail>>> GetShiefts(String month_year)
        {
            
            //  join between 2 tables  
            //var shift_shiftType_list = await _context.Shiefts
            //     .Join(
            //         _context.ShiftTypes,
            //         shift => shift.ShiftTypeId,
            //         ShiftType => ShiftType.ShiftTypeId,
            //         (shift, ShiftType) => new
            //         {
            //             Shift_id = shift.ShiftId,
            //         }
            //     ).ToListAsync();

            //join between multible table and where condition by linq query
            //var shift_shiftType_list = (from s in _context.Shiefts
            //                            join st in _context.ShiftTypes on s.ShiftTypeId equals st.ShiftTypeId
            //                            join ua in _context.Users on s.AdminId equals ua.UserId
            //                            join u in _context.Users on s.UserId equals u.UserId
            //                            where s.Date.Substring(3, 7) == month_year
            //                            select new ShiftDetail()
            //                            {
            //                                ShiftId = s.ShiftId,
            //                                Date = s.Date,
            //                                UserId = s.UserId,
            //                                UserName = u.UserName,
            //                                AdminId = s.AdminId,
            //                                AdminName = ua.UserName,
            //                                Time = s.Time,
            //                                ShiftName = st.ShiftName
            //                            }).ToList();

            
            // use view databse 
            var shift_details = _context.ShiftDetails.Where(s => s.Date.Substring(3, 7) == month_year).OrderBy(d=>d.DepName);

            return Ok(shift_details);
        }


        [HttpGet("shift_details_inDay")]
        public async Task<ActionResult<IEnumerable<ShiftDetail>>> GetShiefts_inDay(String day_month_year)
        {

            // use view databse 
            var shift_details = _context.ShiftDetails.Where(s => s.Date == day_month_year).OrderBy(d => d.DepName);

            return Ok(shift_details);
        }

        [HttpGet("users_shift_count__bymonth")]
        public async Task<ActionResult<IEnumerable<ShiftDetail>>> users_shift_count__bymonth(String month_year,int manger_id)
        {

            

            var countPerGroup = await _context.ShiftDetails
                    .Where(s => s.Date.Substring(3, 7) == month_year&&s.AdminId==manger_id)
                    .GroupBy(t => t.UserName)
                    .Select(shiftGroup => new { user_name = shiftGroup.Key, Count = shiftGroup.Count() })
                    .ToListAsync();

            return Ok(countPerGroup);
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

            if (await user_id_foundAsync(shieft.UserId)==null) return NotFound("User not Found");

            if (!await check_user_role_foundAsync(shieft.AdminId)) return NotFound("Not Admin");

            if (await shieft_type_foundAsync(shieft)==null) return NotFound("Shift type not found");

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

            if (await user_id_foundAsync(shieft.UserId)==null) return NotFound("User not Found");

            if (!await check_user_role_foundAsync(shieft.AdminId)) return NotFound("Not Department Admin");

            if (await shieft_type_foundAsync(shieft)==null) return NotFound("Shift type not found");

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

        private async Task<User> user_id_foundAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user ;
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
                        var department = _context.DepartmentMangers.FirstOrDefault(c=>c.DepId==user.DeptId);

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

        private async Task<String> shieft_type_foundAsync(Shieft shieft)
        {
            var shift_type = await _context.ShiftTypes.FindAsync(shieft.ShiftTypeId);
            if (shift_type != null) {
                return shift_type.ShiftName;
            }
            return  null;
        }



        private bool ShieftExists(int id)
        {
            return _context.Shiefts.Any(e => e.ShiftId == id);
        }
    }
}
