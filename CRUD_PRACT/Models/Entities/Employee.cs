namespace CRUD_PRACT.Models.Entities
{
    public class Employee
    {

        public string Email { get; set; }
        public Guid id { get; set; }
        public  string Name { get; set; }
       
        public  string Post { get; set; }
        public long? Salary { get; set; }

        public  string Password { get; set; }
        public string Role { get; set; }
    }
}
