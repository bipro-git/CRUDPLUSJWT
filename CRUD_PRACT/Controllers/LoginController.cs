using CRUD_PRACT.Data;
using CRUD_PRACT.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CRUD_PRACT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
         IConfiguration _configuration;
         EmployeeDbContext _employeeDbContext;

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
                    return new Employee { id = employee.id, Name = employee.Name, Email = employee.Email, Post = employee.Post, Salary = employee.Salary };
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
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                 _configuration["Jwt:Issuer"],
                 _configuration["Jwt:Audience"],
                 null,
                expires: DateTime.Now.AddMinutes(30), // Adjust the expiration time as needed
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(Guid id, string password)
        {
            var user = AuthenticateEmployee(id, password);
            if (user != null)
            {
                var token = GenerateToken(user);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }
}
