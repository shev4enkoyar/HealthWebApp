using HealthWebApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWebApp.DAL
{
    public class ApplicationDbContext : DbContext
    {
        #region Properties
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Cabinet> Cabinets { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        #endregion

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
