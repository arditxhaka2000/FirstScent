using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FirstScent.Models;

namespace FirstScent.Data
{
    public class FirstScentContext : IdentityDbContext<IdentityUser>
    {
        public FirstScentContext(DbContextOptions<FirstScentContext> options)
            : base(options)
        {
        }

        public DbSet<Fragrances> Fragrances { get; set; }
        public DbSet<RegistrationViewModel> Users { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CartItems> CartItems { get; set; }
        public DbSet<UserOrders> UserOrders { get; set; }
        public DbSet<UserAdresses> UserAdresses { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<UserFavoriteFragrance> favoriteFragrances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
        }


    }
}
