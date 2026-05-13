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
}
