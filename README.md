# EFCore Repository

A lightweight and extensible **Repository Pattern implementation for Entity Framework Core**, designed to promote clean architecture, testability, and maintainability in .NET applications.

This library provides a structured abstraction over EF Core, allowing you to separate data access concerns from business logic while still leveraging EF Core’s full power.

---

## Features

- Clean implementation of the Repository Pattern.
- Designed for Entity Framework Core.
- Supports generic base entities and key-based entities.
- Encourages testable and modular architecture.
- Compatible with the Unit of Work pattern.
- Minimal, opinionated abstractions without over-engineering.

---

## Framework Support

- [.NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Entity Framework Core 10.0.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/10.0.0)

---

## Installation

Install via NuGet:

```bash
dotnet add package Opticient.EFCore.Repository
```

Or via the NuGet Package Manager:

```powershell
Install-Package Opticient.EFCore.Repository
```

Alternatively, you may clone or [download](https://github.com/OpticientLtd/EFCoreRepository) the source code and build it locally.

---

## Usage

### Entity Types

The library provides two base entity types:

1. `EntityBase`  
   - Base class for all entities.
   - All database tables without PrimaryKey (entities) should inherit from `EntityBase`.

2. `IdEntity<TKey>`  
   - Inherits from 'EntityBase' and for entities that expose a strongly typed primary key.
   - TKey - DataType of the Key (PrimaryKey in Db Table) of the Entity.
   - All database tables having PrimaryKey (entities) should inherit from `IdEntity<TKey>`.

### Extending Entities

All Entities are abstract and are intended to be extended per entity (table):

```csharp
public class Customer : IdEntity<int>
{
   // Add members of Customer Entity except 'Id'.
}
```
---

### Repository Types

The library includes abstract base repositories that build upon each other to support
common data access patterns:

1. `BaseRepository`  
   - A foundational base repository with no operations, intended to provide shared
   infrastructure for all repositories.

2. `EntityBaseRepository`  
   - Inherits from `BaseRepository` and provides basic read operations
   for entities derived from `EntityBase`.

3. `IdEntityReadRepository<TKey>`  
   - Inherits from `EntityBaseRepository` and extends read operations
   to support entities identified by a strongly typed primary key - `IdEntity<TKey>`.

4. `IdEntityRepository<TKey>`  
   - Inherits from `IdEntityReadRepository<TKey>` and adds support for
   create, update, and delete operations for entities identified by a strongly typed primary key - `IdEntity<TKey>`.

### Extending Repositories

All Repositories are abstract and are intended to be extended per entity (table):

#### Example

```csharp
public interface ICustomerRepository : IIdEntityRepository<Customer, int>
{
    // Add custom methods specific to Customer Entity.
}

public class CustomerRepository : IdEntityRepository<Customer, int>, ICustomerRepository
{
    public CustomerRepository(DbContext context, ILogger<CustomerRepository> logger) : base(context, logger)
    {
    }
}
```

---

### Unit of Work

This library is designed to work seamlessly with the Unit of Work pattern.  
You are free to implement your own `IUnitOfWork` abstraction on top of these repositories.

#### Example

```csharp
public interface IUnitOfWork
{
    ICustomerRepository Customers{ get; }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly ILoggerFactory _loggerFactory;
    private ICustomerRepository _customerRepository;

    public UnitOfWork(IDbContextFactory<DbContext> contextFactory, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _dbContext = contextFactory.CreateDbContext();
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }
    public ICustomerRepository Customers => _customerRepository ??= new CustomerRepository(_dbContext, _loggerFactory.CreateLogger<CustomerRepository>());
}
```
---

### Examples

For concrete examples of:
- Entity definitions
- Repository implementations
- Usage patterns

Please refer to the Test project included in the solution.

---

## Contributing

Contributions are welcome.

- Found a bug? Create an issue.
- Have a feature request? Start a discussion.
- Want to contribute? Fork the repository and submit a pull request.

Please ensure all changes include appropriate tests.

---

## License

Copyright © 2026 [Opticient Ltd](https://www.opticient.co.uk/).

This project is licensed under the MIT License.  
See https://opensource.org/licenses/MIT for details.

---

## Contact

For questions, feedback, or support, please contact the author at [Contact](contact.person@opticient.co.uk).