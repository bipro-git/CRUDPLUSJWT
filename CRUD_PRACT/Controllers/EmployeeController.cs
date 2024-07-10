using CRUD_PRACT.Data;
using CRUD_PRACT.Models;
using CRUD_PRACT.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_PRACT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly EmployeeDbContext employeeDbContext;
        public EmployeeController(EmployeeDbContext employeeDbContext)
        {
            this.employeeDbContext = employeeDbContext;
        }

        [Authorize]
        [HttpGet]
        [Route("GetData")]
        public IActionResult GetData()
        {
            var allemployees = employeeDbContext.employeesCRUDPrac.ToList();
            var fetchdtos = allemployees.Select(Employee => new fetchDto
            {
                Name = Employee.Name,
                Email =Employee.Email,
                Post=Employee.Post,
                Salary = Employee.Salary,
            });
            return Ok(fetchdtos) ;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEmployee")]
        public IActionResult Post(EmployeeDto employeeDto)
        {
            var employeeentity = new Employee()
            {
                Name = employeeDto.Name,
                Email = employeeDto.Email,
                Post  = employeeDto.Post,
                Salary = employeeDto.Salary,
                Password = employeeDto.Password,
            };
            employeeDbContext.employeesCRUDPrac.Add(employeeentity);
            employeeDbContext.SaveChanges();
            return Ok(employeeentity);

        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public IActionResult getEmployeeById(Guid id)
        {

            var employee = employeeDbContext.employeesCRUDPrac.Find(id);
            if (employee == null)
            {
                return BadRequest();
            }
            return Ok(employee);
        }
    }
}
