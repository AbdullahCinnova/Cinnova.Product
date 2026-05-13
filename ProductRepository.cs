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

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Products.CountAsync(cancellationToken);

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _context.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
        return p == null ? null : new ProductDto(p.Id, p.Name, p.Price, p.IsActive);
    }

    public async Task AddAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        var entity = new Product { Name = product.Name, Price = product.Price, IsActive = product.IsActive };
        _context.Products.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
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
