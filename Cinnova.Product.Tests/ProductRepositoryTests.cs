using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Cinnova.Product.Tests;

[TestClass]
public sealed class ProductRepositoryTests
{
    [TestMethod]
    public async Task GetAllAsync_ProductsExist_ReturnsAllProducts()
    {
        await using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "A", Price = 10m, IsActive = true },
            new Product { Id = 2, Name = "B", Price = 11m, IsActive = false });
        await context.SaveChangesAsync();
        var repository = new ProductRepository(context);

        var result = (await repository.GetAllAsync()).ToArray();

        result.Should().HaveCount(2);
        result.Select(product => product.Id).Should().BeEquivalentTo([1, 2]);
    }

    [TestMethod]
    public async Task GetActiveAsync_ProductsContainInactiveItems_FiltersOutInactiveProducts()
    {
        await using var context = CreateContext();
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

    [TestMethod]
    public async Task CountAsync_ProductsExist_ReturnsProductCount()
    {
        await using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "A", Price = 10m, IsActive = true },
            new Product { Id = 2, Name = "B", Price = 11m, IsActive = false });
        await context.SaveChangesAsync();
        var repository = new ProductRepository(context);

        var count = await repository.CountAsync();

        count.Should().Be(2);
    }

    [TestMethod]
    public async Task ExistsByIdAsync_ProductExists_ReturnsTrue()
    {
        await using var context = CreateContext();
        context.Products.Add(new Product { Id = 10, Name = "A", Price = 10m, IsActive = true });
        await context.SaveChangesAsync();
        var repository = new ProductRepository(context);

        var exists = await repository.ExistsByIdAsync(10);

        exists.Should().BeTrue();
    }

    [TestMethod]
    public async Task ExistsByIdAsync_ProductDoesNotExist_ReturnsFalse()
    {
        await using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "A", Price = 10m, IsActive = true });
        await context.SaveChangesAsync();
        var repository = new ProductRepository(context);

        var exists = await repository.ExistsByIdAsync(99);

        exists.Should().BeFalse();
    }

    [TestMethod]
    public async Task GetByIdAsync_ProductExists_ReturnsProduct()
    {
        await using var context = CreateContext();
        context.Products.Add(new Product { Id = 3, Name = "A", Price = 10m, IsActive = true });
        await context.SaveChangesAsync();
        var repository = new ProductRepository(context);

        var product = await repository.GetByIdAsync(3);

        product.Should().NotBeNull();
        product!.Id.Should().Be(3);
    }

    [TestMethod]
    public async Task GetByIdAsync_ProductDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "A", Price = 10m, IsActive = true });
        await context.SaveChangesAsync();
        var repository = new ProductRepository(context);

        var product = await repository.GetByIdAsync(50);

        product.Should().BeNull();
    }

    [TestMethod]
    public async Task AddAsync_ValidProduct_AddsEntityToDatabase()
    {
        await using var context = CreateContext();
        var repository = new ProductRepository(context);

        await repository.AddAsync(new ProductDto(0, "New", 22m, true));

        var entities = await context.Products.ToListAsync();
        entities.Should().HaveCount(1);
        entities[0].Name.Should().Be("New");
        entities[0].Price.Should().Be(22m);
        entities[0].IsActive.Should().BeTrue();
    }

    private static ProductDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ProductDbContext(options);
    }
}
