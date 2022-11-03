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
        public virtual DbSet<Token> Tokens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:portfoliogensqldatabase.database.windows.net,1433;Initial Catalog=PortfolioGenDB;Persist Security Info=False;User ID=portAdmin;Password=EFjUAPveMg4K89x;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasIndex(e => e.ProfileName, "UQ__Profiles__A8A4D4708AE8133A")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.About).HasMaxLength(2080);

                entity.Property(e => e.Avatar).HasMaxLength(2080);

                entity.Property(e => e.Email).HasMaxLength(2080);

                entity.Property(e => e.Facebook).HasMaxLength(2080);

                entity.Property(e => e.FirstName).HasMaxLength(55);

                entity.Property(e => e.Github).HasMaxLength(2080);

                entity.Property(e => e.GithubUsername).HasMaxLength(55);

                entity.Property(e => e.Instagram).HasMaxLength(2080);

                entity.Property(e => e.Interests).HasMaxLength(2080);

                entity.Property(e => e.LastName).HasMaxLength(55);

                entity.Property(e => e.Linkedin).HasMaxLength(2080);

                entity.Property(e => e.ProfileName).HasMaxLength(55);

                entity.Property(e => e.Title).HasMaxLength(55);

                entity.Property(e => e.Website).HasMaxLength(2080);
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AccessToken)
                    .HasMaxLength(55)
                    .HasColumnName("access_token");

                entity.Property(e => e.Scope)
                    .HasMaxLength(55)
                    .HasColumnName("scope");

                entity.Property(e => e.TokenType)
                    .HasMaxLength(55)
                    .HasColumnName("token_type");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Tokens__user_id__6FE99F9F");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
