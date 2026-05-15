using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Tests
{
    public class TransactionServiceTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact]
        public async Task GetDashboardDataAsync_ShouldCalculateTotalsCorrectly()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var service = new TransactionService(dbContext);
            var userId = Guid.NewGuid();
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 31);

            var categoryIncome = new Category { CategoryId = 1, Title = "Salary", Type = CategoryType.Income, UserId = userId };
            var categoryExpense = new Category { CategoryId = 2, Title = "Rent", Type = CategoryType.Expense, UserId = userId };
            
            dbContext.Categories.AddRange(categoryIncome, categoryExpense);
            dbContext.Transactions.AddRange(
                new Transaction { TransactionId = 1, Name = "Monthly Salary", Amount = 5000, CategoryId = 1, Date = startDate.AddDays(5), UserId = userId },
                new Transaction { TransactionId = 2, Name = "Freelance", Amount = 500, CategoryId = 1, Date = startDate.AddDays(10), UserId = userId },
                new Transaction { TransactionId = 3, Name = "Rent Payment", Amount = 1200, CategoryId = 2, Date = startDate.AddDays(1), UserId = userId }
            );
            await dbContext.SaveChangesAsync();

            // Action
            var result = await service.GetDashboardDataAsync(userId, startDate, endDate);

            // Assert
            result.TotalIncome.Should().Be(5500.ToString("C2"));
            result.TotalExpense.Should().Be(1200.ToString("C2"));
            result.Balance.Should().Be(4300.ToString("C2"));
        }

        [Fact]
        public async Task GetDashboardDataAsync_ShouldFilterByDateRange()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var service = new TransactionService(dbContext);
            var userId = Guid.NewGuid();
            var targetMonthStart = new DateTime(2023, 2, 1);
            var targetMonthEnd = new DateTime(2023, 2, 28);

            var category = new Category { CategoryId = 1, Title = "Food", Type = CategoryType.Expense, UserId = userId };
            dbContext.Categories.Add(category);

            dbContext.Transactions.AddRange(
                new Transaction { TransactionId = 1, Name = "Jan Expense", Amount = 100, CategoryId = 1, Date = new DateTime(2023, 1, 15), UserId = userId },
                new Transaction { TransactionId = 2, Name = "Feb Expense", Amount = 200, CategoryId = 1, Date = new DateTime(2023, 2, 15), UserId = userId },
                new Transaction { TransactionId = 3, Name = "Mar Expense", Amount = 300, CategoryId = 1, Date = new DateTime(2023, 3, 15), UserId = userId }
            );
            await dbContext.SaveChangesAsync();

            // Action
            var result = await service.GetDashboardDataAsync(userId, targetMonthStart, targetMonthEnd);

            // Assert
            result.TotalExpense.Should().Be(200.ToString("C2"));
        }

        [Fact]
        public async Task GetDashboardDataAsync_ShouldIsolateUserData()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var service = new TransactionService(dbContext);
            var userA = Guid.NewGuid();
            var userB = Guid.NewGuid();
            var date = new DateTime(2023, 1, 1);

            var category = new Category { CategoryId = 1, Title = "Misc", Type = CategoryType.Expense, UserId = Guid.Empty }; // Shared category logic? Let's use specific ones
            dbContext.Categories.Add(category);

            dbContext.Transactions.AddRange(
                new Transaction { TransactionId = 1, Name = "User A Transaction", Amount = 100, CategoryId = 1, Date = date, UserId = userA },
                new Transaction { TransactionId = 2, Name = "User B Transaction", Amount = 500, CategoryId = 1, Date = date, UserId = userB }
            );
            await dbContext.SaveChangesAsync();

            // Action
            var result = await service.GetDashboardDataAsync(userA, date.AddDays(-1), date.AddDays(1));

            // Assert
            result.TotalExpense.Should().Be(100.ToString("C2"));
        }
    }
}
