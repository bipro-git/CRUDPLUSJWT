using CRUD_PRACT.Data;
using CRUD_PRACT.Middlewares;
using CRUD_PRACT.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRUD_PRACT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
         IConfiguration _configuration;
         EmployeeDbContext _employeeDbContext;
        private static Dictionary<string, string> RefreshTokens = new Dictionary<string, string>();


        public LoginController(IConfiguration configuration, EmployeeDbContext employeeDbContext)
        {
            this._configuration = configuration;
            this._employeeDbContext = employeeDbContext;
        }


        private Employee AuthenticateEmployee(Guid id, string password)
        {
            var employee = _employeeDbContext.employeesCRUDPrac.Find(id);

            if (employee != null)
            {

                if (id == employee.id && password == employee.Password)
                {
                    return new Employee { id = employee.id, Name = employee.Name, Email = employee.Email, Post = employee.Post, Salary = employee.Salary , Role=employee.Role };
                }
                else return null;
            }
            else
            {
                return null;
            }
        }


        private string GenerateToken(Employee employee)
        {
            var claims = new List<Claim>
            {
                //new Claim(ClaimTypes.Name,"Username"),
                //new Claim(ClaimTypes.Role,"Admin"),
                //new Claim(ClaimTypes.Role,"User")
                 new Claim(JwtRegisteredClaimNames.Sub, employee.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, employee.Email),
                new Claim(ClaimTypes.Name, employee.Name)
            };
            var role = employee.Role;
            claims.Add(new Claim(ClaimTypes.Role, employee.Role));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                 _configuration["Jwt:Issuer"],
                 _configuration["Jwt:Audience"],
                 claims:claims,
                expires: DateTime.Now.AddMinutes(30), // Adjust the expiration time as needed
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }


        [AllowAnonymous]
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] TokenRequest tokenRequest)
        {
            if (BlockToken.IsBlacklisted(tokenRequest.Token))
            {
                return BadRequest("Token already loggedout");
            }
            if (tokenRequest is null)
            {
                return BadRequest("Invalid client request");
            }

            var principal = GetPrincipalFromExpiredToken(tokenRequest.Token);
            if (principal == null)
            {
                return BadRequest("Invalid access token");
            }

            var userId = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userId) || !RefreshTokens.ContainsKey(tokenRequest.RefreshToken) || RefreshTokens[tokenRequest.RefreshToken] != userId)
            {
                return BadRequest("Invalid refresh token");
            }
            var employee = _employeeDbContext.employeesCRUDPrac.Find(Guid.Parse(userId));
            BlockToken.addBlacklist(tokenRequest.Token);
            var newToken = GenerateToken(employee);
            //var newToken = GenerateToken(new Employee { id = Guid.Parse(userId), Name = principal.Identity.Name, Email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value, Role = principal.FindFirst(ClaimTypes.Role)?.Value });
            var newRefreshToken = GenerateRefreshToken();

            // Remove the old refresh token and add the new refresh token
            RefreshTokens.Remove(tokenRequest.RefreshToken);
            RefreshTokens[newRefreshToken] = userId;

            return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(Guid id, string password)
        {
            var user = AuthenticateEmployee(id, password);
            if (user != null)
            {
                var token = GenerateToken(user);
                var refreshToken = GenerateRefreshToken();

                // Store refresh token in-memory
                RefreshTokens[refreshToken] = user.id.ToString();

                return Ok(new { Token = token, RefreshToken = refreshToken });
            }

            return Unauthorized();
        }
    }
}
