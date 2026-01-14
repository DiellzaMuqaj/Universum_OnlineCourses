using Microsoft.EntityFrameworkCore;
using Universum_OnlineCourses.Domain.Entities;

namespace Universum_OnlineCourses.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserCourseAccess> UserCourseAccesses => Set<UserCourseAccess>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // USER
            // =========================
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // =========================
            // USER → REFRESH TOKENS
            // =========================
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // COURSE ↔ USER COURSE ACCESS
            // =========================
            modelBuilder.Entity<UserCourseAccess>()
                .HasKey(x => new { x.UserId, x.CourseId });

            modelBuilder.Entity<UserCourseAccess>()
                .HasOne(x => x.Course)
                .WithMany(c => c.UserCourseAccesses)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCourseAccess>()
                .HasOne(x => x.User)
                .WithMany(u => u.UserCourseAccesses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // COURSE → MODULES
            // =========================
            modelBuilder.Entity<Module>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // MODULE → LESSONS
            // =========================
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Module)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // PAYMENTS
            // =========================
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Course)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
