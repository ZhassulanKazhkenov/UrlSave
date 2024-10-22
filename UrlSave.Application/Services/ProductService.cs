using Microsoft.EntityFrameworkCore;
using UrlSave.Application.Interfaces;
using UrlSave.Domain.Entities;

namespace UrlSave.Application.Services;

public class ProductService
{
    private readonly ILinkContext _linkContext;

    public ProductService(ILinkContext linkContext)
    {
        _linkContext = linkContext;
    }
    //Публичный асинхрон метод который возвра тип Product имеет назван AddOrUpdate и приним тип Prod и назв парам product
    public async Task<Product> AddAsync(Product product)
    {
        var existingProduct = await _linkContext.Products
            .AsNoTracking()
            .Where(x => x.Name.ToLower() == product.Name.ToLower())
            .FirstOrDefaultAsync();
        if (existingProduct == null)
        {
            _linkContext.Products.Add(product);
            await _linkContext.SaveChangesAsync();
            return product;
        }
        return existingProduct;
    }
}
