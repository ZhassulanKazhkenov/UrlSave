using Microsoft.EntityFrameworkCore;
using UrlSave.Contexts;
using UrlSave.Entities;

namespace UrlSave.Services
{
    public class SupplierService
    {
        private readonly LinkContext _linkContext;

        public SupplierService(LinkContext linkContext)
        {
            _linkContext = linkContext;
        }
        public async Task<Supplier> AddOrUpdate(Supplier supplier)
        {
            var sup = await _linkContext.Suppliers
                .Where(x => x.Name.Equals(supplier.Name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefaultAsync();
            if (sup == null)
            {
                _linkContext.Suppliers.Add(supplier);
                await _linkContext.SaveChangesAsync();
                return supplier;
            }
            return sup;
        }
    }
}
