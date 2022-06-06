using Microsoft.AspNetCore.Mvc;

using sheift.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;


using BC = BCrypt.Net.BCrypt;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace sheift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigninController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly shieftContext _context;

        public SigninController(IConfiguration config, shieftContext context)
        {
            _configuration = config;
            _context = context;
        }



        [EnableCors("AllowOrigin")]
        [HttpPost]
        public async Task<IActionResult> Post(Sign_in _userData)
        {

            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {


                var user = await CheckUserEmail(_userData.Email);
                

                if (user != null)
                {
                    //String dp_pass = BC.HashPassword(user.Password);
                    
                    string role_name = await get_role_foundAsync(user);
                    String dep_name = await GetDepartment(user);
                    if (BC.Verify(_userData.Password, user.Password))
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                            new Claim("id", user.UserId.ToString()),
                            new Claim  (ClaimTypes.Role,role_name),//--------set role of user ------------
                            new Claim("UserName", user.UserName),
                            new Claim("Email", user.Email)
                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                        var json = JsonConvert.SerializeObject(user);
                        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
                        dictionary.Add("token", new JwtSecurityTokenHandler().WriteToken(token));
                        dictionary.Remove("Password");
                        dictionary.Remove("RoleId");
                        dictionary.Add("Role", role_name);
                        dictionary.Add("Department", dep_name);
                        return Ok(dictionary);


                    }
                    else return BadRequest("Invalid email and password");
                }else return NotFound("Invalid email and password");
            }else return BadRequest("Fill all Parameters");
            

        }


        


        private async Task<User> CheckUserEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
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

        private async Task<String> GetDepartment(User user)
        {

            //if (!await check_user_role_foundAsync(admin_id)) return NotFound("Not Admin");

            var department = await _context.Departments.FindAsync(user.DeptId);

            if (department != null)
            {
                return department.DepName;
            }

            return null;
        }


    }
}