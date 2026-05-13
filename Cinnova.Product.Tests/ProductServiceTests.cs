using FluentAssertions;
using Moq;

namespace Cinnova.Product.Tests;

[TestClass]
public sealed class ProductServiceTests
{
    [TestMethod]
    public async Task GetAllAsync_RepositoryReturnsProducts_ReturnsProducts()
    {
        var repository = new Mock<IProductRepository>();
        var products = new[] { new ProductDto(1, "A", 10m, true), new ProductDto(2, "B", 20m, false) };
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
        var sut = new ProductService(repository.Object);

        var result = (await sut.GetAllAsync()).ToArray();

        result.Should().HaveCount(2);
        result.Select(p => p.Id).Should().BeEquivalentTo([1, 2]);
    }

    [TestMethod]
    public async Task GetByIdAsync_RepositoryReturnsProduct_ReturnsProduct()
    {
        var repository = new Mock<IProductRepository>();
        var product = new ProductDto(7, "A", 10m, true);
        repository.Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var sut = new ProductService(repository.Object);

        var result = await sut.GetByIdAsync(7);

        result.Should().NotBeNull();
        result!.Id.Should().Be(7);
    }

    [TestMethod]
    public async Task AddAsync_ProductPriceIsValid_CallsRepository()
    {
        var repository = new Mock<IProductRepository>();
        var product = new ProductDto(1, "A", 10m, true);
        var sut = new ProductService(repository.Object);

        await sut.AddAsync(product);

        repository.Verify(r => r.AddAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task AddAsync_ProductPriceIsNotPositive_ThrowsArgumentException()
    {
        var repository = new Mock<IProductRepository>();
        var product = new ProductDto(1, "A", 0m, true);
        var sut = new ProductService(repository.Object);

        Func<Task> act = async () => await sut.AddAsync(product);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Price must be greater than 0");
        repository.Verify(r => r.AddAsync(It.IsAny<ProductDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task GetProductCountAsync_RepositoryIsEmpty_ReturnsZero()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var sut = new ProductService(repository.Object);

        var result = await sut.GetProductCountAsync();

        result.Should().Be(0);
    }

    [TestMethod]
    public async Task GetProductCountAsync_RepositoryHasSingleProduct_ReturnsOne()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var sut = new ProductService(repository.Object);

        var result = await sut.GetProductCountAsync();

        result.Should().Be(1);
    }

    [TestMethod]
    public async Task GetProductCountAsync_RepositoryHasManyProducts_ReturnsMultipleCount()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
        var sut = new ProductService(repository.Object);

        var result = await sut.GetProductCountAsync();

        result.Should().Be(5);
    }

    [TestMethod]
    public async Task ProductExistsAsync_ProductExists_ReturnsTrue()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var sut = new ProductService(repository.Object);

        var exists = await sut.ProductExistsAsync(10);

        exists.Should().BeTrue();
    }

    [TestMethod]
    public async Task ProductExistsAsync_ProductDoesNotExist_ReturnsFalse()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.ExistsByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var sut = new ProductService(repository.Object);

        var exists = await sut.ProductExistsAsync(999);

        exists.Should().BeFalse();
    }

    [TestMethod]
    public async Task GetActiveProductsAsync_RepositoryReturnsActiveProducts_ReturnsActiveProducts()
    {
        var repository = new Mock<IProductRepository>();
        var activeProducts = new[]
        {
            new ProductDto(1, "A", 10m, true),
            new ProductDto(3, "C", 12m, true)
        };

        repository.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(activeProducts);
        var sut = new ProductService(repository.Object);

        var result = (await sut.GetActiveProductsAsync()).ToArray();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.IsActive);
        result.Select(p => p.Id).Should().BeEquivalentTo([1, 3]);
    }

    [TestMethod]
    public async Task GetActiveProductsAsync_RepositoryReturnsNoActiveProducts_ReturnsEmpty()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());
        var sut = new ProductService(repository.Object);

        var activeProducts = await sut.GetActiveProductsAsync();

        activeProducts.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GenericMethods_CancellationTokenProvided_ForwardsTokenToRepositoryCalls()
    {
        var repository = new Mock<IProductRepository>();
        var cancellationToken = new CancellationTokenSource().Token;

        repository.Setup(r => r.CountAsync(cancellationToken)).ReturnsAsync(0);
        repository.Setup(r => r.ExistsByIdAsync(7, cancellationToken)).ReturnsAsync(false);
        repository.Setup(r => r.GetActiveAsync(cancellationToken))
            .ReturnsAsync(new[] { new ProductDto(1, "A", 1m, true) });
        var sut = new ProductService(repository.Object);

        await sut.GetProductCountAsync(cancellationToken);
        await sut.ProductExistsAsync(7, cancellationToken);
        await sut.GetActiveProductsAsync(cancellationToken);

        repository.Verify(r => r.CountAsync(cancellationToken), Times.Once);
        repository.Verify(r => r.ExistsByIdAsync(7, cancellationToken), Times.Once);
        repository.Verify(r => r.GetActiveAsync(cancellationToken), Times.Once);
    }
}
