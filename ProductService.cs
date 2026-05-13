using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cinnova.Product;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(id, cancellationToken);

    public async Task AddAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        if (product.Price <= 0)
            throw new ArgumentException("Price must be greater than 0");
        await _repository.AddAsync(product, cancellationToken);
    }
    // Returns the total number of products
    public async Task<int> GetProductCountAsync(CancellationToken cancellationToken = default)
        => await _repository.CountAsync(cancellationToken);

    // Checks if a product exists by id
    public async Task<bool> ProductExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _repository.ExistsByIdAsync(id, cancellationToken);

    // Returns all active products
    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        => await _repository.GetActiveAsync(cancellationToken);
}
