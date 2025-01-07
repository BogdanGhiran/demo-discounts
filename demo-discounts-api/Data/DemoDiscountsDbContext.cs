using Microsoft.EntityFrameworkCore;
using demo_discounts_api.Models;

namespace demo_discounts_api.Data
{
    public class DemoDiscountsDbContext : DbContext
    {
        public DemoDiscountsDbContext(DbContextOptions<DemoDiscountsDbContext> options)
            : base(options)
        {
        }

        public DbSet<DiscountCode> DiscountCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DiscountCode>(entity =>
            {
                entity.HasKey(dc => dc.Code);
                
                entity.Property(dc => dc.Code)
                    .IsRequired();
                
                entity.Property(dc => dc.DateCreated)
                    .IsRequired();
            });
        }
    }
}