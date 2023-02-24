using Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {

            modelBuilder
            .Entity<UserEntity>()
            .Property(e => e.Role)
            .HasConversion(
                v => v.ToString(),
                v => (Role)Enum.Parse(typeof(Role), v));

            modelBuilder
            .Entity<CommandEntity>()
            .Property(e => e.Command)
            .HasConversion(
                v => v.ToString(),
                v => (Command)Enum.Parse(typeof(Command), v));

        }

        public DbSet<AssetEntity> Assets { get; set; }
        public DbSet<UserEntity>? Users { get; set; }
        public DbSet<CommandEntity>? Commands { get; set; }
    }
}