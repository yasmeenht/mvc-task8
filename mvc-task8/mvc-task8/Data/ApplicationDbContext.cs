using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvc_task8.Models;

namespace mvc_task8.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<SupplyRequest> SupplyRequests { get; set; }
        public DbSet<RequestItem> RequestItems { get; set; }
    }
}

