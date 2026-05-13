# Cinnova .NET Team — Copilot Review Instructions

## Code Style

- All public APIs must have XML documentation comments (///).
- Follow namespace convention: Cinnova.[Domain].[Feature] (e.g. Cinnova.Orders.Services).
- Prefer async/await throughout; never use .Result or .Wait() on Task.
- Use nullable reference types (#nullable enable); all parameters must be non-nullable or explicitly annotated with ?.
- Use record types for DTOs and value objects where applicable.
  CINNOVA | GitHub Copilot Training Plan | .NET Team March 2026 | Confidential
  Cinnova .NET Development Team | All training resources are free Page 9

## Testing

- Unit tests use MSTest with Moq for mocking. Test class name: [ClassName]Tests.
- Every public service method must have at least: happy-path, null-input, and exception tests.
- Do not test EF Core DbContext directly; use an in-memory provider or mock IRepository.

## Security

- Never log sensitive data (passwords, tokens, PII).
- Validate all inputs before passing to EF Core or Dataverse API.
- Flag any use of dynamic SQL or raw string interpolation in queries.

## Azure DevOps

- Every PR description must include the ADO work item ID: Resolves AB#<id>.
- Commit messages follow: [AB#<id>] <type>: <short description>.

## Deprecated Patterns

- Flag any use of HttpClient created with new HttpClient() — use IHttpClientFactory.
- Flag synchronous EF Core calls (e.g. .ToList() without Async).
- Flag any use of var where the type is not obvious from the right-hand side.

# Cinnova.Product

This feature contains the Product entity, repository, service, controller, and tests for a .NET 8 application following Cinnova namespace conventions.

- `ProductDto`: Data transfer object for products
- `IProductRepository`: Repository interface for async CRUD operations
- `ProductRepository`: EF Core implementation
- `ProductService`: Business logic (e.g., price validation)
- `ProductController`: ASP.NET Core API endpoints
- `ProductServiceTests`: MSTest unit tests
