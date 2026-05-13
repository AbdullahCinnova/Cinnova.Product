using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Cinnova.Product.Tests;

[TestClass]
public sealed class ProductRepositoryTests
{
    [TestMethod]
    public async Task GetActiveAsync_ShouldFilterOutInactiveProducts()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new ProductDbContext(options);
        context.Products.AddRange(
            new Product { Id = 1, Name = "A", Price = 10m, IsActive = true },
            new Product { Id = 2, Name = "B", Price = 11m, IsActive = false },
            new Product { Id = 3, Name = "C", Price = 12m, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        var result = (await repository.GetActiveAsync()).ToArray();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(product => product.IsActive);
        result.Select(product => product.Id).Should().BeEquivalentTo([1, 3]);
    }
}
