# Cinnova Code Review Standards for GitHub Copilot

## Overview

This file defines the code quality standards and best practices that GitHub Copilot should enforce when reviewing code in pull requests for the Cinnova.Product repository.

## Code Style Guidelines

### C# Naming Conventions

- Use PascalCase for class names, method names, and properties
- Use camelCase for private fields and local variables
- Prefix private fields with underscore: `_repository`, `_service`
- Use UPPER_CASE for constants
- Avoid Hungarian notation

### Async/Await Requirements

- All I/O operations must use async/await pattern
- Always include `CancellationToken` parameter (default value allowed)
- Return `Task` or `Task<T>` for async methods
- Never use `.Result` or `.Wait()` - always await
- Properly propagate CancellationToken to all async calls

### Method Structure

- Methods should be small and focused (max 20 lines of logic)
- Use early returns for validation/guard clauses
- Keep nesting depth to maximum 3 levels
- Single responsibility principle (one reason to change)
- Separate concerns: service layer handles business logic, repository handles data access

### Exception Handling

- Throw `ArgumentException` for invalid parameters
- Use meaningful exception messages
- Catch specific exceptions, not general `Exception`
- Avoid catching and swallowing exceptions

## Testing Requirements

### Unit Test Standards

- Test naming convention: `MethodName_StateUnderTest_ExpectedBehavior`
- Use MSTest `[TestClass]` and `[TestMethod]` attributes
- Arrange-Act-Assert (AAA) pattern required
- Mock external dependencies with Moq
- Use `It.IsAny<T>()` for flexible stub matching
- Verify mocks with `Times.Once()`, `Times.Never()`, etc.

### Test Coverage

- Minimum 80% code coverage required
- Test happy path and error scenarios
- Test null/empty edge cases
- Test boundary conditions

### Assertion Style

- Use FluentAssertions for all assertions
- Examples:
  - `result.Should().NotBeNull()`
  - `result.Should().Be(expected)`
  - `result.Should().HaveCount(5)`
  - `result.Should().BeTrue()`
  - `result.All(x => x.IsActive).Should().BeTrue()`
- Never use MSTest's `Assert.AreEqual()` or `Assert.IsTrue()`

## Security Requirements

- Never hardcode secrets or connection strings
- Use environment variables or configuration for sensitive data
- Validate all user input
- Use parameterized queries for database operations (EF Core does this)
- Follow the principle of least privilege

## Code Organization

### File Structure

- One public class per file (unless closely related)
- Order: fields, constructor, public methods, private methods
- Keep related classes in same namespace: `Cinnova.Product`

### Dependencies

- Use dependency injection via constructor
- Never create instances of services directly
- Program should use built-in DI container

### DTOs and Records

- Use C# records for DTOs (immutable data transfer)
- No business logic in DTOs
- Example: `public record ProductDto(int Id, string Name, decimal Price, bool IsActive);`

## Repository and Service Layer

### Repository Pattern

- Interface: `IProductRepository`
- Implementation: `ProductRepository`
- All database operations must be async
- Always include CancellationToken parameter
- Handle DbContext operations properly

### Service Layer

- Contains business logic, validation, and orchestration
- Depends on repositories, not DbContext
- All public methods must be async
- Include CancellationToken in all async methods

## Build and Deployment

### GitHub Actions Workflow

- Must trigger on push/PR to main and develop
- Jobs: restore → build → test → publish
- NuGet caching required
- Code coverage collection required (XPlat)
- SonarCloud analysis required
- Test results must be uploaded
- Publish only on main branch push

## Common Issues to Flag

### Red Flags

- ❌ Synchronous database calls (no async/await)
- ❌ Missing CancellationToken parameters
- ❌ Direct DbContext access in services
- ❌ `Task.Result` or `.Wait()` usage
- ❌ Catching all exceptions
- ❌ Hardcoded values/secrets
- ❌ Missing or inadequate test coverage
- ❌ Assertions not using FluentAssertions

### Green Flags

- ✅ Proper async/await usage
- ✅ CancellationToken threading
- ✅ Dependency injection
- ✅ Comprehensive error handling
- ✅ Good test coverage with FluentAssertions
- ✅ Clear separation of concerns
- ✅ Repository pattern correctly implemented
- ✅ SonarCloud passing

## Review Checklist for Copilot

When reviewing code, ensure:

- [ ] All methods follow naming conventions
- [ ] Async/await used correctly with CancellationToken
- [ ] No synchronous blocking calls
- [ ] DTOs/records defined properly
- [ ] Tests follow `MethodName_StateUnderTest_ExpectedBehavior` pattern
- [ ] FluentAssertions used exclusively
- [ ] 80%+ code coverage
- [ ] No hardcoded secrets
- [ ] Dependency injection used
- [ ] Single responsibility principle followed
- [ ] Repository pattern correctly implemented
- [ ] No catch-all exception handlers
- [ ] Database operations are async
- [ ] Early returns for validation
- [ ] Nesting depth ≤ 3 levels

## ADO Integration (Azure DevOps)

When applicable:

- Link commits to work items
- Use conventional commit format: `type: description #123`
- Examples: `feat: add product count method #45`, `fix: handle null products #67`
- Mark work items as completed when code merged

## Example: Good Code

```csharp
public interface IProductRepository
{
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductDto product, CancellationToken cancellationToken = default);
}

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> GetProductCountAsync(CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products.Count();
    }

    public async Task AddAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        if (product?.Price <= 0)
            throw new ArgumentException("Price must be greater than 0");

        await _repository.AddAsync(product, cancellationToken);
    }
}
```

## Example: Good Test

```csharp
[TestClass]
public class ProductServiceTests
{
    private Mock<IProductRepository> _repositoryMock;
    private ProductService _service;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new ProductService(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task GetProductCountAsync_RepositoryReturnsProducts_ReturnsCorrectCount()
    {
        // Arrange
        var products = new List<ProductDto> { new(1, "A", 10, true), new(2, "B", 20, false) };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var count = await _service.GetProductCountAsync();

        // Assert
        count.Should().Be(2);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## References

- [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Async/Await Best Practices](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [FluentAssertions Documentation](https://fluentassertions.com/)
