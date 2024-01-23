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

    }
}
