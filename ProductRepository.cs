using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cinnova.Product;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        return await _context.Products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.IsActive))
            .ToListAsync();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Products.CountAsync(cancellationToken);

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var p = await _context.Products.FindAsync(id);
        return p == null ? null : new ProductDto(p.Id, p.Name, p.Price, p.IsActive);
    }

    public async Task AddAsync(ProductDto product)
    {
        var entity = new Product { Name = product.Name, Price = product.Price, IsActive = product.IsActive };
        _context.Products.Add(entity);
        await _context.SaveChangesAsync();
    }
}

public class ProductDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
