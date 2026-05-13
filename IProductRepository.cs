using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinnova.Product;

public interface IProductRepository
{
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductDto product, CancellationToken cancellationToken = default);
}
