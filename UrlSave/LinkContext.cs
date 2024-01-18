using Microsoft.EntityFrameworkCore;

namespace UrlSave
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
