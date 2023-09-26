using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerServer
{
    internal class Context : DbContext
    {
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=TaskTrackerDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(x => x.Role).WithMany().OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasOne(x => x.Assignee).WithMany().OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(x => x.Status).WithMany().OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
