# EFCore Repository

A simple package for Repository Pattern with Entity Framework Core.

This is a Library for Repository pattern with C# and Entity Framework Core (.Net 6.0). The Repository pattern is a well-known design pattern that allows you to separate the code that accesses your data from the rest of your application code. This can make your code more modular, easier to test, and easier to maintain.

# Frameworks
1. Net 6.0
2. Entity FrameworkCore 6.0.16


## Installation
You can install this as [Nuget package.](https://www.nuget.org/packages/Opticient.EFCore.Repository)


Alternatively, you can download the source code as a ZIP file and extract it to your local machine. You can then open the solution file in Visual Studio and build the project.

## Usage

It has implementations for two types of Repositories
1. ReadRepository (supports only Read operations)
2. Repository (supports Read & Write operations)

Above both are abstract classes, please extend them for each repository for each individual entity (Table in Db)
You can use it with UnitOfWork pattern as well.

Please extend the Entity (Table) from DbIdEntity class.

To understand how entities and repositories should be created, please refer Tests project.

## Contributing
If you find a bug or have a feature request, please create an issue on GitHub. If you would like to contribute to the project, please fork the repository and submit a pull request.

## License
This demo application is licensed under the MIT License. See the LICENSE file for more information.

## Contact
If you have any questions or feedback on this demo application, please feel free to contact the author at [Contact](contact.person@opticient.co.uk).