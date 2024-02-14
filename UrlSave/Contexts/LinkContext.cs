﻿using Microsoft.EntityFrameworkCore;
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }


    }
}
