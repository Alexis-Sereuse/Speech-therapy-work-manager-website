using GestionnaireOrthophonie.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionnaireOrthophonie.Data
{
    public class DatabaseContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<NamedPeriod> NamedPeriods { get; set; }
        public DbSet<PlanningOptions> PlanningOptions { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
