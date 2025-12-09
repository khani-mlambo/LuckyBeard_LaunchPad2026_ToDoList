using LuckyBeardAPI_Launchpad2026.Data;
using LuckyBeardAPI_Launchpad2026.DTOs;
using LuckyBeardAPI_Launchpad2026.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static LuckyBeardAPI_Launchpad2026.DTOs.AuthDto;

namespace LuckyBeardAPI_Launchpad2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _appdb;
        private IConfiguration _config;

        public AuthController(ApplicationDbContext appdb, IConfiguration config)
        {
            _appdb = appdb;
            _config = config;
        }

        //register
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(AuthDto.RegisterDto regdto,
            CancellationToken ct)
        {
            //Check if email already exists
            var exists = await _appdb.ToDoUsers.AnyAsync(u => u.Email == regdto.Email && !u.IsDeleted, ct);
            if (exists)
            {
                return BadRequest("Email already exists");
            }

            // Create user with hashed password that will be stores into the database
            var user = new UserModel
            {
                Name = regdto.Name,
                Surname = regdto.Surname,
                Email = regdto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(regdto.Password),
                IsDeleted = false
            };

            //adds the user object to the dbSet object Users
            _appdb.ToDoUsers.Add(user);
            await _appdb.SaveChangesAsync(); //saves to dataabase

            return Ok("User Registered Successfully");
        } 

        //Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<AuthDto.TokenResponseDto>> Login([FromBody] AuthDto.LoginDto userlogin,
            CancellationToken ct)
        {
            // Load user from DB
            var user = await _appdb.ToDoUsers
                .FirstOrDefaultAsync(u => u.Email == userlogin.Email && !u.IsDeleted, ct);

            if (user == null)
            {
                return Unauthorized("Invalid Credentials");
            }

            //Verify password
            var valid = BCrypt.Net.BCrypt.Verify(userlogin.Password, user.Password);
            if (!valid)
                return Unauthorized("Invalid Credentials");

            var token = Generate(user);
            return Ok($"Login successful!\nThe token is:\n {token}");

        }

        [Authorize]
        [HttpDelete("DeleteUser")]        
        public async Task<ActionResult> Delete(CancellationToken ct)
        {
            //get email from token
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Invalid token");

            //find user in DB
            var user = await _appdb.ToDoUsers
                .FirstOrDefaultAsync(u => u.Email == userEmail && !u.IsDeleted, ct);

            if (user == null)
                return NotFound("User not found");

            // soft-delete all their todos
            var tasks = await _appdb.ToDos
                .Where(t => t.UserToDoId == user.UserId && !t.IsDeleted)
                .ToListAsync(ct);

            foreach (var todo in tasks)
            {
                todo.IsDeleted = true;
                todo.Updated_At = DateTime.UtcNow;
            }
            user.IsDeleted = true;
            await _appdb.SaveChangesAsync();

            return Ok("User soft deleted");
        }
        private string Generate(UserModel user)
        {
            //create security token for jwt
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims = user identity info that travels with the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Email, user.Email)
            };

            //build token
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }


        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new UserModel
                {
                    Name = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    Surname = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value
                };

            }

            return null;
        }


    }
}
