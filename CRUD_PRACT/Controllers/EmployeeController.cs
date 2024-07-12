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
            if (employeeDto.Name.Contains("AD")) employeeentity.Role = "Admin";
            else employeeentity.Role = "User";
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


        [Authorize(Roles ="Admin")]
        [HttpPut]
        [Route("{id}")]
        public IActionResult updateEmployeeById(Guid id, EmployeeDto employeeDto)
        {
            var employee = employeeDbContext.employeesCRUDPrac.Find(id);
            if(employee == null) return BadRequest();
            employee.Name = employeeDto.Name;
            employee.Email = employeeDto.Email;
            employee.Post   = employeeDto.Post;
            employee.Salary = employeeDto.Salary;
            employee.Password = employeeDto.Password;
            //var employeeentity = new Employee()
            //{
            //    Name = employeeDto.Name,
            //    Email = employeeDto.Email,
            //    Post = employeeDto.Post,
            //    Salary = employeeDto.Salary,
            //    Password = employeeDto.Password,
            //};
            if (employeeDto.Name.Contains("AD")) employee.Role = "Admin";
            else employee.Role = "User";
            employeeDbContext.employeesCRUDPrac.Update(employee);
            employeeDbContext.SaveChanges();
            return Ok(employee);

        }


        [Authorize(Roles="Admin")]
        [HttpDelete]
        [Route("{id}")]
        public ActionResult deleteEmployeeById(Guid id) {
            var employee = employeeDbContext.employeesCRUDPrac.Find(id);
            if (employee == null) return BadRequest("You don't have the authority!");
            employeeDbContext.employeesCRUDPrac.Remove(employee);
            employeeDbContext.SaveChanges();
            return Ok("Deleted Successfully");
        }
    }
}
