using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gfraser4_College_Strike.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
//Graeme Fraser
//Project Part 5
namespace gfraser4_College_Strike.Data
{
    public class CollegeStrikeContext : DbContext
    {
        public CollegeStrikeContext(DbContextOptions<CollegeStrikeContext> options)
            : base(options)
        {
            UserName = "SeedData";
        }

        public CollegeStrikeContext(DbContextOptions<CollegeStrikeContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
            UserName = UserName ?? "Unknown";
        }

        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<MemberPosition> MemberPositions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("CS");

            modelBuilder.Entity<Assignment>()
                .HasMany<Member>(a => a.Members)
                .WithOne(m => m.Assignment)
                .HasForeignKey(m => m.AssignmentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Position>()
            .HasIndex(p => p.Title)
            .IsUnique();

            modelBuilder.Entity<Member>()
            .HasIndex(p => p.eMail)
            .IsUnique();

            modelBuilder.Entity<Assignment>()
            .HasIndex(p => p.AssignmentName)
            .IsUnique();

            modelBuilder.Entity<MemberPosition>()
            .HasKey(t => new { t.PositionID, t.MemberID });

            modelBuilder.Entity<MemberPosition>()
                .HasOne(pc => pc.Position)
                .WithMany(c => c.MemberPositions)
                .HasForeignKey(pc => pc.PositionID)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Shift>()
            //.HasKey(t => new { t.ID });

            modelBuilder.Entity<Shift>()
            .HasOne(s => s.Member)
            .WithMany(m => m.Shifts)
            .HasForeignKey(s => s.MemberID)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shift>()
            .HasOne(s => s.Assignment)
            .WithMany(a => a.Shifts)
            .HasForeignKey(s => s.AssignmentID)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shift>()
            .HasIndex(s => new { s.MemberID, s.ShiftDate })
            .IsUnique();

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }
}

