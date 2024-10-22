using Microsoft.EntityFrameworkCore;
using UrlSave.Domain.Entities;

namespace UrlSave.Application.Interfaces;

public interface ILinkContext
{
    DbSet<Link> Links { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Price> Prices { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Notification> Notifications { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
