using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleNpgsqlBug
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Parent> Parents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parent>(builder =>
            {
                builder.ToTable("Parent");
                builder.HasKey(n => n.ParentId);
                builder.Property(p => p.Name).IsRequired();
                builder.Property(p => p.ConcurrencyToken).IsRequired().IsConcurrencyToken();

                builder.HasMany(p => p.Childs).WithOne().HasForeignKey(p => p.ParentId).HasPrincipalKey(n => n.ParentId)
                    .IsRequired();
            });

            modelBuilder.Entity<Child>(builder =>
            {
                builder.ToTable("Child");
                builder.HasKey(p => p.ChildId);
                builder.Property(p => p.ChildId).IsRequired();
                builder.Property(p => p.ParentId).IsRequired();
                builder.Property(p => p.Name).IsRequired();
            });
        }
    }
}
