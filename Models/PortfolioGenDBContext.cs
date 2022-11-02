using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API_Test.Models
{
    public partial class PortfolioGenDBContext : DbContext
    {
        public PortfolioGenDBContext()
        {
        }

        public PortfolioGenDBContext(DbContextOptions<PortfolioGenDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Profile> Profiles { get; set; } = null!; 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.About).HasMaxLength(2080);

                entity.Property(e => e.Avatar).HasMaxLength(2080);

                entity.Property(e => e.Email).HasMaxLength(2080);

                entity.Property(e => e.Facebook).HasMaxLength(2080);

                entity.Property(e => e.FirstName).HasMaxLength(55);

                entity.Property(e => e.Github).HasMaxLength(2080);

                entity.Property(e => e.Instagram).HasMaxLength(2080);

                entity.Property(e => e.Interests).HasMaxLength(2080);

                entity.Property(e => e.LastName).HasMaxLength(55);

                entity.Property(e => e.Linkedin).HasMaxLength(2080);

                entity.Property(e => e.Title).HasMaxLength(55);

                entity.Property(e => e.Website).HasMaxLength(2080);

                entity.Property(e => e.ProfileName).HasMaxLength(55);

                entity.Property(e => e.GithubUsername).HasMaxLength(55); 
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
