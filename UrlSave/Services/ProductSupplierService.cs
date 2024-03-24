using Microsoft.EntityFrameworkCore;
using UrlSave.Contexts;
using UrlSave.Entities;

namespace UrlSave.Services
{
    public class ProductSupplierService
    {
        private readonly LinkContext _linkContext;

        public ProductSupplierService(LinkContext linkContext)
        {
            _linkContext = linkContext;
        }
        public async Task<ProductSupplier> AddAsync(ProductSupplier productSupplier)
        {
            var existingProductSupplier = await _linkContext.ProductSuppliers
                .Where(x => x.ProductId == productSupplier.ProductId && x.SupplierId == productSupplier.SupplierId)
                .FirstOrDefaultAsync();
            if (existingProductSupplier == null)
            {
                _linkContext.ProductSuppliers.Add(productSupplier);
                await _linkContext.SaveChangesAsync();
                return productSupplier;
            }
            return existingProductSupplier;
        }
    }
}
