﻿using Microsoft.EntityFrameworkCore;
using MuseumAPI.Models;

namespace MuseumAPI.Context
{
    public class MuseumContext : DbContext
    {
        public MuseumContext() { }

        public MuseumContext(DbContextOptions<MuseumContext> options) : base(options)
        {
            // EnsureDeleted to skip migrations
            // delete to keep data between runs
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set Painting price default value to 1000.00
            modelBuilder.Entity<Painting>()
                .Property(p => p.Price)
                .HasDefaultValue(1000.00);

            // Set PagePreference default value to 5
            modelBuilder.Entity<UserProfile>()
                .Property(p => p.PagePreference)
                .HasDefaultValue(5);

            // Set AccessLevel default value to Unconfirmed
            modelBuilder.Entity<User>()
                .Property(u => u.AccessLevel)
                .HasDefaultValue(AccessLevel.Unconfirmed);

            // Make user name unique constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();

            // Make user name unique constraint
            modelBuilder.Entity<ConfirmationCode>()
                .HasIndex(c => c.Code)
                .IsUnique();

            // Define one-to-one relationship between User and UserProfile
            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define one-to-many relationship Artist and Painting
            modelBuilder.Entity<Painting>()
                .HasOne(a => a.Artist)
                .WithMany(p => p.Paintings)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define many-to-many relationship between Artist and Museum
            modelBuilder.Entity<Museum>()
                .HasMany(p => p.Artists)
                .WithMany(p => p.Museums)
                .UsingEntity<Exhibition>(
                    j => j
                        .HasOne(pt => pt.Artist)
                        .WithMany(t => t.Exhibitions)
                        .HasForeignKey(pt => pt.ArtistId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne(pt => pt.Museum)
                        .WithMany(p => p.Exhibitions)
                        .HasForeignKey(pt => pt.MuseumId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.Property(pt => pt.StartDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                        j.Property(pt => pt.EndDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                        j.HasKey(t => new { t.ArtistId, t.MuseumId });
                    });

            // Define one-to-many relationship User and Confirmation Code
            modelBuilder.Entity<ConfirmationCode>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assign users to Artists
            modelBuilder.Entity<Artist>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Assign users to Paintings
            modelBuilder.Entity<Painting>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Assign users to Museums
            modelBuilder.Entity<Museum>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Assign users to Exhibitions
            modelBuilder.Entity<Exhibition>()
                .HasOne(ss => ss.User)
                .WithMany()
                .HasForeignKey(ss => ss.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public virtual DbSet<User> Users { get; set; } = null!;
        
        public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;

        public virtual DbSet<ConfirmationCode> ConfirmationCodes { get; set; } = null!;
        
        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        public virtual DbSet<Painting> Paintings { get; set; } = null!;

        public virtual DbSet<Artist> Artists { get; set; } = null!;

        public virtual DbSet<Museum> Museums { get; set; } = null!;

        public virtual DbSet<Exhibition> Exhibitions { get; set; } = null!;
    }
}
