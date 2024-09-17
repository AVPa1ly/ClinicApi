using ClinicApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
    }
}
