using FluentAssertions;
using Moq;

namespace Cinnova.Product.Tests;

[TestClass]
public sealed class ProductServiceTests
{
    [TestMethod]
    public async Task GetProductCountAsync_ShouldReturnZero_WhenRepositoryIsEmpty()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var sut = new ProductService(repository.Object);

        var result = await sut.GetProductCountAsync();

        result.Should().Be(0);
    }

    [TestMethod]
    public async Task GetProductCountAsync_ShouldReturnOne_WhenRepositoryHasSingleProduct()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var sut = new ProductService(repository.Object);

        var result = await sut.GetProductCountAsync();

        result.Should().Be(1);
    }

    [TestMethod]
    public async Task GetProductCountAsync_ShouldReturnMultipleCount_WhenRepositoryHasManyProducts()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
        var sut = new ProductService(repository.Object);

        var result = await sut.GetProductCountAsync();

        result.Should().Be(5);
    }

    [TestMethod]
    public async Task ProductExistsAsync_ShouldReturnTrue_WhenProductExists()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var sut = new ProductService(repository.Object);

        var exists = await sut.ProductExistsAsync(10);

        exists.Should().BeTrue();
    }

    [TestMethod]
    public async Task ProductExistsAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.ExistsByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var sut = new ProductService(repository.Object);

        var exists = await sut.ProductExistsAsync(999);

        exists.Should().BeFalse();
    }

    [TestMethod]
    public async Task GetActiveProductsAsync_ShouldReturnActiveProducts_WhenRepositoryReturnsActiveProducts()
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
    public async Task GetActiveProductsAsync_ShouldReturnEmpty_WhenRepositoryReturnsNoActiveProducts()
    {
        var repository = new Mock<IProductRepository>();
        repository.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ProductDto>());
        var sut = new ProductService(repository.Object);

        var activeProducts = await sut.GetActiveProductsAsync();

        activeProducts.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GenericMethods_ShouldForwardCancellationToken_ToRepositoryCalls()
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
