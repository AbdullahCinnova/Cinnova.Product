namespace Cinnova.Product;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductDto product, CancellationToken cancellationToken = default);
    Task<int> GetProductCountAsync(CancellationToken cancellationToken = default);
    Task<bool> ProductExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
}
