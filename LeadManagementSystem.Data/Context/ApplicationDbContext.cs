using LeadManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LeadManagementSystem.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {

    
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadStatus> LeadStatuses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);


            SeedRole(builder);

        }

        private void SeedRole(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Name = "Manager", ConcurrencyStamp = "2", NormalizedName = "Manager" },
                new IdentityRole() { Name = "Agent",ConcurrencyStamp = "3", NormalizedName = "Agent"}
                );
        }





    }
}
