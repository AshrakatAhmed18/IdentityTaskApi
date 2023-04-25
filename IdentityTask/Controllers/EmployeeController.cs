using autheticationpart.Data.models;
using autheticationpart.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace autheticationpart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Employee> _userManager;

        public EmployeeController(IConfiguration configuration , UserManager<Employee> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("AdminRegister")]
        public async Task<ActionResult> AdminRegister( RegisterDto registerDto)
        {
            var employee = new Employee
            {
                UserName   = registerDto.UsrName,
                Email      = registerDto.Email,
                Department = registerDto.Department,
            };

           var result = await _userManager.CreateAsync(employee, registerDto.Password);

            if(!result.Succeeded) 
            {
                return BadRequest(result.Errors);
            }

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name,employee.UserName),
               new Claim(ClaimTypes.NameIdentifier , employee.Id),
               new Claim(ClaimTypes.Role , "Admin"),
            };

            await _userManager.AddClaimsAsync(employee, claims);

            return Ok("Use Added");          
        }


        [HttpPost]
        [Route("UserRegister")]
        public async Task<ActionResult> UserRegister(RegisterDto registerDto)
        {
            var employee = new Employee
            {
                UserName = registerDto.UsrName,
                Email = registerDto.Email,
                Department = registerDto.Department,
            };

            var result = await _userManager.CreateAsync(employee, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name,employee.UserName),
               new Claim(ClaimTypes.NameIdentifier , employee.Id),
               new Claim(ClaimTypes.Role , "User"),
            };

            await _userManager.AddClaimsAsync(employee, claims);

            return Ok("User Added");
        }


        [HttpPost]
        [Route("Login")]
        public async Task <ActionResult<TokenDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UsrName);
            if(user == null)
            {
                return Unauthorized();
            }

            var isAuthenticated = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if(!isAuthenticated) 
            {
                return Unauthorized();
            }

            var claimsList = await _userManager.GetClaimsAsync(user);

            var secretKeyString = _configuration.GetValue<string>("SecretKey");
            var secretKeyInByte = Encoding.ASCII.GetBytes(secretKeyString ?? string.Empty);
            var secretKey = new SymmetricSecurityKey(secretKeyInByte);

            var signinCredentials = new SigningCredentials(secretKey,
                SecurityAlgorithms.HmacSha256Signature);

            var expireDate = DateTime.Now.AddDays(1);
            var token = new JwtSecurityToken(
                claims: claimsList, expires: expireDate, signingCredentials: signinCredentials);

            var tokenHndler = new JwtSecurityTokenHandler();
            var tokenString = tokenHndler.WriteToken(token);

            return new TokenDto(tokenString, expireDate);
        }



    }
}
