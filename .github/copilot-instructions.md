# GitHub Copilot Instructions for MeeshoWebClone

## Project Overview

This is an ASP.NET Core 8.0 MVC web application that clones the Meesho e-commerce platform. The application allows users to browse products, manage carts, process checkouts, and includes an admin panel for management operations.

## Technology Stack

- **Framework**: ASP.NET Core 8.0 (MVC)
- **Database**: SQL Server with Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity with custom User model
- **Architecture Pattern**: CQRS using MediatR (12.5.0)
- **UI**: Razor Views with server-side rendering

## Project Structure

- **Controllers/**: Main application controllers (Account, Cart, Checkout, Home, Like)
- **Areas/Admin/**: Admin area with separate controllers and views
- **Models/**: Domain models and entities
- **ViewModels/**: View-specific models for data transfer
- **Data/**: Database context (MeeshoAppDbContext)
- **Commands/**: CQRS command definitions
- **Queries/**: CQRS query definitions
- **Handler/**: MediatR command and query handlers
- **Enums/**: Application enumerations (e.g., VerificationStatus)
- **Views/**: Razor views organized by controller
- **wwwroot/**: Static files (CSS, JavaScript, images, libraries)

## Architecture Patterns

### CQRS with MediatR
- Use MediatR for command and query handling
- Commands go in the `Commands/` directory
- Queries go in the `Queries/` directory
- Handlers go in the `Handler/Commands/` and `Handler/Queries/` directories
- Always implement `IRequest` or `IRequest<T>` for commands/queries
- Always implement `IRequestHandler` for handlers

### Entity Framework Core
- Use `MeeshoAppDbContext` for database operations
- Apply migrations using Entity Framework Core CLI
- Follow code-first approach for database schema
- Use async/await for all database operations

### ASP.NET Core Identity
- Custom `User` model extends `IdentityUser<Guid>`
- Three roles: Admin, User, Seller
- Default users:
  - Admin: admin@meesho.com (password: Admin@123)
  - Seller: seller@meesho.com (password: Seller@123)
- Email confirmation is disabled by default
- Minimum password length is 6 characters

## Coding Conventions

### General Guidelines
- Use C# 12 features and nullable reference types (`<Nullable>enable</Nullable>`)
- Use implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`)
- Follow standard ASP.NET Core MVC conventions
- Use async/await for all I/O operations
- Use dependency injection for services

### Naming Conventions
- Controllers: PascalCase with "Controller" suffix (e.g., `AccountController`)
- Actions: PascalCase (e.g., `Index`, `Login`)
- Models/ViewModels: PascalCase (e.g., `User`, `LoginViewModel`)
- Commands/Queries: PascalCase with descriptive names (e.g., `GetCartItemListQuery`)
- Handlers: PascalCase with "Handler" suffix (e.g., `GetCartItemListHandler`)
- Private fields: camelCase (standard C# convention)

### Code Organization
- Keep controllers thin; delegate business logic to handlers
- Use ViewModels for view-specific data transfer
- Separate read operations (queries) from write operations (commands)
- Place admin-specific functionality in the Admin area
- Keep models clean and focused on domain logic

## Database

### Connection String
- Configuration key: `MeeshoDbContext` in appsettings.json
- Uses SQL Server

### Migrations
- Migrations are applied automatically on application startup via `Program.cs`
- Run `dotnet ef migrations add <MigrationName>` to create new migrations
- Database is seeded with default roles and users on startup

## Building and Running

### Build the Project
```bash
dotnet build
```

### Run the Application
```bash
dotnet run --project MeeshoWebClone/MeeshoWebClone.csproj
```

### Restore Packages
```bash
dotnet restore
```

### Create Migrations
```bash
dotnet ef migrations add <MigrationName> --project MeeshoWebClone
```

### Update Database
```bash
dotnet ef database update --project MeeshoWebClone
```

## Authentication & Authorization

- Use `[Authorize]` attribute for protected actions
- Use `[Authorize(Roles = "Admin")]` for admin-only actions
- Access denied redirects to `/Account/AccessDenied`
- Authentication cookie configuration is in `Program.cs`

## Areas

### Admin Area
- Route pattern: `{area:exists}/{controller=Home}/{action=Index}/{id?}`
- Restricted to Admin role
- Contains administrative controllers and views

## Best Practices

- Always validate user input using model validation attributes
- Use ViewModels instead of passing domain models directly to views
- Implement proper error handling and user-friendly error messages
- Follow RESTful conventions for controller actions where applicable
- Use strongly-typed views
- Keep business logic out of controllers and views
- Use MediatR for complex operations to maintain separation of concerns
- Ensure all database operations are async
- Use proper HTTP verbs: GET for reads, POST for writes
- Implement CSRF protection (enabled by default in ASP.NET Core)

## Security Considerations

- Never store passwords in plain text (handled by Identity)
- Use HTTPS in production (configured in Program.cs)
- Validate and sanitize all user inputs
- Use parameterized queries (handled by EF Core)
- Implement proper authorization checks
- Keep sensitive configuration in appsettings or environment variables

## Dependencies

Key NuGet packages:
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.14)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.14)
- Microsoft.EntityFrameworkCore.Tools (8.0.14)
- MediatR (12.5.0)
- Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.7)
