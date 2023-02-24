using Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
        }

        public DbSet<AssetEntity> Assets { get; set; }
        public DbSet<UserEntity>? Users { get; set; }

        public DbSet<CommandEntity>? Commands { get; set; }
    }
}