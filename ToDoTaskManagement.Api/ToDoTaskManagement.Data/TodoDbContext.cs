using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ToDoTaskManagement.Domain.Entities;

namespace ToDoTaskManagement.Data
{
    public class TodoDbContext : IdentityDbContext<ApplicationUser>
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TodoItem>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Title).IsRequired().HasMaxLength(500);
                b.Property(x => x.Description).HasMaxLength(2000);
                b.Property(x => x.UserId).IsRequired();
                b.HasIndex(x => x.UserId);
            });
            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Token).IsRequired();
                b.HasIndex(x => x.Token).IsUnique();
                b.Property(x => x.UserId).IsRequired();
            });
        }
    }
}
