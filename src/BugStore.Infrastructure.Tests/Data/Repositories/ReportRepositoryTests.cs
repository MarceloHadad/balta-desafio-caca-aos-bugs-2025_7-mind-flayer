using BugStore.Application.UseCases.Reports.BestCustomers;
using BugStore.Application.UseCases.Reports.RevenueByPeriod;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Data.Repositories;
using BugStore.Infrastructure.Tests.Data.Helpers;
using FluentAssertions;

namespace BugStore.Infrastructure.Tests.Data.Repositories;

public class ReportRepositoryTests
{
    private static AppDbContext CreateInMemoryContext()
        => TestDbContextFactory.CreateInMemoryContext();

    private static async Task SeedTestData(AppDbContext context)
    {
        var customer1 = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "+1 555-1234",
            BirthDate = new DateTime(1985, 5, 15)
        };
        var customer2 = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Jane Smith",
            Email = "jane@example.com",
            Phone = "+1 555-5678",
            BirthDate = new DateTime(1990, 8, 20)
        };

        var product1 = new Product
        {
            Id = Guid.NewGuid(),
            Title = "Product A",
            Description = "Description A",
            Slug = "product-a",
            Price = 100m
        };

        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            CreatedAt = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc)
        };
        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            CreatedAt = new DateTime(2024, 1, 20, 14, 0, 0, DateTimeKind.Utc)
        };
        var order3 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            CreatedAt = new DateTime(2024, 2, 10, 11, 0, 0, DateTimeKind.Utc)
        };

        var order4 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer2.Id,
            CreatedAt = new DateTime(2024, 2, 5, 9, 0, 0, DateTimeKind.Utc)
        };
        var order5 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer2.Id,
            CreatedAt = new DateTime(2024, 2, 25, 16, 0, 0, DateTimeKind.Utc)
        };

        var orderLine1 = new OrderLine { Id = Guid.NewGuid(), OrderId = order1.Id, ProductId = product1.Id, Quantity = 2, Total = 200m };
        var orderLine2 = new OrderLine { Id = Guid.NewGuid(), OrderId = order2.Id, ProductId = product1.Id, Quantity = 2, Total = 200m };
        var orderLine3 = new OrderLine { Id = Guid.NewGuid(), OrderId = order3.Id, ProductId = product1.Id, Quantity = 3, Total = 300m };
        var orderLine4 = new OrderLine { Id = Guid.NewGuid(), OrderId = order4.Id, ProductId = product1.Id, Quantity = 1, Total = 100m };
        var orderLine5 = new OrderLine { Id = Guid.NewGuid(), OrderId = order5.Id, ProductId = product1.Id, Quantity = 4, Total = 400m };

        context.Customers.AddRange(customer1, customer2);
        context.Products.Add(product1);
        context.Orders.AddRange(order1, order2, order3, order4, order5);
        context.OrderLines.AddRange(orderLine1, orderLine2, orderLine3, orderLine4, orderLine5);

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetBestCustomersAsync_ShouldReturnAllCustomers_WhenNoFiltersProvided()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new BestCustomersRequest();

        // Act
        var result = await repository.GetBestCustomersAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var customer1 = result.FirstOrDefault(c => c.CustomerName == "John Doe");
        customer1.Should().NotBeNull();
        customer1!.TotalOrders.Should().Be(3);
        customer1.SpentAmount.Should().Be(700m);

        var customer2 = result.FirstOrDefault(c => c.CustomerName == "Jane Smith");
        customer2.Should().NotBeNull();
        customer2!.TotalOrders.Should().Be(2);
        customer2.SpentAmount.Should().Be(500m);
    }

    [Fact]
    public async Task GetBestCustomersAsync_ShouldFilterByName_WhenNameProvided()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new BestCustomersRequest { CustomerName = "John" };

        // Act
        var result = await repository.GetBestCustomersAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].CustomerName.Should().Be("John Doe");
        result[0].TotalOrders.Should().Be(3);
    }

    [Fact]
    public async Task GetBestCustomersAsync_ShouldFilterByEmail_WhenEmailProvided()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new BestCustomersRequest { CustomerEmail = "jane@" };

        // Act
        var result = await repository.GetBestCustomersAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].CustomerName.Should().Be("Jane Smith");
    }

    [Fact]
    public async Task GetBestCustomersAsync_ShouldFilterByMinOrders_WhenMinOrdersProvided()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new BestCustomersRequest { MinOrders = 3 };

        // Act
        var result = await repository.GetBestCustomersAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].CustomerName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetBestCustomersAsync_ShouldOrderBySpentAmountDescending_WhenSortBySpentAmountDesc()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new BestCustomersRequest { OrderBy = "spentAmount", OrderDirection = "desc" };

        // Act
        var result = await repository.GetBestCustomersAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].SpentAmount.Should().Be(700m);
        result[1].SpentAmount.Should().Be(500m);
    }

    [Fact]
    public async Task GetRevenueByPeriodAsync_ShouldReturnAllPeriods_WhenNoFiltersProvided()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new RevenueByPeriodRequest();

        // Act
        var result = await repository.GetRevenueByPeriodAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var jan2024 = result.FirstOrDefault(r => r.Year == 2024 && r.Month == "January");
        jan2024.Should().NotBeNull();
        jan2024!.TotalOrders.Should().Be(2);
        jan2024.TotalRevenue.Should().Be(400m);

        var feb2024 = result.FirstOrDefault(r => r.Year == 2024 && r.Month == "February");
        feb2024.Should().NotBeNull();
        feb2024!.TotalOrders.Should().Be(3);
        feb2024.TotalRevenue.Should().Be(800m);
    }

    [Fact]
    public async Task GetRevenueByPeriodAsync_ShouldFilterByStartPeriod_WhenStartPeriodProvided()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new RevenueByPeriodRequest { StartPeriod = "2024-02" };

        // Act
        var result = await repository.GetRevenueByPeriodAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Year.Should().Be(2024);
        result[0].Month.Should().Be("February");
    }

    [Fact]
    public async Task GetRevenueByPeriodAsync_ShouldFilterByPeriodRange_WhenBothPeriodsProvided()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new RevenueByPeriodRequest { StartPeriod = "2024-02", EndPeriod = "2024-02" };

        // Act
        var result = await repository.GetRevenueByPeriodAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Month.Should().Be("February");
    }

    [Fact]
    public async Task GetRevenueByPeriodAsync_ShouldFilterByMinOrders_WhenMinOrdersProvided()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new RevenueByPeriodRequest { MinOrders = 3 };

        // Act
        var result = await repository.GetRevenueByPeriodAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Month.Should().Be("February");
        result[0].TotalOrders.Should().Be(3);
    }

    [Fact]
    public async Task GetRevenueByPeriodAsync_ShouldOrderByDateAscending_WhenSortByDateAsc()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new RevenueByPeriodRequest { OrderBy = "date", OrderDirection = "asc" };

        // Act
        var result = await repository.GetRevenueByPeriodAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Month.Should().Be("January");
        result[1].Month.Should().Be("February");
    }

    [Fact]
    public async Task GetRevenueByPeriodAsync_ShouldHandleInvalidPeriodFormat_Gracefully()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var repository = new ReportRepository(context);

        var request = new RevenueByPeriodRequest { StartPeriod = "invalid-format" };

        // Act
        var result = await repository.GetRevenueByPeriodAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
}
