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
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=TaskTrackerDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(x => x.Role).WithMany().OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasOne(x => x.UserCreated).WithMany().OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.LastUserModified).WithMany().OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Assignee).WithMany().OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Status).WithMany().OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
