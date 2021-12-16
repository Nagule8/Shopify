using ShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ShopApi.Data
{
    public class CategoryApiContext : DbContext
    {
        public CategoryApiContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegisterUser>()
                .Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<RegisterUser> RegisterUsers { get; set; }
        public DbSet<ShopApi.Models.UserActivity> UserActivity { get; set; }
    }
}
