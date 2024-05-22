using Microsoft.EntityFrameworkCore;
using OrcShackApi.Core.Models;

namespace OrcShackApi.Data
{
    public class OrcShackApiContext : DbContext
    {
        public OrcShackApiContext() { }

        public OrcShackApiContext(DbContextOptions<OrcShackApiContext> options) : base(options) { }

        public virtual DbSet<Dish> Dishes { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureDishEntity(modelBuilder);
            ConfigureRatingEntity(modelBuilder);
            SeedAdminUser(modelBuilder);
        }

        private void ConfigureDishEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dish>()
                .Property(d => d.Id)
                .ValueGeneratedOnAdd();
        }

        private void ConfigureRatingEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Dish)
                .WithMany(d => d.Ratings)
                .HasForeignKey(r => r.DishId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId);
        }

        private void SeedAdminUser(ModelBuilder modelBuilder)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash("admin", out passwordHash, out passwordSalt);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@gmail.com",
                Role = "Admin",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
