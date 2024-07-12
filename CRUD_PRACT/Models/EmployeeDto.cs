using CRUD_PRACT.Models.Entities;

namespace CRUD_PRACT.Models
{
    public class EmployeeDto 
    {
        public string Name { get; set; }

        public string Email { get; set; }
      
        public  string Post { get; set; }
        public long? Salary { get; set; }

        public  string Password { get; set; }
        public string Role { get; set; }
    }
}
