using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cinnova.Product.Tests;

[TestClass]
public sealed class ProductControllerTests
{
    [TestMethod]
    public async Task GetAll_ServiceReturnsProducts_ReturnsOkResultWithProducts()
    {
        var service = new Mock<IProductService>();
        var products = new[] { new ProductDto(1, "A", 10m, true), new ProductDto(2, "B", 20m, false) };
        service.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
        var controller = new ProductController(service.Object);

        var result = await controller.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = ok.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject.ToArray();
        value.Should().HaveCount(2);
    }

    [TestMethod]
    public async Task GetById_ProductExists_ReturnsOkResultWithProduct()
    {
        var service = new Mock<IProductService>();
        var product = new ProductDto(1, "A", 10m, true);
        service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var controller = new ProductController(service.Object);

        var result = await controller.GetById(1);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(product);
    }

    [TestMethod]
    public async Task GetById_ProductDoesNotExist_ReturnsNotFound()
    {
        var service = new Mock<IProductService>();
        service.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto?)null);
        var controller = new ProductController(service.Object);

        var result = await controller.GetById(5);

        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
