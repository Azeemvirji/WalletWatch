# WalletWatch - Project Instructions

## Project Overview
WalletWatch is a web-based expense tracking application built using **ASP.NET Core MVC**. It allows users to track their income and expenses, manage categories, and visualize financial data through an interactive dashboard.

### Core Technology Stack
- **Framework:** .NET 10.0 (ASP.NET Core Web App MVC)
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Identity:** ASP.NET Core Identity (Roles-based)
- **UI Components:** Syncfusion EJ2 for ASP.NET Core
- **Styling:** Custom CSS and Bootstrap

## Architecture & Design
The project follows the standard **MVC (Model-View-Controller)** pattern:
- **Models:** Defined in `ExpenseTracker/Models/`. Core entities include `Transaction` and `Category`.
- **Views:** Located in `ExpenseTracker/Views/`. Uses Razor syntax.
- **Controllers:** Located in `ExpenseTracker/Controllers/`. Handles business logic and routing.
- **Identity:** Managed in `ExpenseTracker/Areas/Identity/`.

### Data Isolation
All data (Transactions and Categories) is isolated per user. Both `Transaction` and `Category` models include a `UserId` property to ensure users only see their own data.

## Building and Running
To build and run the application locally, use the following commands from the `ExpenseTracker` directory:

```powershell
# Restore dependencies and build the project
dotnet build

# Run the application
dotnet run
```

### Database Management
The project uses EF Core migrations. Ensure you have the `dotnet-ef` tool installed.

```powershell
# Add a new migration
dotnet ef migrations add <MigrationName> --project ExpenseTracker.csproj

# Update the database
dotnet ef database update --project ExpenseTracker.csproj
```

## Development Conventions
- **Authentication:** Most controllers are protected with the `[Authorize]` attribute.
- **Validation:** Use Data Annotations in Models for server-side and client-side validation.
- **UI:** Syncfusion components are heavily used for charts and grids. License registration is handled in `Program.cs`.
- **Error Handling:** Centralized error handling is configured in `Program.cs` and managed by `ErrorController`.
- **Static Assets:** CSS, JS, and images are located in `ExpenseTracker/wwwroot/`.

## Key Files & Directories
- `ExpenseTracker/Program.cs`: Application configuration and middleware pipeline.
- `ExpenseTracker/Models/ApplicationDbContext.cs`: Database context and entity configurations.
- `ExpenseTracker/Controllers/DashboardController.cs`: Main dashboard logic including data aggregation for charts.
- `ExpenseTracker/appsettings.json`: Configuration settings (Connection Strings, Syncfusion keys, etc.).
