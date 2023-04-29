# EFCore Repository
.Net 6.0 Library for Repository Pattern

A simple package for Repository Pattern with Entity Framework Core.

It has implementations for two types of Repositories
1. ReadRepository (supports only Read operations)
2. Repository (supports Read & Write operations)

Above both are abstract classes, please extend them for each repository for each individual entity (Table in Db)
You can use it with UnitOfWork pattern as well.

Please extend the Entity (Table) from DbIdEntity class.

# Frameworks
1. Net 6.0
2. Entity FrameworkCore 6.0.16
