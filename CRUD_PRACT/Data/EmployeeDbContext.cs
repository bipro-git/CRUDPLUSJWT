using CRUD_PRACT.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUD_PRACT.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet <Employee> employeesCRUDPrac { get; set; }
    }
}
