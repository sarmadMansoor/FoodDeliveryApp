using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_Project.Models;

namespace Web_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Product> products { get; set; } 
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<BookingModel> bookings { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
