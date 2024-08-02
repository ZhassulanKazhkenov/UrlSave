using Microsoft.EntityFrameworkCore;
using UrlSave.Contexts;
using UrlSave.Entities;

namespace UrlSave.Services;

public class ProductService
{
    private readonly LinkContext _linkContext;

    public ProductService(LinkContext linkContext)
    {
        _linkContext = linkContext;
    }
    //Публичный асинхрон метод который возвра тип Product имеет назван AddOrUpdate и приним тип Prod и назв парам product
    public async Task<Product> AddAsync(Product product)
    {
        var existingProduct = await _linkContext.Products
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
