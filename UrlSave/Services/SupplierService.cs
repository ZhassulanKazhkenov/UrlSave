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
        public async Task<Supplier> AddAsync(Supplier supplier)
        {
            var existingSupplier = await _linkContext.Suppliers
                .Where(x => x.Name.ToLower() == supplier.Name.ToLower())
                .FirstOrDefaultAsync();
            if (existingSupplier == null)
            {
                _linkContext.Suppliers.Add(supplier);
                await _linkContext.SaveChangesAsync();
                return supplier;
            }
            return existingSupplier;
        }
    }
}
