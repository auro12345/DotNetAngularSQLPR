using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Repositories;
using Xunit;

namespace ProductManagement.Tests;

public class ProductDeleteRepositoryTests
{
    [Fact]
    public void AIK_005_AC_1_existingProduct_isRemovedFromSqlRepositoryContext()
    {
        var options = new DbContextOptionsBuilder<ProductManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new ProductManagementDbContext(options);
        context.Products.Add(new Product(7, "Laptop", "", 50000m, "Electronics"));
        context.SaveChanges();
        var repository = new SqlProductRepository(context);

        var deleted = repository.Delete(7);

        Assert.True(deleted);
        Assert.Empty(context.Products);
    }

    [Fact]
    public void AIK_005_AC_2_missingProduct_returnsFalse()
    {
        var options = new DbContextOptionsBuilder<ProductManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new ProductManagementDbContext(options);
        var repository = new SqlProductRepository(context);

        var deleted = repository.Delete(404);

        Assert.False(deleted);
    }
}
