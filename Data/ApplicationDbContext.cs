using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryTrackingApp.Models;

namespace InventoryTrackingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<InventoryTrackingApp.Models.Product>? Product { get; set; }
        public DbSet<InventoryTrackingApp.Models.Resource>? Resource { get; set; }
    }
}