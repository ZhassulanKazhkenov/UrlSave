using Microsoft.EntityFrameworkCore;
using UrlSave.Entities;

namespace UrlSave.Contexts
{
    public class LinkContext : DbContext
    {
        public LinkContext(DbContextOptions<LinkContext> options)
            : base(options)
        {

        }

        public DbSet<Link> Links { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<ProductSupplier> ProductSuppliers { get; set; }

        public DbSet<PriceProductSupplier> PriceProductSuppliers { get; set; }


    }
}
