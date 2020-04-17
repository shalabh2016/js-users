using JsUsers.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsUsers.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserModel> UserModels { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=jsusers.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasIndex(user => user.Id)
                .IsUnique();

            modelBuilder.Entity<UserModel>()
               .HasKey(user => user.Id);
        }
    }
}
