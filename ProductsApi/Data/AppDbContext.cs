using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Data
{
    // This is our database context - it handles all the communication with our database
    public class AppDbContext : DbContext
    {
        // Constructor that sets up our database connection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // This is our collection of products in the database
        public DbSet<Product> Products { get; set; }
    }
}
